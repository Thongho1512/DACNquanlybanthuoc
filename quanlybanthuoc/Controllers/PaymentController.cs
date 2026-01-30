using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Payment;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tạo payment request (Momo QR Code)
        /// Cho phép cả khách hàng đã đăng nhập và guest checkout
        /// </summary>
        [HttpPost("create")]
        [AllowAnonymous] // Cho phép guest checkout
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            _logger.LogInformation($"Creating payment for order {request.OrderId}");

            try
            {
                var result = await _paymentService.CreatePaymentAsync(request);
                return Ok(ApiResponse<PaymentResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating payment for order {request.OrderId}");
                return BadRequest(ApiResponse<PaymentResponse>.FailureResponse(ex.Message));
            }
        }

        /// <summary>
        /// Webhook callback từ Momo (AllowAnonymous vì Momo gọi từ bên ngoài)
        /// </summary>
        [HttpPost("notify")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentNotify([FromBody] Dictionary<string, string> callbackData)
        {
            _logger.LogInformation("Received payment callback from Momo");

            try
            {
                var orderId = callbackData.GetValueOrDefault("orderId", "");
                if (string.IsNullOrEmpty(orderId))
                {
                    _logger.LogWarning("Missing orderId in callback data");
                    return BadRequest(ApiResponse<string>.FailureResponse("Missing orderId"));
                }

                var success = await _paymentService.ProcessPaymentCallbackAsync(orderId, callbackData);

                if (success)
                {
                    return Ok(new { resultCode = 0, message = "Success" });
                }
                else
                {
                    return BadRequest(new { resultCode = -1, message = "Failed to process callback" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                return StatusCode(500, new { resultCode = -1, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy trạng thái thanh toán của đơn hàng
        /// Cho phép cả khách hàng đã đăng nhập và guest checkout
        /// </summary>
        [HttpGet("{orderId}/status")]
        [AllowAnonymous] // Cho phép guest checkout kiểm tra trạng thái
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            _logger.LogInformation($"Getting payment status for order {orderId}");

            try
            {
                var result = await _paymentService.GetPaymentStatusAsync(orderId);
                return Ok(ApiResponse<PaymentStatusResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment status for order {orderId}");
                return BadRequest(ApiResponse<PaymentStatusResponse>.FailureResponse(ex.Message));
            }
        }
    }
}

