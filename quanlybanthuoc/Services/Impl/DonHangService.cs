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
        // NGHIỆP VỤ ĐIỂM THƯỞNG - CẤU HÌNH TẠI ĐÂY
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

            // ================================================================
            // BƯỚC 1: VALIDATE DỮ LIỆU ĐẦU VÀO
            // ================================================================

            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(dto.IdchiNhanh);
            if (chiNhanh == null || chiNhanh.TrangThai == false)
            {
                throw new NotFoundException("Chi nhánh không tồn tại hoặc không hoạt động.");
            }

            KhachHang? khachHang = null;
            if (dto.IdkhachHang.HasValue)
            {
                khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(dto.IdkhachHang.Value);
                if (khachHang == null || khachHang.TrangThai == false)
                {
                    throw new NotFoundException("Khách hàng không tồn tại.");
                }

                _logger.LogInformation($"👤 Khách hàng: {khachHang.TenKhachHang}");
                _logger.LogInformation($"💎 Điểm hiện có: {khachHang.DiemTichLuy ?? 0} điểm");
            }

            var phuongThucTt = await _unitOfWork.PhuongThucThanhToanRepository.GetByIdAsync(dto.IdphuongThucTt);
            if (phuongThucTt == null || phuongThucTt.TrangThai == false)
            {
                throw new NotFoundException("Phương thức thanh toán không hợp lệ.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ================================================================
                // BƯỚC 2: TÍNH TỔNG TIỀN VÀ TẠO CHI TIẾT ĐƠN HÀNG
                // ================================================================

                decimal tongTien = 0;
                var chiTietList = new List<ChiTietDonHang>();

                _logger.LogInformation("");
                _logger.LogInformation(" CHI TIẾT SẢN PHẨM:");
                _logger.LogInformation("─────────────────────────────────────────────────────");

                foreach (var item in dto.ChiTietDonHangs)
                {
                    var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(item.Idthuoc);
                    if (thuoc == null || thuoc.TrangThai == false)
                    {
                        throw new NotFoundException($"Thuốc ID {item.Idthuoc} không tồn tại.");
                    }

                    var thanhTienItem = item.SoLuong * item.DonGia;
                    tongTien += thanhTienItem;

                    _logger.LogInformation($"  • {thuoc.TenThuoc}: {item.SoLuong} x {item.DonGia:N0} = {thanhTienItem:N0} VNĐ");

                    chiTietList.Add(new ChiTietDonHang
                    {
                        Idthuoc = item.Idthuoc,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        ThanhTien = thanhTienItem
                    });
                }

                _logger.LogInformation("─────────────────────────────────────────────────────");
                _logger.LogInformation($"💰 TỔNG TIỀN: {tongTien:N0} VNĐ");
                _logger.LogInformation("");

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

                    //  HỆ THỐNG TỰ ĐỘNG CHỌN SỐ ĐIỂM TỐI ƯU
                    diemSuDung = Math.Min(diemKhaDungCuaKhachHang, diemToiDaCoTheSuDung);

                    // Tính tiền giảm giá từ điểm
                    tienGiamGia = diemSuDung * TY_LE_QUYDO_DIEM_SANG_TIEN;

                }
                else if (khachHang != null)
                {
                    _logger.LogInformation($" Khách hàng có {diemKhaDungCuaKhachHang} điểm");
                    _logger.LogInformation($"   (Cần tối thiểu {SO_DIEM_TOI_THIEU_SU_DUNG} điểm để sử dụng)");
                    _logger.LogInformation("");
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

                _logger.LogInformation($" Đã tạo đơn hàng ID: {donHang.Id}");

                // ================================================================
                // BƯỚC 5: XỬ LÝ CHI TIẾT ĐƠN HÀNG VÀ TRỪ TỒN KHO (FEFO)
                // ================================================================

                _logger.LogInformation("");
                _logger.LogInformation("📦 XỬ LÝ TỒN KHO (FEFO - First Expired, First Out):");
                _logger.LogInformation("─────────────────────────────────────────────────────");

                foreach (var chiTiet in chiTietList)
                {
                    chiTiet.IddonHang = donHang.Id;

                    int soLuongCanTru = chiTiet.SoLuong ?? 0;
                    var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(chiTiet.Idthuoc ?? 0);

                    //  Lấy danh sách lô hàng đã được sắp xếp theo FEFO
                    // (Hết hạn sớm nhất → muộn nhất)
                    var loHangs = await _unitOfWork.LoHangRepository.GetByThuocIdAsync(chiTiet.Idthuoc ?? 0);

                    if (!loHangs.Any())
                    {
                        throw new BadRequestException(
                            $" Không có lô hàng nào cho thuốc '{thuoc?.TenThuoc}'");
                    }

                    _logger.LogInformation($"  🔍 Xử lý thuốc: {thuoc?.TenThuoc} (Cần: {soLuongCanTru} {thuoc?.DonVi})");

                    //  Lọc chỉ lấy các lô có tồn tại chi nhánh này
                    var loHangsTaiChiNhanh = loHangs
                        .Where(lh => lh.KhoHangs.Any(kh =>
                            kh.IdchiNhanh == dto.IdchiNhanh &&
                            kh.SoLuongTon > 0))
                        .ToList();

                    if (!loHangsTaiChiNhanh.Any())
                    {
                        throw new BadRequestException(
                            $" Không có tồn kho cho thuốc '{thuoc?.TenThuoc}' tại chi nhánh này");
                    }

                    foreach (var loHang in loHangsTaiChiNhanh)
                    {
                        if (soLuongCanTru <= 0) break; // Đã đủ số lượng

                        var khoHang = await _unitOfWork.KhoHangRepository
                            .GetByChiNhanhAndLoHangAsync(dto.IdchiNhanh, loHang.Id);

                        if (khoHang == null || khoHang.SoLuongTon <= 0)
                            continue; // Skip lô này nếu không có tồn

                        // Tính số lượng cần trừ từ lô này
                        int soLuongTruLoNay = Math.Min(soLuongCanTru, khoHang.SoLuongTon ?? 0);

                        // Trừ tồn kho
                        await _unitOfWork.KhoHangRepository.TruTonKhoAsync(
                            dto.IdchiNhanh,
                            loHang.Id,
                            soLuongTruLoNay);

                        // Tính số ngày còn lại đến hết hạn
                        int soNgayConLai = 0;
                        if (loHang.NgayHetHan.HasValue)
                        {
                            soNgayConLai = (loHang.NgayHetHan.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days;
                        }

                        _logger.LogInformation(
                            $"     Lô {loHang.SoLo} (HSD: {loHang.NgayHetHan?.ToString("dd/MM/yyyy") ?? "N/A"}, còn {soNgayConLai} ngày): " +
                            $"Trừ {soLuongTruLoNay} {thuoc?.DonVi}");

                        soLuongCanTru -= soLuongTruLoNay;
                    }

                    // Kiểm tra nếu vẫn còn thiếu hàng
                    if (soLuongCanTru > 0)
                    {
                        throw new BadRequestException(
                            $" Không đủ tồn kho cho thuốc '{thuoc?.TenThuoc}'. " +
                            $"Còn thiếu: {soLuongCanTru} {thuoc?.DonVi}");
                    }

                    _logger.LogInformation($"    ✔️ Hoàn tất xử lý thuốc: {thuoc?.TenThuoc}");
                }

                await _unitOfWork.ChiTietDonHangRepository.CreateRangeAsync(chiTietList);
                await _unitOfWork.SaveChangesAsync();

                // ================================================================
                // BƯỚC 6: CẬP NHẬT ĐIỂM TÍCH LŨY CHO KHÁCH HÀNG
                // ================================================================

                if (khachHang != null)
                {
                    // Tính điểm được cộng từ đơn hàng này
                    int diemCong = (int)(thanhTien / TY_LE_TICH_DIEM);

                    // Cập nhật điểm: Cộng điểm mới, Trừ điểm đã sử dụng
                    await _khachHangService.UpdateDiemTichLuyAsync(
                        khachHang.Id,
                        diemCong,
                        diemSuDung
                    );

                    int diemMoi = (khachHang.DiemTichLuy ?? 0) + diemCong - diemSuDung;


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

                // Load lại với details để trả về
                var result = await GetByIdAsync(donHang.Id);
                return result!;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, " LỖI KHI TẠO ĐƠN HÀNG");
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
            _logger.LogInformation($" Bắt đầu xóa đơn hàng ID: {id}");
            _logger.LogInformation("========================================");

            // ================================================================
            // BƯỚC 1: LẤY THÔNG TIN ĐƠN HÀNG VỚI CHI TIẾT ĐẦY ĐỦ
            // ================================================================
            var donHang = await _unitOfWork.DonHangRepository.GetByIdWithDetailsAsync(id);
            if (donHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn hàng với ID: {id}");
            }

            _logger.LogInformation($" Thông tin đơn hàng:");
            _logger.LogInformation($"   - Chi nhánh: {donHang.IdchiNhanhNavigation?.TenChiNhanh}");
            _logger.LogInformation($"   - Khách hàng: {donHang.IdkhachHangNavigation?.TenKhachHang ?? "Khách lẻ"}");
            _logger.LogInformation($"   - Ngày tạo: {donHang.NgayTao}");
            _logger.LogInformation($"   - Tổng tiền: {donHang.TongTien:N0} VNĐ");
            _logger.LogInformation($"   - Giảm giá: {donHang.TienGiamGia:N0} VNĐ");
            _logger.LogInformation($"   - Thành tiền: {donHang.ThanhTien:N0} VNĐ");
            _logger.LogInformation($"   - Số sản phẩm: {donHang.ChiTietDonHangs.Count}");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ================================================================
                // BƯỚC 2: ⭐ XỬ LÝ HOÀN TRẢ ĐIỂM TÍCH LŨY TRƯỚC (QUAN TRỌNG!)
                // ================================================================
                LichSuDiem? lichSuDiem = null;

                if (donHang.IdkhachHang.HasValue)
                {
                    _logger.LogInformation("");
                    _logger.LogInformation(" XỬ LÝ ĐIỂM TÍCH LŨY:");
                    _logger.LogInformation("─────────────────────────────────────");

                    lichSuDiem = await _unitOfWork.LichSuDiemRepository
                        .GetByDonHangIdAsync(id);

                    if (lichSuDiem != null)
                    {
                        int diemDaCong = lichSuDiem.DiemCong ?? 0;
                        int diemDaTru = lichSuDiem.DiemTru ?? 0;

                        _logger.LogInformation($"   Điểm đã cộng: {diemDaCong}");
                        _logger.LogInformation($"   Điểm đã trừ: {diemDaTru}");

                        // Hoàn trả điểm cho khách hàng
                        var khachHang = donHang.IdkhachHangNavigation;

                        if (khachHang != null)
                        {
                            int diemCu = khachHang.DiemTichLuy ?? 0;

                            // Cập nhật điểm: - điểm đã cộng + điểm đã trừ
                            await _khachHangService.UpdateDiemTichLuyAsync(
                                khachHang.Id,
                                diemDaTru,      // Hoàn lại điểm đã sử dụng
                                diemDaCong      // Trừ điểm đã được cộng
                            );

                            int diemMoi = diemCu - diemDaCong + diemDaTru;

                            _logger.LogInformation($"   Điểm cũ: {diemCu}");
                            _logger.LogInformation($"   Điểm mới: {diemMoi}");
                            _logger.LogInformation($"   (= {diemCu} - {diemDaCong} + {diemDaTru})");
                        }

                        // ⭐ XÓA LỊCH SỬ ĐIỂM NGAY SAU KHI CẬP NHẬT
                        await _unitOfWork.LichSuDiemRepository.DeleteAsync(lichSuDiem);
                        await _unitOfWork.SaveChangesAsync(); // Lưu ngay để tránh conflict
                        _logger.LogInformation("    Đã xóa lịch sử điểm");
                    }
                    else
                    {
                        _logger.LogInformation("    Không có lịch sử điểm cần xử lý");
                    }
                }

                // ================================================================
                // BƯỚC 3: HOÀN TRẢ TỒN KHO (REVERSE FEFO)
                // ================================================================
                _logger.LogInformation("");
                _logger.LogInformation(" HOÀN TRẢ TỒN KHO:");
                _logger.LogInformation("─────────────────────────────────────");

                if (!donHang.ChiTietDonHangs.Any())
                {
                    _logger.LogWarning(" Đơn hàng không có chi tiết sản phẩm");
                }
                else
                {
                    foreach (var chiTiet in donHang.ChiTietDonHangs)
                    {
                        var thuoc = chiTiet.IdthuocNavigation;
                        int soLuongCanHoanTra = chiTiet.SoLuong ?? 0;

                        _logger.LogInformation($"    Hoàn trả: {thuoc?.TenThuoc}");
                        _logger.LogInformation($"      Số lượng: {soLuongCanHoanTra} {thuoc?.DonVi}");

                        // Lấy danh sách lô hàng theo FEFO (hết hạn sớm nhất trước)
                        var loHangs = await _unitOfWork.LoHangRepository
                            .GetByThuocIdAsync(chiTiet.Idthuoc ?? 0);

                        var loHangsTaiChiNhanh = loHangs
                            .Where(lh => lh.KhoHangs.Any(kh =>
                                kh.IdchiNhanh == donHang.IdchiNhanh))
                            .ToList();

                        if (!loHangsTaiChiNhanh.Any())
                        {
                            _logger.LogWarning($"       Không tìm thấy lô hàng tại chi nhánh");
                            continue;
                        }

                        // Hoàn trả theo thứ tự FEFO (lô hết hạn sớm nhất trước)
                        foreach (var loHang in loHangsTaiChiNhanh)
                        {
                            if (soLuongCanHoanTra <= 0) break;

                            // Hoàn trả vào kho
                            try
                            {
                                await _unitOfWork.KhoHangRepository.CongTonKhoAsync(
                                    donHang.IdchiNhanh ?? 0,
                                    loHang.Id,
                                    soLuongCanHoanTra
                                );

                                _logger.LogInformation(
                                    $"       Lô {loHang.SoLo}: +{soLuongCanHoanTra} {thuoc?.DonVi}");

                                soLuongCanHoanTra = 0; // Hoàn trả xong
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(
                                    $"      ❌ Lỗi hoàn trả lô {loHang.SoLo}: {ex.Message}");
                            }
                        }

                        if (soLuongCanHoanTra > 0)
                        {
                            _logger.LogWarning(
                                $"       Còn thiếu {soLuongCanHoanTra} {thuoc?.DonVi} chưa hoàn trả");
                        }
                    }
                }

                // ================================================================
                // BƯỚC 4: XÓA CHI TIẾT ĐƠN HÀNG
                // ================================================================
                _logger.LogInformation("");
                _logger.LogInformation(" XÓA CHI TIẾT ĐƠN HÀNG:");
                _logger.LogInformation("─────────────────────────────────────");

                if (donHang.ChiTietDonHangs.Any())
                {
                    await _unitOfWork.ChiTietDonHangRepository
                        .DeleteRangeAsync(donHang.ChiTietDonHangs);

                    _logger.LogInformation($"    Đã xóa {donHang.ChiTietDonHangs.Count} chi tiết đơn hàng");
                }

                // ================================================================
                // BƯỚC 5: XÓA ĐƠN HÀNG (SAU CÙNG)
                // ================================================================
                _logger.LogInformation("");
                _logger.LogInformation(" XÓA ĐƠN HÀNG:");
                _logger.LogInformation("─────────────────────────────────────");

                await _unitOfWork.DonHangRepository.DeleteAsync(donHang);
                _logger.LogInformation("    Đã xóa đơn hàng khỏi database");

                // ================================================================
                // BƯỚC 6: LƯU THAY ĐỔI VÀ COMMIT TRANSACTION
                // ================================================================
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("");
                _logger.LogInformation("✅ XÓA ĐƠN HÀNG THÀNH CÔNG");
                _logger.LogInformation("========================================");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError($"❌ LỖI: {ex.Message}");
                throw new Exception($"Lỗi khi xóa đơn hàng: {ex.Message}", ex);
            }
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