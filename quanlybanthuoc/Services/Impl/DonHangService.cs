// File: quanlybanthuoc/Services/Impl/DonHangService.cs (HOÀN CHỈNH)
using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class DonHangService : IDonHangService
    {
        private readonly ILogger<DonHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IKhachHangService _khachHangService;

        // ====================================================================
        // NGHIỆP VỤ ĐIỂM THƯỞNG - CÓ THỂ CHỈNH SỬA
        // ====================================================================
        private const decimal TY_LE_QUYDO_DIEM_SANG_TIEN = 1000m;  // 1 điểm = 1,000 VNĐ
        private const decimal TY_LE_TICH_DIEM = 10000m;            // 10,000 VNĐ = 1 điểm
        private const int SO_DIEM_TOI_THIEU_SU_DUNG = 10;         // Tối thiểu 10 điểm mới được dùng
        private const decimal TY_LE_GIAM_GIA_TOI_DA = 0.5m;       // Giảm tối đa 50% giá trị đơn hàng
        // ====================================================================

        public DonHangService(
            IUnitOfWork unitOfWork,
            ILogger<DonHangService> logger,
            IMapper mapper,
            IKhachHangService khachHangService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _khachHangService = khachHangService;
        }

        public async Task<DonHangDto> CreateAsync(CreateDonHangDto dto, int idNguoiDung)
        {
            _logger.LogInformation("=== BẮT ĐẦU TẠO ĐỚN HÀNG VỚI TỰ ĐỘNG SỬ DỤNG ĐIỂM ===");

            // ================================================================
            // BƯỚC 1: VALIDATE DỮ LIỆU ĐẦU VÀO
            // ================================================================

            // Validate chi nhánh
            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(dto.IdchiNhanh);
            if (chiNhanh == null || chiNhanh.TrangThai == false)
            {
                throw new NotFoundException("Chi nhánh không tồn tại hoặc không hoạt động.");
            }

            // Validate khách hàng (nếu có)
            KhachHang? khachHang = null;
            if (dto.IdkhachHang.HasValue)
            {
                khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(dto.IdkhachHang.Value);
                if (khachHang == null || khachHang.TrangThai == false)
                {
                    throw new NotFoundException("Khách hàng không tồn tại.");
                }

                _logger.LogInformation($"Khách hàng: {khachHang.TenKhachHang} - Điểm hiện tại: {khachHang.DiemTichLuy ?? 0}");
            }

            // Validate phương thức thanh toán
            var phuongThucTt = await _unitOfWork.PhuongThucThanhToanRepository.GetByIdAsync(dto.IdphuongThucTt);
            if (phuongThucTt == null || phuongThucTt.TrangThai == false)
            {
                throw new NotFoundException("Phương thức thanh toán không hợp lệ.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ================================================================
                // BƯỚC 2: TÍNH TOÁN TỔNG TIỀN VÀ TẠO CHI TIẾT ĐƠN HÀNG
                // ================================================================

                decimal tongTien = 0;
                var chiTietList = new List<ChiTietDonHang>();

                foreach (var item in dto.ChiTietDonHangs)
                {
                    // Validate thuốc
                    var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(item.Idthuoc);
                    if (thuoc == null || thuoc.TrangThai == false)
                    {
                        throw new NotFoundException($"Thuốc ID {item.Idthuoc} không tồn tại.");
                    }

                    var thanhTienItem = item.SoLuong * item.DonGia;
                    tongTien += thanhTienItem;

                    chiTietList.Add(new ChiTietDonHang
                    {
                        Idthuoc = item.Idthuoc,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        ThanhTien = thanhTienItem
                    });
                }

                _logger.LogInformation($"Tổng tiền đơn hàng: {tongTien:N0} VNĐ");

                // ================================================================
                // BƯỚC 3: TỰ ĐỘNG TÍNH TOÁN VÀ SỬ DỤNG ĐIỂM TÍCH LŨY
                // ================================================================

                decimal tienGiamGia = 0;
                int diemSuDung = 0;
                int diemKhaDungCuaKhachHang = khachHang?.DiemTichLuy ?? 0;

                if (khachHang != null && diemKhaDungCuaKhachHang >= SO_DIEM_TOI_THIEU_SU_DUNG)
                {
                    // Tính số tiền giảm giá tối đa (50% giá trị đơn hàng)
                    decimal tienGiamGiaToiDa = tongTien * TY_LE_GIAM_GIA_TOI_DA;

                    // Tính số điểm tối đa có thể sử dụng dựa trên giới hạn giảm giá
                    int diemToiDaCoTheSuDung = (int)(tienGiamGiaToiDa / TY_LE_QUYDO_DIEM_SANG_TIEN);

                    // Số điểm thực tế sử dụng = MIN(điểm khách hàng có, điểm tối đa được dùng)
                    diemSuDung = Math.Min(diemKhaDungCuaKhachHang, diemToiDaCoTheSuDung);

                    // Tính tiền giảm giá từ điểm
                    tienGiamGia = diemSuDung * TY_LE_QUYDO_DIEM_SANG_TIEN;

                    _logger.LogInformation($"╔═══════════════════════════════════════════════════════╗");
                    _logger.LogInformation($"║          THÔNG TIN SỬ DỤNG ĐIỂM TÍCH LŨY            ║");
                    _logger.LogInformation($"╠═══════════════════════════════════════════════════════╣");
                    _logger.LogInformation($"║ Điểm khả dụng:            {diemKhaDungCuaKhachHang,10} điểm          ║");
                    _logger.LogInformation($"║ Điểm sử dụng:             {diemSuDung,10} điểm          ║");
                    _logger.LogInformation($"║ Tiền giảm giá:            {tienGiamGia,10:N0} VNĐ        ║");
                    _logger.LogInformation($"║ Tỷ lệ giảm:               {(tienGiamGia / tongTien) * 100,10:F1}%           ║");
                    _logger.LogInformation($"╚═══════════════════════════════════════════════════════╝");
                }
                else if (khachHang != null)
                {
                    _logger.LogInformation($"⚠️  Khách hàng có {diemKhaDungCuaKhachHang} điểm (< {SO_DIEM_TOI_THIEU_SU_DUNG} điểm tối thiểu)");
                }

                decimal thanhTien = tongTien - tienGiamGia;

                // ================================================================
                // BƯỚC 4: TẠO ĐƠN HÀNG
                // ================================================================

                var donHang = new DonHang
                {
                    IdnguoiDung = idNguoiDung,
                    IdkhachHang = dto.IdkhachHang,
                    IdchiNhanh = dto.IdchiNhanh,
                    IdphuongThucTt = dto.IdphuongThucTt,
                    TongTien = tongTien,
                    TienGiamGia = tienGiamGia,
                    ThanhTien = thanhTien,
                    NgayTao = DateOnly.FromDateTime(DateTime.Now)
                };

                await _unitOfWork.DonHangRepository.CreateAsync(donHang);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"✅ Đã tạo đơn hàng ID: {donHang.Id}");

                // ================================================================
                // BƯỚC 5: TẠO CHI TIẾT ĐƠN HÀNG VÀ TRỪ TỒN KHO (FIFO)
                // ================================================================

                foreach (var chiTiet in chiTietList)
                {
                    chiTiet.IddonHang = donHang.Id;

                    int soLuongCanTru = chiTiet.SoLuong ?? 0;
                    var loHangs = await _unitOfWork.LoHangRepository.GetByThuocIdAsync(chiTiet.Idthuoc ?? 0);

                    foreach (var loHang in loHangs)
                    {
                        if (soLuongCanTru <= 0) break;

                        var khoHang = await _unitOfWork.KhoHangRepository
                            .GetByChiNhanhAndLoHangAsync(dto.IdchiNhanh, loHang.Id);

                        if (khoHang == null || khoHang.SoLuongTon <= 0)
                            continue;

                        int soLuongTruLoNay = Math.Min(soLuongCanTru, khoHang.SoLuongTon ?? 0);

                        await _unitOfWork.KhoHangRepository.TruTonKhoAsync(
                            dto.IdchiNhanh,
                            loHang.Id,
                            soLuongTruLoNay);

                        _logger.LogInformation($"  → Trừ {soLuongTruLoNay} {chiTiet.IdthuocNavigation?.DonVi} từ lô {loHang.SoLo}");

                        soLuongCanTru -= soLuongTruLoNay;
                    }

                    if (soLuongCanTru > 0)
                    {
                        throw new BadRequestException(
                            $"Không đủ tồn kho cho thuốc '{chiTiet.IdthuocNavigation?.TenThuoc}'. " +
                            $"Còn thiếu: {soLuongCanTru} {chiTiet.IdthuocNavigation?.DonVi}");
                    }
                }

                await _unitOfWork.ChiTietDonHangRepository.CreateRangeAsync(chiTietList);
                await _unitOfWork.SaveChangesAsync();

                // ================================================================
                // BƯỚC 6: CẬP NHẬT ĐIỂM TÍCH LŨY CHO KHÁCH HÀNG
                // ================================================================

                if (khachHang != null)
                {
                    // Tính điểm được cộng từ đơn hàng này
                    // Công thức: 10,000 VNĐ = 1 điểm (tính trên thành tiền sau giảm giá)
                    int diemCong = (int)(thanhTien / TY_LE_TICH_DIEM);

                    // Cập nhật điểm: Cộng điểm mới, Trừ điểm đã sử dụng
                    await _khachHangService.UpdateDiemTichLuyAsync(
                        khachHang.Id,
                        diemCong,      // Điểm được cộng
                        diemSuDung     // Điểm đã sử dụng
                    );

                    // Tính điểm mới sau giao dịch
                    int diemMoi = (khachHang.DiemTichLuy ?? 0) + diemCong - diemSuDung;

                    _logger.LogInformation($"╔═══════════════════════════════════════════════════════╗");
                    _logger.LogInformation($"║          CẬP NHẬT ĐIỂM TÍCH LŨY THÀNH CÔNG          ║");
                    _logger.LogInformation($"╠═══════════════════════════════════════════════════════╣");
                    _logger.LogInformation($"║ Điểm ban đầu:             {khachHang.DiemTichLuy ?? 0,10} điểm          ║");
                    _logger.LogInformation($"║ Điểm sử dụng:            -{diemSuDung,10} điểm          ║");
                    _logger.LogInformation($"║ Điểm được cộng:          +{diemCong,10} điểm          ║");
                    _logger.LogInformation($"║ Điểm sau giao dịch:       {diemMoi,10} điểm          ║");
                    _logger.LogInformation($"╚═══════════════════════════════════════════════════════╝");

                    // Lưu lịch sử điểm
                    var lichSuDiem = new LichSuDiem
                    {
                        IdkhachHang = khachHang.Id,
                        IddonHang = donHang.Id,
                        DiemCong = diemCong,
                        DiemTru = diemSuDung,
                        NgayGiaoDich = DateOnly.FromDateTime(DateTime.Now)
                    };

                    await _unitOfWork.LichSuDiemRepository.CreateAsync(lichSuDiem);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("=== HOÀN TẤT TẠO ĐƠN HÀNG THÀNH CÔNG ===");

                // Load lại với details để trả về
                var result = await GetByIdAsync(donHang.Id);
                return result!;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "❌ LỖI KHI TẠO ĐƠN HÀNG");
                throw;
            }
        }

        // ================================================================
        // CÁC PHƯƠNG THỨC HỖ TRỢ KHÁC (GIỮ NGUYÊN)
        // ================================================================

        public async Task<DonHangDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting order by id: {id}");

            var donHang = await _unitOfWork.DonHangRepository.GetByIdWithDetailsAsync(id);
            if (donHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn hàng với id: {id}");
            }

            var result = new DonHangDto
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

            return result;
        }

        public async Task<PagedResult<DonHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idKhachHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null)
        {
            _logger.LogInformation("Getting all orders");

            var pagedList = await _unitOfWork.DonHangRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                idChiNhanh,
                idKhachHang,
                tuNgay,
                denNgay);

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
                TenNguoiDung = dh.IdnguoiDungNavigation?.HoTen,
                TenKhachHang = dh.IdkhachHangNavigation?.TenKhachHang,
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

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting order with id: {id}");

            var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(id);
            if (donHang == null)
            {
                throw new NotFoundException("Không tìm thấy đơn hàng.");
            }

            // Không cho phép xóa đơn hàng (chỉ hủy hoặc hoàn trả)
            throw new BadRequestException("Không thể xóa đơn hàng. Vui lòng liên hệ quản trị viên.");
        }

        public async Task<IEnumerable<DonHangDto>> GetByKhachHangIdAsync(int khachHangId)
        {
            _logger.LogInformation($"Getting orders by customer id: {khachHangId}");

            var donHangs = await _unitOfWork.DonHangRepository.GetByKhachHangIdAsync(khachHangId);

            var result = donHangs.Select(dh => new DonHangDto
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
                ChiTietDonHangs = dh.ChiTietDonHangs.Select(ct => new ChiTietDonHangDto
                {
                    Id = ct.Id,
                    Idthuoc = ct.Idthuoc,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            });

            return result;
        }
    }
}