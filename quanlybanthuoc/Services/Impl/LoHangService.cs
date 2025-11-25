using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.LoHang;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class LoHangService : ILoHangService
    {
        private readonly ILogger<LoHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoHangService(
            IUnitOfWork unitOfWork,
            ILogger<LoHangService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<LoHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idThuoc = null,
            int? idChiNhanh = null,
            bool? sapHetHan = null,
            int? daysToExpire = 30)
        {
            _logger.LogInformation("Getting all batches with filters");

            var pagedList = await _unitOfWork.LoHangRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                idThuoc,
                idChiNhanh,
                sapHetHan,
                daysToExpire);

            var dtoList = pagedList.Items.Select(lh => new LoHangDto
            {
                Id = lh.Id,
                IddonNhapHang = lh.IddonNhapHang,
                Idthuoc = lh.Idthuoc,
                SoLo = lh.SoLo,
                NgaySanXuat = lh.NgaySanXuat,
                NgayHetHan = lh.NgayHetHan,
                SoLuong = lh.SoLuong,
                GiaNhap = lh.GiaNhap,
                TenThuoc = lh.IdthuocNavigation?.TenThuoc,
                SoDonNhap = lh.IddonNhapHangNavigation?.SoDonNhap
            }).ToList();

            return new PagedResult<LoHangDto>
            {
                Items = dtoList,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<LoHangDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting batch by id: {id}");

            var entity = await _unitOfWork.LoHangRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy lô hàng với id: {id}");
            }

            var result = _mapper.Map<LoHangDto>(entity);
            return result;
        }

        public async Task<IEnumerable<LoHangDto>> GetByThuocIdAsync(int thuocId)
        {
            _logger.LogInformation($"Getting batches by medicine id: {thuocId}");

            var loHangs = await _unitOfWork.LoHangRepository.GetByThuocIdAsync(thuocId);

            var result = loHangs.Select(lh => new LoHangDto
            {
                Id = lh.Id,
                IddonNhapHang = lh.IddonNhapHang,
                Idthuoc = lh.Idthuoc,
                SoLo = lh.SoLo,
                NgaySanXuat = lh.NgaySanXuat,
                NgayHetHan = lh.NgayHetHan,
                SoLuong = lh.SoLuong,
                GiaNhap = lh.GiaNhap,
                TenThuoc = lh.IdthuocNavigation?.TenThuoc,
                SoDonNhap = lh.IddonNhapHangNavigation?.SoDonNhap
            });

            return result;
        }

        public async Task<IEnumerable<LoHangDto>> GetLoHangSapHetHanAsync(int days, int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting batches expiring in {days} days");

            var pagedResult = await _unitOfWork.LoHangRepository.GetPagedListAsync(
                1,
                1000, // Get all expiring items
                null,
                idChiNhanh,
                true, // sapHetHan = true
                days);

            var result = pagedResult.Items.Select(lh => new LoHangDto
            {
                Id = lh.Id,
                IddonNhapHang = lh.IddonNhapHang,
                Idthuoc = lh.Idthuoc,
                SoLo = lh.SoLo,
                NgaySanXuat = lh.NgaySanXuat,
                NgayHetHan = lh.NgayHetHan,
                SoLuong = lh.SoLuong,
                GiaNhap = lh.GiaNhap,
                TenThuoc = lh.IdthuocNavigation?.TenThuoc,
                SoDonNhap = lh.IddonNhapHangNavigation?.SoDonNhap
            });

            return result;
        }

        public async Task UpdateAsync(int id, UpdateLoHangDto dto)
        {
            _logger.LogInformation($"Updating batch with id: {id}");

            var loHang = await _unitOfWork.LoHangRepository.GetByIdAsync(id);
            if (loHang == null)
            {
                throw new NotFoundException($"Không tìm thấy lô hàng với id: {id}");
            }

            // Validate ngày hết hạn
            if (dto.NgayHetHan.HasValue && dto.NgaySanXuat.HasValue)
            {
                if (dto.NgayHetHan.Value <= dto.NgaySanXuat.Value)
                {
                    throw new BadRequestException("Ngày hết hạn phải sau ngày sản xuất.");
                }
            }

            _mapper.Map(dto, loHang);

            await _unitOfWork.LoHangRepository.UpdateAsync(loHang);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<LoHangDto> CreateAsync(CreateLoHangDto dto, int idChiNhanh)
        {
            _logger.LogInformation("Creating new batch manually");

            // Validate chi nhánh
            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(idChiNhanh);
            if (chiNhanh == null || chiNhanh.TrangThai == false)
            {
                throw new NotFoundException("Chi nhánh không tồn tại hoặc không hoạt động.");
            }

            // Validate thuốc
            var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(dto.Idthuoc);
            if (thuoc == null || thuoc.TrangThai == false)
            {
                throw new NotFoundException("Thuốc không tồn tại.");
            }

            // Validate ngày hết hạn
            if (dto.NgayHetHan <= dto.NgaySanXuat)
            {
                throw new BadRequestException("Ngày hết hạn phải sau ngày sản xuất.");
            }

            if (dto.NgayHetHan <= DateOnly.FromDateTime(DateTime.Now))
            {
                throw new BadRequestException("Không thể nhập thuốc đã hết hạn.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Tạo lô hàng
                var loHang = new LoHang
                {
                    IddonNhapHang = null, // Lô hàng thủ công không có đơn nhập hàng
                    Idthuoc = dto.Idthuoc,
                    SoLo = dto.SoLo,
                    NgaySanXuat = dto.NgaySanXuat,
                    NgayHetHan = dto.NgayHetHan,
                    SoLuong = dto.SoLuong,
                    GiaNhap = dto.GiaNhap
                };

                await _unitOfWork.LoHangRepository.CreateAsync(loHang);
                await _unitOfWork.SaveChangesAsync();

                // Tạo hoặc cập nhật kho hàng
                var khoHang = await _unitOfWork.KhoHangRepository
                    .GetByChiNhanhAndLoHangAsync(idChiNhanh, loHang.Id);

                if (khoHang == null)
                {
                    khoHang = new KhoHang
                    {
                        IdchiNhanh = idChiNhanh,
                        IdloHang = loHang.Id,
                        TonKhoToiThieu = 10,
                        SoLuongTon = dto.SoLuong,
                        NgayCapNhat = DateOnly.FromDateTime(DateTime.Now)
                    };
                    await _unitOfWork.KhoHangRepository.CreateAsync(khoHang);
                }
                else
                {
                    await _unitOfWork.KhoHangRepository.CongTonKhoAsync(
                        idChiNhanh,
                        loHang.Id,
                        dto.SoLuong);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var result = _mapper.Map<LoHangDto>(loHang);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating batch");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting batch with id: {id}");

            var loHang = await _unitOfWork.LoHangRepository.GetByIdAsync(id);
            if (loHang == null)
            {
                throw new NotFoundException($"Không tìm thấy lô hàng với id: {id}");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Kiểm tra và trừ tồn kho trước khi xóa
                var khoHangs = await _unitOfWork.KhoHangRepository.GetByChiNhanhIdAsync(loHang.IddonNhapHangNavigation?.IdchiNhanh ?? 0);
                var khoHangOfLoHang = khoHangs.Where(kh => kh.IdloHang == id).ToList();

                foreach (var khoHang in khoHangOfLoHang)
                {
                    if (khoHang.SoLuongTon > 0)
                    {
                        throw new BadRequestException(
                            $"Không thể xóa lô hàng vì còn {khoHang.SoLuongTon} sản phẩm trong kho.");
                    }
                }

                // Xóa lô hàng
                _unitOfWork.LoHangRepository.DeleteAsync(loHang);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting batch");
                throw;
            }
        }
    }
}