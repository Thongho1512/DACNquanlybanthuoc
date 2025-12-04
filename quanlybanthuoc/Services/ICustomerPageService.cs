using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Dtos.DonGiaoHang;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Dtos.LichSuDiem;
using quanlybanthuoc.Dtos.ChiNhanh;

namespace quanlybanthuoc.Services
{
    /// <summary>
    /// Service cho các API c?a trang khách hàng (Customer Pages)
    /// </summary>
    public interface ICustomerPageService
    {
        /// <summary>
        /// L?y danh sách thu?c n?i b?t trên trang ch?
        /// </summary>
        Task<PagedResult<ThuocCustomerDto>> GetFeaturedMedicinesAsync(
            int pageNumber = 1,
            int pageSize = 12);

        /// <summary>
        /// Tìm ki?m và l?c thu?c nâng cao
        /// </summary>
        Task<PagedResult<ThuocCustomerDto>> SearchMedicinesAsync(
            string? searchTerm = null,
            int? idDanhMuc = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int pageNumber = 1,
            int pageSize = 12);

        /// <summary>
        /// L?y chi ti?t s?n ph?m
        /// </summary>
        Task<ThuocDetailDto?> GetMedicineDetailAsync(int id);

        /// <summary>
        /// L?y danh sách t?t c? danh m?c
        /// </summary>
        Task<IEnumerable<DanhMucDto>> GetCategoriesAsync();

        /// <summary>
        /// L?y thông tin cá nhân khách hàng
        /// </summary>
        Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId);

        /// <summary>
        /// C?p nh?t thông tin cá nhân khách hàng
        /// </summary>
        Task UpdateCustomerProfileAsync(int customerId, UpdateKhachHangDto dto);

        /// <summary>
        /// L?y l?ch s? mua hàng c?a khách hàng
        /// </summary>
        Task<PagedResult<DonHangDto>> GetOrderHistoryAsync(
            int customerId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// L?y chi ti?t ??n hàng
        /// </summary>
        Task<DonHangDto?> GetOrderDetailAsync(int orderId);

        /// <summary>
        /// Theo dõi tr?ng thái giao hàng
        /// </summary>
        Task<ShipmentTrackingDto?> TrackShipmentAsync(int orderId);

        /// <summary>
        /// L?y l?ch s? ?i?m tích l?y
        /// </summary>
        Task<PagedResult<LichSuDiemCustomerDto>> GetLoyaltyPointHistoryAsync(
            int customerId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// L?y danh sách chi nhánh có s?n ph?m
        /// </summary>
        Task<IEnumerable<ChiNhanhCustomerDto>> GetBranchesAsync();

        /// <summary>
        /// L?y t?n kho c?a s?n ph?m t?i chi nhánh
        /// </summary>
        Task<int?> GetStockByBranchAsync(int medicineId, int branchId);
    }
}
