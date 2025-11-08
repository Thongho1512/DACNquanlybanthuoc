// File: quanlybanthuoc/Services/Impl/DonHangService.cs (UPDATED VERSION)
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
            _logger.LogInformation("Creating new order with inventory deduction");

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
                // Tính toán tổng tiền
                decimal tongTien = 0;
                var chiTietList = new List<ChiTietDonHang>();
                var chiTietLoHangList = new List<ChiTietLoHang>();

                foreach (var item in dto.ChiTietDonHangs)
                {
                    // Validate thuốc
                    var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(item.Idthuoc);
                    if (thuoc == null || thuoc.TrangThai == false)
                    {
                        throw new NotFoundException($"Thuốc ID {item.Idthuoc} không tồn tại.");
                    }

                    var thanhTien = item.SoLuong * item.DonGia;
                    tongTien += thanhTien;

                    chiTietList.Add(new ChiTietDonHang
                    {
                        Idthuoc = item.Idthuoc,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        ThanhTien = thanhTien
                    });
                }

                // Tính tiền giảm giá từ điểm tích lũy
                decimal tienGiamGia = 0;
                int diemSuDung = 0;

                if (dto.DiemSuDung.HasValue && dto.DiemSuDung.Value > 0 && khachHang != null)
                {
                    if (khachHang.DiemTichLuy < dto.DiemSuDung.Value)
                    {
                        throw new BadRequestException("Số điểm sử dụng vượt quá điểm tích lũy hiện có.");
                    }

                    // Quy đổi: 100 điểm = 10,000 VNĐ
                    diemSuDung = dto.DiemSuDung.Value;
                    tienGiamGia = (diemSuDung / 100m) * 10000;

                    // Giảm giá không vượt quá tổng tiền
                    if (tienGiamGia > tongTien)
                    {
                        tienGiamGia = tongTien;
                    }
                }

                decimal thanhTien = tongTien - tienGiamGia;

                // Tạo đơn hàng
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

                // Tạo chi tiết đơn hàng và TRỪ TỒN KHO
                foreach (var chiTiet in chiTietList)
                {
                    chiTiet.IddonHang = donHang.Id;

                    // ✅ LOGIC TRỪ KHO THEO FIFO (First In, First Out)
                    int soLuongCanTru = chiTiet.SoLuong ?? 0;

                    // Lấy danh sách lô hàng của thuốc này, sắp xếp theo ngày hết hạn (FIFO)
                    var loHangs = await _unitOfWork.LoHangRepository.GetByThuocIdAsync(chiTiet.Idthuoc ?? 0);

                    foreach (var loHang in loHangs)
                    {
                        if (soLuongCanTru <= 0) break;

                        // Lấy kho hàng của lô này tại chi nhánh
                        var khoHang = await _unitOfWork.KhoHangRepository
                            .GetByChiNhanhAndLoHangAsync(dto.IdchiNhanh, loHang.Id);

                        if (khoHang == null || khoHang.SoLuongTon <= 0)
                        {
                            continue; // Bỏ qua lô hàng không có tồn kho
                        }

                        // Tính số lượng trừ từ lô này
                        int soLuongTruLoNay = Math.Min(soLuongCanTru, khoHang.SoLuongTon ?? 0);

                        // Trừ tồn kho
                        await _unitOfWork.KhoHangRepository.TruTonKhoAsync(
                            dto.IdchiNhanh,
                            loHang.Id,
                            soLuongTruLoNay);

                        // Lưu chi tiết lô hàng
                        var chiTietLoHang = new ChiTietLoHang
                        {
                            IdkhoHang = khoHang.Id,
                            SoLuong = soLuongTruLoNay
                        };
                        chiTietLoHangList.Add(chiTietLoHang);

                        soLuongCanTru -= soLuongTruLoNay;
                    }

                    // Nếu vẫn còn số lượng cần trừ => không đủ hàng
                    if (soLuongCanTru > 0)
                    {
                        throw new BadRequestException(
                            $"Không đủ tồn kho cho thuốc '{chiTiet.IdthuocNavigation?.TenThuoc}'. " +
                            $"Còn thiếu: {soLuongCanTru} {chiTiet.IdthuocNavigation?.DonVi}");
                    }
                }

                await _unitOfWork.ChiTietDonHangRepository.CreateRangeAsync(chiTietList);
                await _unitOfWork.SaveChangesAsync();

                // Lưu chi tiết lô hàng
                foreach (var chiTietLoHang in chiTietLoHangList)
                {
                    // Link với ChiTietDonHang tương ứng
                    var chiTietDh = chiTietList.FirstOrDefault(ct =>
                        ct.Idthuoc == chiTietLoHang.IdkhoHangNavigation?.IdloHangNavigation?.Idthuoc);

                    if (chiTietDh != null)
                    {
                        chiTietLoHang.IdchiTietDh = chiTietDh.Id;
                    }
                }

                // Note: Bạn cần tạo repository cho ChiTietLoHang nếu muốn lưu
                // Hiện tại có thể bỏ qua phần này nếu không cần tracking chi tiết lô

                // Cập nhật điểm tích lũy
                if (khachHang != null)
                {
                    // Trừ điểm đã sử dụng
                    int diemTru = diemSuDung;

                    // Cộng điểm mới: 100,000 VNĐ = 10 điểm
                    int diemCong = (int)(thanhTien / 100000) * 10;

                    await _khachHangService.UpdateDiemTichLuyAsync(khachHang.Id, diemCong, diemTru);

                    // Lưu lịch sử điểm
                    var lichSuDiem = new LichSuDiem
                    {
                        IdkhachHang = khachHang.Id,
                        IddonHang = donHang.Id,
                        DiemCong = diemCong,
                        DiemTru = diemTru,
                        NgayGiaoDich = DateOnly.FromDateTime(DateTime.Now)
                    };

                    await _unitOfWork.LichSuDiemRepository.CreateAsync(lichSuDiem);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                // Load lại với details
                var result = await GetByIdAsync(donHang.Id);
                return result!;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating order with inventory deduction");
                throw;
            }
        }

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

            // TODO: Khi xóa đơn hàng, cần HOÀN LẠI TỒN KHO
            // Để tránh phức tạp, có thể chỉ soft delete hoặc đánh dấu "đã hủy"
            // Không implement hard delete cho giao dịch tài chính

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