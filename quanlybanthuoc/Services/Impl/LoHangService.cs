using AutoMapper;
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
    }
}