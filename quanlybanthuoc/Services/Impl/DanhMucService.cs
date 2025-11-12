using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class DanhMucService : IDanhMucService
    {
        private readonly ILogger<DanhMucService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DanhMucService(IUnitOfWork unitOfWork, ILogger<DanhMucService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DanhMucDto> CreateAsync(CreateDanhMucDto dto)
        {
            _logger.LogInformation("Creating new category");

            var existing = await _unitOfWork.DanhMucRepository.GetByTenDanhMucAsync(dto.TenDanhMuc!);
            if (existing != null)
            {
                throw new BadRequestException("Tên danh mục đã tồn tại.");
            }

            var entity = _mapper.Map<DanhMuc>(dto);
            await _unitOfWork.DanhMucRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<DanhMucDto>(entity);
            return result;
        }

        public async Task<IEnumerable<DanhMucDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all categories");

            var danhMucs = await _unitOfWork.DanhMucRepository.GetAllAsync();
            var result = danhMucs.Select(dm => _mapper.Map<DanhMucDto>(dm));

            return result;
        }

        public async Task<DanhMucDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting category by id: {id}");

            var entity = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {id}");
            }

            var result = _mapper.Map<DanhMucDto>(entity);
            return result;
        }

        public async Task UpdateAsync(int id, UpdateDanhMucDto dto)
        {
            _logger.LogInformation($"Updating category with id: {id}");

            var danhMuc = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (danhMuc == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {id}");
            }

            _mapper.Map(dto, danhMuc);
            await _unitOfWork.DanhMucRepository.UpdateAsync(danhMuc);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting category with id: {id}");

            var entity = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục.");
            }

            // Check if category has medicines
            var hasThuocs = entity.Thuocs.Any();
            if (hasThuocs)
            {
                throw new BadRequestException("Không thể xóa danh mục đang có thuốc.");
            }

            await _unitOfWork.DanhMucRepository.SoftDeleted(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}