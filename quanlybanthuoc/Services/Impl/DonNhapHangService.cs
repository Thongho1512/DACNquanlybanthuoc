using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonNhapHang;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class DonNhapHangService : IDonNhapHangService
    {
        private readonly ILogger<DonNhapHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DonNhapHangService(
            IUnitOfWork unitOfWork,
            ILogger<DonNhapHangService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DonNhapHangDto> CreateAsync(CreateDonNhapHangDto dto, int idNguoiNhan)
        {
            _logger.LogInformation("Creating new import order");

            // Validate chi nhánh
            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(dto.IdchiNhanh);
            if (chiNhanh == null || chiNhanh.TrangThai == false)
            {
                throw new NotFoundException("Chi nhánh không tồn tại hoặc không hoạt động.");
            }

            // Validate nhà cung cấp
            var nhaCungCap = await _unitOfWork.NhaCungCapRepository.GetByIdAsync(dto.IdnhaCungCap);
            if (nhaCungCap == null || nhaCungCap.TrangThai == false)
            {
                throw new NotFoundException("Nhà cung cấp không tồn tại.");
            }

            // Validate số đơn nhập không trùng
            var existing = await _unitOfWork.DonNhapHangRepository.GetBySoDonNhapAsync(dto.SoDonNhap);
            if (existing != null)
            {
                throw new BadRequestException("Số đơn nhập đã tồn tại.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                decimal tongTien = 0;
                var loHangList = new List<LoHang>();

                // Tạo đơn nhập hàng
                var donNhapHang = new DonNhapHang
                {
                    IdchiNhanh = dto.IdchiNhanh,
                    IdnhaCungCap = dto.IdnhaCungCap,
                    IdnguoiNhan = idNguoiNhan,
                    SoDonNhap = dto.SoDonNhap,
                    NgayNhap = dto.NgayNhap
                };

                await _unitOfWork.DonNhapHangRepository.CreateAsync(donNhapHang);
                await _unitOfWork.SaveChangesAsync();

                // Tạo các lô hàng
                foreach (var loHangDto in dto.LoHangs)
                {
                    // Validate thuốc
                    var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(loHangDto.Idthuoc);
                    if (thuoc == null || thuoc.TrangThai == false)
                    {
                        throw new NotFoundException($"Thuốc ID {loHangDto.Idthuoc} không tồn tại.");
                    }

                    // Validate ngày hết hạn
                    if (loHangDto.NgayHetHan <= loHangDto.NgaySanXuat)
                    {
                        throw new BadRequestException("Ngày hết hạn phải sau ngày sản xuất.");
                    }

                    if (loHangDto.NgayHetHan <= DateOnly.FromDateTime(DateTime.Now))
                    {
                        throw new BadRequestException("Không thể nhập thuốc đã hết hạn.");
                    }

                    var thanhTien = loHangDto.SoLuong * loHangDto.GiaNhap;
                    tongTien += thanhTien;

                    // Tạo lô hàng
                    var loHang = new LoHang
                    {
                        IddonNhapHang = donNhapHang.Id,
                        Idthuoc = loHangDto.Idthuoc,
                        SoLo = loHangDto.SoLo,
                        NgaySanXuat = loHangDto.NgaySanXuat,
                        NgayHetHan = loHangDto.NgayHetHan,
                        SoLuong = loHangDto.SoLuong,
                        GiaNhap = loHangDto.GiaNhap
                    };

                    await _unitOfWork.LoHangRepository.CreateAsync(loHang);
                    await _unitOfWork.SaveChangesAsync();

                    // Tạo hoặc cập nhật kho hàng
                    var khoHang = await _unitOfWork.KhoHangRepository
                        .GetByChiNhanhAndLoHangAsync(dto.IdchiNhanh, loHang.Id);

                    if (khoHang == null)
                    {
                        // Tạo mới kho hàng
                        khoHang = new KhoHang
                        {
                            IdchiNhanh = dto.IdchiNhanh,
                            IdloHang = loHang.Id,
                            TonKhoToiThieu = 10, // Mặc định
                            SoLuongTon = loHangDto.SoLuong,
                            NgayCapNhat = DateOnly.FromDateTime(DateTime.Now)
                        };

                        await _unitOfWork.KhoHangRepository.CreateAsync(khoHang);
                    }
                    else
                    {
                        // Cập nhật tồn kho
                        khoHang.SoLuongTon += loHangDto.SoLuong;
                        khoHang.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);
                        await _unitOfWork.KhoHangRepository.UpdateAsync(khoHang);
                    }

                    loHangList.Add(loHang);
                }

                // Cập nhật tổng tiền
                donNhapHang.TongTien = tongTien;
                await _unitOfWork.DonNhapHangRepository.UpdateAsync(donNhapHang);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Load lại với details
                var result = await GetByIdAsync(donNhapHang.Id);
                return result!;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating import order");
                throw;
            }
        }

        public async Task<DonNhapHangDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting import order by id: {id}");

            var donNhapHang = await _unitOfWork.DonNhapHangRepository.GetByIdWithDetailsAsync(id);
            if (donNhapHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn nhập hàng với id: {id}");
            }

            var result = new DonNhapHangDto
            {
                Id = donNhapHang.Id,
                IdchiNhanh = donNhapHang.IdchiNhanh,
                IdnhaCungCap = donNhapHang.IdnhaCungCap,
                IdnguoiNhan = donNhapHang.IdnguoiNhan,
                SoDonNhap = donNhapHang.SoDonNhap,
                NgayNhap = donNhapHang.NgayNhap,
                TongTien = donNhapHang.TongTien,
                TenChiNhanh = donNhapHang.IdchiNhanhNavigation?.TenChiNhanh,
                TenNhaCungCap = donNhapHang.IdnhaCungCapNavigation?.TenNhaCungCap,
                TenNguoiNhan = donNhapHang.IdnguoiNhanNavigation?.HoTen,
                LoHangs = donNhapHang.LoHangs.Select(lh => new LoHangItemDto
                {
                    Id = lh.Id,
                    Idthuoc = lh.Idthuoc ?? 0,
                    TenThuoc = lh.IdthuocNavigation?.TenThuoc,
                    SoLo = lh.SoLo,
                    NgaySanXuat = lh.NgaySanXuat ?? DateOnly.MinValue,
                    NgayHetHan = lh.NgayHetHan ?? DateOnly.MinValue,
                    SoLuong = lh.SoLuong ?? 0,
                    GiaNhap = lh.GiaNhap ?? 0,
                    ThanhTien = (lh.SoLuong ?? 0) * (lh.GiaNhap ?? 0)
                }).ToList()
            };

            return result;
        }

        public async Task<PagedResult<DonNhapHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNhaCungCap = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null)
        {
            _logger.LogInformation("Getting all import orders");

            var pagedList = await _unitOfWork.DonNhapHangRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                idChiNhanh,
                idNhaCungCap,
                tuNgay,
                denNgay);

            var dtoList = pagedList.Items.Select(dnh => new DonNhapHangDto
            {
                Id = dnh.Id,
                IdchiNhanh = dnh.IdchiNhanh,
                IdnhaCungCap = dnh.IdnhaCungCap,
                IdnguoiNhan = dnh.IdnguoiNhan,
                SoDonNhap = dnh.SoDonNhap,
                NgayNhap = dnh.NgayNhap,
                TongTien = dnh.TongTien,
                TenChiNhanh = dnh.IdchiNhanhNavigation?.TenChiNhanh,
                TenNhaCungCap = dnh.IdnhaCungCapNavigation?.TenNhaCungCap,
                TenNguoiNhan = dnh.IdnguoiNhanNavigation?.HoTen
            }).ToList();

            return new PagedResult<DonNhapHangDto>
            {
                Items = dtoList,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }
    }
}