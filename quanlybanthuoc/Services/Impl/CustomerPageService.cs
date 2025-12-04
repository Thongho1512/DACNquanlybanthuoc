using AutoMapper;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Dtos.DonGiaoHang;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Dtos.LichSuDiem;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class CustomerPageService : ICustomerPageService
    {
        private readonly ILogger<CustomerPageService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerPageService(
            ILogger<CustomerPageService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ThuocCustomerDto>> GetFeaturedMedicinesAsync(int pageNumber = 1, int pageSize = 12)
        {
            _logger.LogInformation("Getting featured medicines for homepage");
            var pagedList = await _unitOfWork.ThuocRepository.GetPagedListAsync(pageNumber, pageSize, active: true, searchTerm: null, idDanhMuc: null);

            var thuocDtos = pagedList.Items.Select(thuoc => new ThuocCustomerDto
            {
                Id = thuoc.Id,
                TenThuoc = thuoc.TenThuoc,
                MoTa = thuoc.MoTa,
                GiaBan = thuoc.GiaBan,
                DonVi = thuoc.DonVi,
                HoatChat = thuoc.HoatChat,
                TenDanhMuc = thuoc.IddanhMucNavigation?.TenDanhMuc,
                SoLuongConLai = thuoc.LoHangs.Sum(lh => lh.KhoHangs.Sum(kh => kh.SoLuongTon ?? 0)),
                CoSan = (thuoc.LoHangs.Sum(lh => lh.KhoHangs.Sum(kh => kh.SoLuongTon ?? 0)) > 0)
            }).ToList();

            return new PagedResult<ThuocCustomerDto>
            {
                Items = thuocDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<PagedResult<ThuocCustomerDto>> SearchMedicinesAsync(string? searchTerm = null, int? idDanhMuc = null, decimal? minPrice = null, decimal? maxPrice = null, int pageNumber = 1, int pageSize = 12)
        {
            _logger.LogInformation($"Searching medicines");
            var pagedList = await _unitOfWork.ThuocRepository.GetPagedListAsync(pageNumber, pageSize, active: true, searchTerm: searchTerm, idDanhMuc: idDanhMuc);

            var filteredItems = pagedList.Items
                .Where(t => (!minPrice.HasValue || t.GiaBan >= minPrice) && (!maxPrice.HasValue || t.GiaBan <= maxPrice))
                .ToList();

            var thuocDtos = filteredItems.Select(thuoc => new ThuocCustomerDto
            {
                Id = thuoc.Id,
                TenThuoc = thuoc.TenThuoc,
                MoTa = thuoc.MoTa,
                GiaBan = thuoc.GiaBan,
                DonVi = thuoc.DonVi,
                HoatChat = thuoc.HoatChat,
                TenDanhMuc = thuoc.IddanhMucNavigation?.TenDanhMuc,
                SoLuongConLai = thuoc.LoHangs.Sum(lh => lh.KhoHangs.Sum(kh => kh.SoLuongTon ?? 0)),
                CoSan = (thuoc.LoHangs.Sum(lh => lh.KhoHangs.Sum(kh => kh.SoLuongTon ?? 0)) > 0)
            }).ToList();

            return new PagedResult<ThuocCustomerDto>
            {
                Items = thuocDtos,
                TotalCount = filteredItems.Count,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<ThuocDetailDto?> GetMedicineDetailAsync(int id)
        {
            _logger.LogInformation($"Getting medicine detail: {id}");
            var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(id);
            if (thuoc == null || thuoc.TrangThai == false)
                throw new NotFoundException($"Không tìm th?y thu?c v?i id: {id}");

            var tonKhoByBranch = thuoc.LoHangs
                .SelectMany(lh => lh.KhoHangs)
                .GroupBy(kh => kh.IdchiNhanhNavigation!)
                .Select(g => new ThuocTonKhoByBranchDto
                {
                    IdChiNhanh = g.Key.Id,
                    TenChiNhanh = g.Key.TenChiNhanh,
                    SoLuongTon = g.Sum(kh => kh.SoLuongTon),
                    CoSan = g.Sum(kh => kh.SoLuongTon) > 0
                }).ToList();

            return new ThuocDetailDto
            {
                Id = thuoc.Id,
                TenThuoc = thuoc.TenThuoc,
                MoTa = thuoc.MoTa,
                GiaBan = thuoc.GiaBan,
                DonVi = thuoc.DonVi,
                HoatChat = thuoc.HoatChat,
                TenDanhMuc = thuoc.IddanhMucNavigation?.TenDanhMuc,
                IdDanhMuc = thuoc.IddanhMuc,
                TonKhoTheoChiNhanh = tonKhoByBranch
            };
        }

        public async Task<IEnumerable<DanhMucDto>> GetCategoriesAsync()
        {
            _logger.LogInformation("Getting all categories");
            var danhMucs = await _unitOfWork.DanhMucRepository.GetAllAsync();
            return danhMucs
                .Where(d => d.TrangThai == true)
                .Select(d => new DanhMucDto
                {
                    Id = d.Id,
                    TenDanhMuc = d.TenDanhMuc,
                    MoTa = d.MoTa,
                    TrangThai = d.TrangThai
                })
                .ToList();
        }

        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(int customerId)
        {
            _logger.LogInformation($"Getting customer profile: {customerId}");
            var khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(customerId);
            if (khachHang == null)
                throw new NotFoundException($"Không tìm th?y khách hàng v?i id: {customerId}");

            var donHangs = await _unitOfWork.DonHangRepository.GetByKhachHangIdAsync(customerId);
            var tongGiaTriMua = donHangs.Sum(d => d.ThanhTien ?? 0);
            
            DateTime? lanMuaGanNhat = null;
            var donHangMuiNhat = donHangs.OrderByDescending(d => d.NgayTao).FirstOrDefault();
            if (donHangMuiNhat?.NgayTao.HasValue == true)
                lanMuaGanNhat = donHangMuiNhat.NgayTao.Value.ToDateTime(TimeOnly.MinValue);

            return new CustomerProfileDto
            {
                Id = khachHang.Id,
                TenKhachHang = khachHang.TenKhachHang,
                Sdt = khachHang.Sdt,
                DiemTichLuy = khachHang.DiemTichLuy,
                NgayDangKy = khachHang.NgayDangKy,
                TongDonHang = donHangs.Count(),
                TongGiaTriMua = tongGiaTriMua,
                LanMuaGanNhat = lanMuaGanNhat
            };
        }

        public async Task UpdateCustomerProfileAsync(int customerId, UpdateKhachHangDto dto)
        {
            _logger.LogInformation($"Updating customer profile: {customerId}");
            var khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(customerId);
            if (khachHang == null)
                throw new NotFoundException($"Không tìm th?y khách hàng v?i id: {customerId}");

            khachHang.TenKhachHang = dto.TenKhachHang ?? khachHang.TenKhachHang;
            khachHang.Sdt = dto.Sdt ?? khachHang.Sdt;
            await _unitOfWork.KhachHangRepository.UpdateAsync(khachHang);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<DonHangDto>> GetOrderHistoryAsync(int customerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Getting order history for customer: {customerId}");
            var khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(customerId);
            if (khachHang == null)
                throw new NotFoundException($"Không tìm th?y khách hàng v?i id: {customerId}");

            var pagedList = await _unitOfWork.DonHangRepository.GetPagedListAsync(pageNumber, pageSize, idChiNhanh: null, idKhachHang: customerId, tuNgay: null, denNgay: null);

            var dhDtos = pagedList.Items.Select(dh => new DonHangDto
            {
                Id = dh.Id,
                IdnguoiDung = dh.IdnguoiDung,
                IdkhachHang = dh.IdkhachHang,
                IdchiNhanh = dh.IdchiNhanh,
                IdphuongThucTt = dh.IdphuongThucTt,
                TongTien = dh.TongTien,
                TienGiamGia = dh.TienGiamGia,
                ThanhTien = dh.ThanhTien,
                NgayTao = dh.NgayTao,
                TenChiNhanh = dh.IdchiNhanhNavigation?.TenChiNhanh,
                TenPhuongThucTt = dh.IdphuongThucTtNavigation?.TenPhuongThuc
            }).ToList();

            return new PagedResult<DonHangDto>
            {
                Items = dhDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<DonHangDto?> GetOrderDetailAsync(int orderId)
        {
            _logger.LogInformation($"Getting order detail: {orderId}");
            var donHang = await _unitOfWork.DonHangRepository.GetByIdWithDetailsAsync(orderId);
            if (donHang == null)
                throw new NotFoundException($"Không tìm th?y ??n hàng v?i id: {orderId}");

            return new DonHangDto
            {
                Id = donHang.Id,
                IdnguoiDung = donHang.IdnguoiDung,
                IdkhachHang = donHang.IdkhachHang,
                IdchiNhanh = donHang.IdchiNhanh,
                IdphuongThucTt = donHang.IdphuongThucTt,
                TongTien = donHang.TongTien,
                TienGiamGia = donHang.TienGiamGia,
                ThanhTien = donHang.ThanhTien,
                NgayTao = donHang.NgayTao,
                TenNguoiDung = donHang.IdnguoiDungNavigation?.HoTen,
                TenKhachHang = donHang.IdkhachHangNavigation?.TenKhachHang,
                TenChiNhanh = donHang.IdchiNhanhNavigation?.TenChiNhanh,
                TenPhuongThucTt = donHang.IdphuongThucTtNavigation?.TenPhuongThuc,
                ChiTietDonHangs = donHang.ChiTietDonHangs.Select(ct => new ChiTietDonHangDto
                {
                    Id = ct.Id,
                    Idthuoc = ct.Idthuoc,
                    TenThuoc = ct.IdthuocNavigation?.TenThuoc,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            };
        }

        public async Task<ShipmentTrackingDto?> TrackShipmentAsync(int orderId)
        {
            _logger.LogInformation($"Tracking shipment for order: {orderId}");
            var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(orderId);
            if (donHang == null)
                throw new NotFoundException($"Không tìm th?y ??n hàng v?i id: {orderId}");

            var donGiaoHang = donHang.DonGiaoHangs.FirstOrDefault();
            if (donGiaoHang == null)
                throw new NotFoundException($"Không tìm th?y thông tin giao hàng cho ??n hàng: {orderId}");

            var lichSuCapNhat = new List<ShipmentHistoryDto>();
            if (donGiaoHang.NgayTao.HasValue)
                lichSuCapNhat.Add(new ShipmentHistoryDto { TrangThai = "PENDING", ThoiGian = donGiaoHang.NgayTao, MoTa = "??n hàng v?a ???c t?o" });
            if (donGiaoHang.NgayXacNhan.HasValue)
                lichSuCapNhat.Add(new ShipmentHistoryDto { TrangThai = "CONFIRMED", ThoiGian = donGiaoHang.NgayXacNhan, MoTa = "??n hàng ?ã ???c xác nh?n" });
            if (donGiaoHang.NgayLayHang.HasValue)
                lichSuCapNhat.Add(new ShipmentHistoryDto { TrangThai = "PICKING", ThoiGian = donGiaoHang.NgayLayHang, MoTa = "?ang chu?n b? hàng" });
            if (donGiaoHang.NgayBatDauGiao.HasValue)
                lichSuCapNhat.Add(new ShipmentHistoryDto { TrangThai = "SHIPPING", ThoiGian = donGiaoHang.NgayBatDauGiao, MoTa = "?ang giao hàng" });
            if (donGiaoHang.NgayGiaoThanhCong.HasValue)
                lichSuCapNhat.Add(new ShipmentHistoryDto { TrangThai = "DELIVERED", ThoiGian = donGiaoHang.NgayGiaoThanhCong, MoTa = "Giao hàng thành công" });

            return new ShipmentTrackingDto
            {
                Id = donGiaoHang.Id,
                IdDonHang = donGiaoHang.IddonHang,
                TrangThaiGiaoHang = donGiaoHang.TrangThaiGiaoHang,
                TenNguoiGiaoHang = donGiaoHang.IdnguoiGiaoHangNavigation?.HoTen,
                SdtNguoiGiaoHang = donGiaoHang.IdnguoiGiaoHangNavigation?.Sdt,
                DiaChiGiaoHang = donGiaoHang.DiaChiGiaoHang,
                TenNguoiNhan = donGiaoHang.TenNguoiNhan,
                PhiGiaoHang = donGiaoHang.PhiGiaoHang,
                LichSuCapNhat = lichSuCapNhat
            };
        }

        public async Task<PagedResult<LichSuDiemCustomerDto>> GetLoyaltyPointHistoryAsync(int customerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Getting loyalty point history for customer: {customerId}");
            var khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(customerId);
            if (khachHang == null)
                throw new NotFoundException($"Không tìm th?y khách hàng v?i id: {customerId}");

            var lichSuDiems = khachHang.LichSuDiems.OrderByDescending(l => l.NgayGiaoDich).ToList();
            var totalCount = lichSuDiems.Count();
            var skip = (pageNumber - 1) * pageSize;
            var items = lichSuDiems.Skip(skip).Take(pageSize).ToList();

            int diemConLaiTracker = khachHang.DiemTichLuy ?? 0;
            var dtos = new List<LichSuDiemCustomerDto>();

            foreach (var item in items.AsEnumerable().Reverse())
            {
                dtos.Add(new LichSuDiemCustomerDto
                {
                    Id = item.Id,
                    IdDonHang = item.IddonHang ?? 0,
                    DiemCong = item.DiemCong ?? 0,
                    DiemTru = item.DiemTru ?? 0,
                    DiemConLai = diemConLaiTracker,
                    NgayGiaoDich = item.NgayGiaoDich ?? DateOnly.FromDateTime(DateTime.Now),
                    LoaiGiaoDich = (item.DiemCong ?? 0) > 0 ? "Mua" : "S? d?ng",
                    GiaTriDonHang = item.IddonHangNavigation?.ThanhTien
                });
                diemConLaiTracker -= (item.DiemCong ?? 0) - (item.DiemTru ?? 0);
            }

            dtos = dtos.OrderByDescending(d => d.NgayGiaoDich).ToList();

            return new PagedResult<LichSuDiemCustomerDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<IEnumerable<ChiNhanhCustomerDto>> GetBranchesAsync()
        {
            _logger.LogInformation("Getting all active branches for customers");
            var pageResult = await _unitOfWork.ChiNhanhRepository.GetPagedListAsync(pageNumber: 1, pageSize: 1000, active: true, searchTerm: null);

            return pageResult.Items
                .Select(c => new ChiNhanhCustomerDto
                {
                    Id = c.Id,
                    TenChiNhanh = c.TenChiNhanh,
                    DiaChi = c.DiaChi,
                    IsActive = c.TrangThai == true,
                    SoLuongSanPhamCoSan = c.KhoHangs.Count(kh => (kh.SoLuongTon ?? 0) > 0)
                })
                .ToList();
        }

        public async Task<int?> GetStockByBranchAsync(int medicineId, int branchId)
        {
            _logger.LogInformation($"Getting stock for medicine {medicineId} at branch {branchId}");
            var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(medicineId);
            if (thuoc == null)
                throw new NotFoundException($"Không tìm th?y thu?c v?i id: {medicineId}");

            return thuoc.LoHangs
                .SelectMany(lh => lh.KhoHangs)
                .Where(kh => kh.IdchiNhanh == branchId)
                .Sum(kh => kh.SoLuongTon);
        }
    }
}
