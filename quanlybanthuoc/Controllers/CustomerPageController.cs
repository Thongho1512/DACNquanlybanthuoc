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
    /// API cho trang khách hàng (Customer Pages)
    /// Các endpoint này dành cho ng??i dùng cu?i ?? xem s?n ph?m, ??t hàng, theo dõi
    /// </summary>
    [Route("api/v1/customer")]
    [ApiController]
    public class CustomerPageController : ControllerBase
    {
        private readonly ILogger<CustomerPageController> _logger;
        private readonly ICustomerPageService _customerPageService;

        public CustomerPageController(
            ILogger<CustomerPageController> logger,
            ICustomerPageService customerPageService)
        {
            _logger = logger;
            _customerPageService = customerPageService;
        }

        #region ========== TRANG CH? & DANH SÁCH S?N PH?M ==========

        /// <summary>
        /// L?y danh sách s?n ph?m n?i b?t trên trang ch?
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
        /// Tìm ki?m và l?c s?n ph?m nâng cao
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
        /// L?y danh sách t?t c? danh m?c
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

        #region ========== CHI NHÁNH & ??A ?i?M ==========

        /// <summary>
        /// L?y danh sách t?t c? chi nhánh ?ang ho?t ??ng
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
        /// Ki?m tra t?n kho t?i chi nhánh c? th?
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

        #region ========== H? S? CÁ NHÂN (REQUIRE AUTH) ==========

        /// <summary>
        /// L?y thông tin cá nhân khách hàng
        /// Endpoint: GET /api/v1/customer/profile
        /// Yêu c?u: ??ng nh?p
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("Getting customer profile");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm th?y thông tin khách hàng."));

            var result = ApiResponse<CustomerProfileDto>.SuccessResponse(
                await _customerPageService.GetCustomerProfileAsync(customerId.Value));
            return Ok(result);
        }

        /// <summary>
        /// C?p nh?t thông tin cá nhân khách hàng
        /// Endpoint: PUT /api/v1/customer/profile
        /// Yêu c?u: ??ng nh?p
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateKhachHangDto dto)
        {
            _logger.LogInformation("Updating customer profile");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm th?y thông tin khách hàng."));

            await _customerPageService.UpdateCustomerProfileAsync(customerId.Value, dto);
            var result = ApiResponse<string>.SuccessResponse("C?p nh?t thông tin thành công.");
            return Ok(result);
        }

        #endregion

        #region ========== L?CH S? MUA HÀNG (REQUIRE AUTH) ==========

        /// <summary>
        /// L?y l?ch s? mua hàng c?a khách hàng
        /// Endpoint: GET /api/v1/customer/orders
        /// Yêu c?u: ??ng nh?p
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
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm th?y thông tin khách hàng."));

            var result = ApiResponse<PagedResult<DonHangDto>>.SuccessResponse(
                await _customerPageService.GetOrderHistoryAsync(customerId.Value, pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// L?y chi ti?t ??n hàng
        /// Endpoint: GET /api/v1/customer/orders/{orderId}
        /// Yêu c?u: ??ng nh?p
        /// </summary>
        [HttpGet("orders/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            _logger.LogInformation($"Getting order detail: {orderId}");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm th?y thông tin khách hàng."));

            var result = ApiResponse<DonHangDto>.SuccessResponse(
                await _customerPageService.GetOrderDetailAsync(orderId));
            return Ok(result);
        }

        #endregion

        #region ========== THEO DÕI GIAO HÀNG (REQUIRE AUTH) ==========

        /// <summary>
        /// Theo dõi tr?ng thái giao hàng
        /// Endpoint: GET /api/v1/customer/orders/{orderId}/shipment
        /// Yêu c?u: ??ng nh?p
        /// </summary>
        [HttpGet("orders/{orderId}/shipment")]
        [Authorize]
        public async Task<IActionResult> TrackShipment(int orderId)
        {
            _logger.LogInformation($"Tracking shipment for order: {orderId}");

            var customerId = GetCustomerId();
            if (!customerId.HasValue)
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm th?y thông tin khách hàng."));

            var result = ApiResponse<ShipmentTrackingDto>.SuccessResponse(
                await _customerPageService.TrackShipmentAsync(orderId));
            return Ok(result);
        }

        #endregion

        #region ========== ?I?M TÍCH L?Y (REQUIRE AUTH) ==========

        /// <summary>
        /// L?y l?ch s? ?i?m tích l?y
        /// Endpoint: GET /api/v1/customer/loyalty-points
        /// Yêu c?u: ??ng nh?p
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
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm th?y thông tin khách hàng."));

            var result = ApiResponse<PagedResult<LichSuDiemCustomerDto>>.SuccessResponse(
                await _customerPageService.GetLoyaltyPointHistoryAsync(customerId.Value, pageNumber, pageSize));
            return Ok(result);
        }

        #endregion

        #region ========== HELPER METHODS ==========

        /// <summary>
        /// L?y ID khách hàng t? token
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
