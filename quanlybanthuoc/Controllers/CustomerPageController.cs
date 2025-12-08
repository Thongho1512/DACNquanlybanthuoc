using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Dtos.DonGiaoHang;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Dtos.LichSuDiem;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Controllers
{
    /// <summary>
    /// API cho trang kh�ch h�ng (Customer Pages)
    /// C�c endpoint n�y d�nh cho ng??i d�ng cu?i ?? xem s?n ph?m, ??t h�ng, theo d�i
    /// </summary>
    [Route("api/v1/customer")]
    [ApiController]
    public class CustomerPageController : ControllerBase
    {
        private readonly ILogger<CustomerPageController> _logger;
        private readonly ICustomerPageService _customerPageService;
        private readonly IDonHangService _donHangService;

        public CustomerPageController(
            ILogger<CustomerPageController> logger,
            ICustomerPageService customerPageService,
            IDonHangService donHangService)
        {
            _logger = logger;
            _customerPageService = customerPageService;
            _donHangService = donHangService;
        }

        #region ========== TRANG CH? & DANH S�CH S?N PH?M ==========

        /// <summary>
        /// L?y danh s�ch s?n ph?m n?i b?t tr�n trang ch?
        /// Endpoint: GET /api/v1/customer/medicines/featured
        /// </summary>
        [HttpGet("medicines/featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedMedicines(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12)
        {
            _logger.LogInformation("Getting featured medicines for homepage");
            var result = ApiResponse<PagedResult<ThuocCustomerDto>>.SuccessResponse(
                await _customerPageService.GetFeaturedMedicinesAsync(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// T�m ki?m v� l?c s?n ph?m n�ng cao
        /// Endpoint: GET /api/v1/customer/medicines/search
        /// </summary>
        [HttpGet("medicines/search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchMedicines(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12)
        {
            _logger.LogInformation($"Searching medicines: term='{searchTerm}', category={categoryId}");
            var result = ApiResponse<PagedResult<ThuocCustomerDto>>.SuccessResponse(
                await _customerPageService.SearchMedicinesAsync(
                    searchTerm, categoryId, minPrice, maxPrice, pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// L?y chi ti?t s?n ph?m
        /// Endpoint: GET /api/v1/customer/medicines/{id}
        /// </summary>
        [HttpGet("medicines/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMedicineDetail(int id)
        {
            _logger.LogInformation($"Getting medicine detail: {id}");
            var result = ApiResponse<ThuocDetailDto>.SuccessResponse(
                await _customerPageService.GetMedicineDetailAsync(id));
            return Ok(result);
        }

        #endregion

        #region ========== DANH M?C ==========

        /// <summary>
        /// L?y danh s�ch t?t c? danh m?c
        /// Endpoint: GET /api/v1/customer/categories
        /// </summary>
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("Getting all categories");
            var result = ApiResponse<IEnumerable<DanhMucDto>>.SuccessResponse(
                await _customerPageService.GetCategoriesAsync());
            return Ok(result);
        }

        #endregion

        #region ========== CHI NH�NH & ??A ?i?M ==========

        /// <summary>
        /// L?y danh s�ch t?t c? chi nh�nh ?ang ho?t ??ng
        /// Endpoint: GET /api/v1/customer/branches
        /// </summary>
        [HttpGet("branches")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBranches()
        {
            _logger.LogInformation("Getting all active branches");
            var result = ApiResponse<IEnumerable<ChiNhanhCustomerDto>>.SuccessResponse(
                await _customerPageService.GetBranchesAsync());
            return Ok(result);
        }

        /// <summary>
        /// Ki?m tra t?n kho t?i chi nh�nh c? th?
        /// Endpoint: GET /api/v1/customer/branches/{branchId}/medicines/{medicineId}/stock
        /// </summary>
        [HttpGet("branches/{branchId}/medicines/{medicineId}/stock")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckStock(int branchId, int medicineId)
        {
            _logger.LogInformation($"Checking stock for medicine {medicineId} at branch {branchId}");
            var stock = await _customerPageService.GetStockByBranchAsync(medicineId, branchId);
            var result = ApiResponse<StockResponseDto>.SuccessResponse(new StockResponseDto { SoLuongTon = stock ?? 0 });
            return Ok(result);
        }

        #endregion

        #region ========== H? S? C� NH�N (REQUIRE AUTH) ==========

        /// <summary>
        /// L?y th�ng tin c� nh�n kh�ch h�ng
        /// Endpoint: GET /api/v1/customer/profile
        /// Y�u c?u: ??ng nh?p
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("Getting customer profile");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Kh�ng t�m th?y th�ng tin kh�ch h�ng."));

            var result = ApiResponse<CustomerProfileDto>.SuccessResponse(
                await _customerPageService.GetCustomerProfileAsync(customerId.Value));
            return Ok(result);
        }

        /// <summary>
        /// C?p nh?t th�ng tin c� nh�n kh�ch h�ng
        /// Endpoint: PUT /api/v1/customer/profile
        /// Y�u c?u: ??ng nh?p
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateKhachHangDto dto)
        {
            _logger.LogInformation("Updating customer profile");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Kh�ng t�m th?y th�ng tin kh�ch h�ng."));

            await _customerPageService.UpdateCustomerProfileAsync(customerId.Value, dto);
            var result = ApiResponse<string>.SuccessResponse("C?p nh?t th�ng tin th�nh c�ng.");
            return Ok(result);
        }

        #endregion

        #region ========== L?CH S? MUA H�NG (REQUIRE AUTH) ==========

        /// <summary>
        /// L?y l?ch s? mua h�ng c?a kh�ch h�ng
        /// Endpoint: GET /api/v1/customer/orders
        /// Y�u c?u: ??ng nh?p
        /// </summary>
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrderHistory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting order history");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Kh�ng t�m th?y th�ng tin kh�ch h�ng."));

            var result = ApiResponse<PagedResult<DonHangDto>>.SuccessResponse(
                await _customerPageService.GetOrderHistoryAsync(customerId.Value, pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// L?y chi ti?t ??n h�ng
        /// Endpoint: GET /api/v1/customer/orders/{orderId}
        /// Y�u c?u: ??ng nh?p
        /// </summary>
        [HttpGet("orders/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            _logger.LogInformation($"Getting order detail: {orderId}");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Kh�ng t�m th?y th�ng tin kh�ch h�ng."));

            var result = ApiResponse<DonHangDto>.SuccessResponse(
                await _customerPageService.GetOrderDetailAsync(orderId));
            return Ok(result);
        }

        #endregion

        #region ========== THEO D�I GIAO H�NG (REQUIRE AUTH) ==========

        /// <summary>
        /// Theo d�i tr?ng th�i giao h�ng
        /// Endpoint: GET /api/v1/customer/orders/{orderId}/shipment
        /// Y�u c?u: ??ng nh?p
        /// </summary>
        [HttpGet("orders/{orderId}/shipment")]
        [Authorize]
        public async Task<IActionResult> TrackShipment(int orderId)
        {
            _logger.LogInformation($"Tracking shipment for order: {orderId}");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Kh�ng t�m th?y th�ng tin kh�ch h�ng."));

            var result = ApiResponse<ShipmentTrackingDto>.SuccessResponse(
                await _customerPageService.TrackShipmentAsync(orderId));
            return Ok(result);
        }

        #endregion

        #region ========== ?I?M T�CH L?Y (REQUIRE AUTH) ==========

        /// <summary>
        /// L?y l?ch s? ?i?m t�ch l?y
        /// Endpoint: GET /api/v1/customer/loyalty-points
        /// Y�u c?u: ??ng nh?p
        /// </summary>
        [HttpGet("loyalty-points")]
        [Authorize]
        public async Task<IActionResult> GetLoyaltyPointHistory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting loyalty point history");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Kh�ng t�m th?y th�ng tin kh�ch h�ng."));

            var result = ApiResponse<PagedResult<LichSuDiemCustomerDto>>.SuccessResponse(
                await _customerPageService.GetLoyaltyPointHistoryAsync(customerId.Value, pageNumber, pageSize));
            return Ok(result);
        }

        #endregion

        #region ========== TRA CỨU ĐƠN HÀNG BẰNG SỐ ĐIỆN THOẠI (NO AUTH) ==========

        /// <summary>
        /// Tra cứu đơn hàng bằng số điện thoại - Không cần đăng nhập
        /// Endpoint: GET /api/v1/customer/orders/by-phone?phone={sdt}
        /// </summary>
        [HttpGet("orders/by-phone")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrdersByPhone([FromQuery] string phone)
        {
            _logger.LogInformation($"Looking up orders by phone: {phone}");

            if (string.IsNullOrWhiteSpace(phone))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Vui lòng nhập số điện thoại."));
            }

            try
            {
                var result = ApiResponse<IEnumerable<DonHangDto>>.SuccessResponse(
                    await _customerPageService.GetOrdersByPhoneAsync(phone));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error looking up orders by phone: {phone}");
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }

        #endregion

        #region ========== ĐẶT HÀNG ONLINE ==========

        /// <summary>
        /// Đặt hàng online - Cho phép guest checkout (không cần đăng nhập)
        /// Endpoint: POST /api/v1/customer/orders
        /// </summary>
        [HttpPost("orders")]
        [AllowAnonymous] // Cho phép đặt hàng không cần đăng nhập
        public async Task<IActionResult> CreateOrder([FromBody] CreateCustomerOrderDto dto)
        {
            _logger.LogInformation("Creating customer order (online)");

            // Lấy CustomerId từ token nếu có (khách hàng đã đăng nhập)
            int? customerId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int id))
                {
                    customerId = id;
                    _logger.LogInformation($"Customer authenticated: {customerId}");
                }
            }

            try
            {
                var result = ApiResponse<DonHangDto>.SuccessResponse(
                    await _donHangService.CreateCustomerOrderAsync(dto, customerId));
                return CreatedAtAction(nameof(GetOrderDetail), new { orderId = result.Data?.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer order");
                return BadRequest(ApiResponse<DonHangDto>.FailureResponse(ex.Message));
            }
        }

        #endregion

        #region ========== HELPER METHODS ==========

        /// <summary>
        /// L?y ID kh�ch h�ng t? token
        /// </summary>
        private int? GetCustomerId()
        {
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int customerId))
                return null;

            return customerId;
        }

        #endregion
    }
}
