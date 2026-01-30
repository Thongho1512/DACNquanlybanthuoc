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
    /// Service cho c�c API c?a trang kh�ch h�ng (Customer Pages)
    /// </summary>
    public interface ICustomerPageService
    {
        /// <summary>
        /// L?y danh s�ch thu?c n?i b?t tr�n trang ch?
        /// </summary>
        Task<PagedResult<ThuocCustomerDto>> GetFeaturedMedicinesAsync(
            int pageNumber = 1,
            int pageSize = 12);

        /// <summary>
        /// T�m ki?m v� l?c thu?c n�ng cao
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
        /// L?y danh s�ch t?t c? danh m?c
        /// </summary>
        Task<IEnumerable<DanhMucDto>> GetCategoriesAsync();

        /// <summary>
        /// L?y th�ng tin c� nh�n kh�ch h�ng
        /// </summary>
        Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId);

        /// <summary>
        /// C?p nh?t th�ng tin c� nh�n kh�ch h�ng
        /// </summary>
        Task UpdateCustomerProfileAsync(int customerId, UpdateKhachHangDto dto);

        /// <summary>
        /// L?y l?ch s? mua h�ng c?a kh�ch h�ng
        /// </summary>
        Task<PagedResult<DonHangDto>> GetOrderHistoryAsync(
            int customerId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// L?y chi ti?t ??n h�ng
        /// </summary>
        Task<DonHangDto?> GetOrderDetailAsync(int orderId);

        /// <summary>
        /// Theo d�i tr?ng th�i giao h�ng
        /// </summary>
        Task<ShipmentTrackingDto?> TrackShipmentAsync(int orderId);

        /// <summary>
        /// L?y l?ch s? ?i?m t�ch l?y
        /// </summary>
        Task<PagedResult<LichSuDiemCustomerDto>> GetLoyaltyPointHistoryAsync(
            int customerId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// L?y danh s�ch chi nh�nh c� s?n ph?m
        /// </summary>
        Task<IEnumerable<ChiNhanhCustomerDto>> GetBranchesAsync();

        /// <summary>
        /// L?y t?n kho c?a s?n ph?m t?i chi nh�nh
        /// </summary>
        Task<int?> GetStockByBranchAsync(int medicineId, int branchId);

        /// <summary>
        /// Tra cứu đơn hàng bằng số điện thoại (không cần đăng nhập)
        /// </summary>
        Task<IEnumerable<DonHangDto>> GetOrdersByPhoneAsync(string phone);
    }
}
