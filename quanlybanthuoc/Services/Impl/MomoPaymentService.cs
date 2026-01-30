using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using quanlybanthuoc.Data;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Payment;
using quanlybanthuoc.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace quanlybanthuoc.Services.Impl
{
    public class MomoPaymentService : IPaymentService
    {
        private readonly ILogger<MomoPaymentService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ShopDbContext _dbContext;

        // Momo API endpoints
        private readonly string _momoApiEndpoint;
        private readonly string _partnerCode;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _returnUrl;
        private readonly string _notifyUrl;

        public MomoPaymentService(
            ILogger<MomoPaymentService> logger,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ShopDbContext dbContext)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _dbContext = dbContext;

            // Load Momo configuration from appsettings.json
            var momoSettings = _configuration.GetSection("Momo");
            _momoApiEndpoint = momoSettings["ApiEndpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";
            _partnerCode = momoSettings["PartnerCode"] ?? "";
            _accessKey = momoSettings["AccessKey"] ?? "";
            _secretKey = momoSettings["SecretKey"] ?? "";
            _returnUrl = momoSettings["ReturnUrl"] ?? "";
            _notifyUrl = momoSettings["NotifyUrl"] ?? "";

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            _logger.LogInformation($"Creating Momo payment for order {request.OrderId}");

            // Validate order exists
            var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(request.OrderId);
            if (donHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn hàng với ID: {request.OrderId}");
            }

            // Check if order is already paid
            if (donHang.TrangThaiThanhToan == "PAID")
            {
                throw new BadRequestException("Đơn hàng đã được thanh toán.");
            }

            // Generate order ID for Momo (format: ORDER_{orderId}_{timestamp})
            var momoOrderId = $"ORDER_{request.OrderId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var orderInfo = $"Thanh toan don hang #{request.OrderId}";
            var amount = (long)request.Amount;
            var requestId = Guid.NewGuid().ToString();
            var extraData = "";

            // Build signature
            var rawSignature = $"accessKey={_accessKey}&amount={amount}&extraData={extraData}&ipnUrl={_notifyUrl}&orderId={momoOrderId}&orderInfo={orderInfo}&partnerCode={_partnerCode}&redirectUrl={_returnUrl}&requestId={requestId}&requestType=captureWallet";

            var signature = ComputeHmacSha256(rawSignature, _secretKey);

            // Build request body
            var requestBody = new
            {
                partnerCode = _partnerCode,
                partnerName = "QuanLyBanThuoc",
                storeId = "QuanLyBanThuoc",
                requestId = requestId,
                amount = amount,
                orderId = momoOrderId,
                orderInfo = orderInfo,
                redirectUrl = _returnUrl,
                ipnUrl = _notifyUrl,
                lang = "vi",
                extraData = extraData,
                requestType = "captureWallet",
                autoCapture = true,
                signature = signature
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Sending payment request to Momo: {jsonContent}");

            try
            {
                var response = await _httpClient.PostAsync(_momoApiEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Momo API response: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new BadRequestException($"Lỗi khi tạo payment request từ Momo: {responseContent}");
                }

                var momoResponse = JsonSerializer.Deserialize<MomoCreatePaymentResponse>(responseContent);

                if (momoResponse == null || momoResponse.ResultCode != 0)
                {
                    throw new BadRequestException($"Lỗi từ Momo: {momoResponse?.Message ?? "Unknown error"}");
                }

                // Update order with Momo order ID
                donHang.MomoOrderId = momoOrderId;
                donHang.TrangThaiThanhToan = "PENDING_PAYMENT";
                await _unitOfWork.DonHangRepository.UpdateAsync(donHang);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Payment created successfully. Momo OrderId: {momoOrderId}");

                return new PaymentResponse
                {
                    PaymentUrl = momoResponse.PayUrl,
                    QrCodeUrl = momoResponse.QrCodeUrl ?? momoResponse.PayUrl,
                    OrderId = momoOrderId,
                    DeepLink = momoResponse.DeepLink
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating Momo payment for order {request.OrderId}");
                throw new BadRequestException($"Lỗi khi tạo payment request: {ex.Message}");
            }
        }

        public async Task<bool> ProcessPaymentCallbackAsync(string orderId, Dictionary<string, string> callbackData)
        {
            _logger.LogInformation($"Processing payment callback for Momo OrderId: {orderId}");

            try
            {
                // Verify signature
                var signature = callbackData.GetValueOrDefault("signature", "");
                var amount = callbackData.GetValueOrDefault("amount", "0");
                var extraData = callbackData.GetValueOrDefault("extraData", "");
                var momoOrderId = callbackData.GetValueOrDefault("orderId", "");
                var resultCode = callbackData.GetValueOrDefault("resultCode", "");
                var transactionId = callbackData.GetValueOrDefault("transId", "");

                // Build signature string
                var rawSignature = $"accessKey={_accessKey}&amount={amount}&extraData={extraData}&message={callbackData.GetValueOrDefault("message", "")}&orderId={momoOrderId}&orderInfo={callbackData.GetValueOrDefault("orderInfo", "")}&orderType={callbackData.GetValueOrDefault("orderType", "")}&partnerCode={_partnerCode}&payType={callbackData.GetValueOrDefault("payType", "")}&requestId={callbackData.GetValueOrDefault("requestId", "")}&responseTime={callbackData.GetValueOrDefault("responseTime", "")}&resultCode={resultCode}&transId={transactionId}";

                var computedSignature = ComputeHmacSha256(rawSignature, _secretKey);

                if (signature != computedSignature)
                {
                    _logger.LogWarning($"Invalid signature for order {orderId}. Expected: {computedSignature}, Received: {signature}");
                    return false;
                }

                // Find order by MomoOrderId
                var donHang = await _unitOfWork.DonHangRepository
                    .GetByMomoOrderIdAsync(momoOrderId);

                if (donHang == null)
                {
                    _logger.LogWarning($"Order not found for Momo OrderId: {momoOrderId}");
                    return false;
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Process payment result
                    if (resultCode == "0") // Success
                    {
                        donHang.TrangThaiThanhToan = "PAID";
                        donHang.MomoTransactionId = transactionId;
                        donHang.NgayThanhToan = DateTime.Now;

                        // If order is delivery type (GIAO_HANG), create delivery order
                        if (donHang.LoaiDonHang == "GIAO_HANG")
                        {
                            await CreateDonGiaoHangIfNotExistsAsync(donHang);
                        }

                        _logger.LogInformation($"Payment successful for order {donHang.Id}. TransactionId: {transactionId}");
                    }
                    else
                    {
                        donHang.TrangThaiThanhToan = "FAILED";
                        _logger.LogWarning($"Payment failed for order {donHang.Id}. ResultCode: {resultCode}");
                    }

                    await _unitOfWork.DonHangRepository.UpdateAsync(donHang);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, $"Error processing payment callback for order {donHang.Id}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment callback: {ex.Message}");
                return false;
            }
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(int orderId)
        {
            _logger.LogInformation($"Getting payment status for order {orderId}");

            var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(orderId);
            if (donHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn hàng với ID: {orderId}");
            }

            return new PaymentStatusResponse
            {
                OrderId = orderId,
                PaymentStatus = donHang.TrangThaiThanhToan ?? "PENDING_PAYMENT",
                MomoOrderId = donHang.MomoOrderId,
                MomoTransactionId = donHang.MomoTransactionId,
                NgayThanhToan = donHang.NgayThanhToan
            };
        }

        private async Task CreateDonGiaoHangIfNotExistsAsync(DonHang donHang)
        {
            // Check if delivery order already exists
            var existingDelivery = await _dbContext.DonGiaoHangs
                .FirstOrDefaultAsync(dg => dg.IddonHang == donHang.Id);

            if (existingDelivery != null)
            {
                _logger.LogInformation($"Delivery order already exists for order {donHang.Id}");
                return;
            }

            // Create delivery order
            var donGiaoHang = new DonGiaoHang
            {
                IddonHang = donHang.Id,
                TrangThaiGiaoHang = "CHO_XAC_NHAN",
                NgayTao = DateTime.Now
            };

            // You can add more logic here to get delivery address from customer or order
            // For now, we'll create a basic delivery order

            await _dbContext.DonGiaoHangs.AddAsync(donGiaoHang);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Created delivery order for order {donHang.Id}");
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // Momo API response model
        private class MomoCreatePaymentResponse
        {
            public int ResultCode { get; set; }
            public string Message { get; set; } = string.Empty;
            public string PayUrl { get; set; } = string.Empty;
            public string? QrCodeUrl { get; set; }
            public string? DeepLink { get; set; }
        }
    }
}

