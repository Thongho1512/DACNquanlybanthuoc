using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class ChiNhanhService : IChiNhanhService
    {
        private readonly ILogger<ChiNhanhService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChiNhanhService(IUnitOfWork unitOfWork, ILogger<ChiNhanhService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ChiNhanhDto> CreateAsync(CreateChiNhanhDto dto)
        {
            _logger.LogInformation("Creating new branch");

            var entity = _mapper.Map<ChiNhanh>(dto);
            entity.TrangThai = true;

            await _unitOfWork.ChiNhanhRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ChiNhanhDto>(entity);
            return result;
        }

        public async Task<PagedResult<ChiNhanhDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            _logger.LogInformation("Getting all branches");

            var pagedList = await _unitOfWork.ChiNhanhRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                active,
                searchTerm);

            var cnDtos = pagedList.Items.Select(cn => _mapper.Map<ChiNhanhDto>(cn)).ToList();

            return new PagedResult<ChiNhanhDto>
            {
                Items = cnDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<ChiNhanhDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting branch by id: {id}");

            var entity = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy chi nhánh với id: {id}");
            }

            var result = _mapper.Map<ChiNhanhDto>(entity);
            return result;
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft deleting branch with id: {id}");

            var entity = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy chi nhánh.");
            }

            await _unitOfWork.ChiNhanhRepository.SoftDeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateChiNhanhDto dto)
        {
            _logger.LogInformation($"Updating branch with id: {id}");

            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(id);
            if (chiNhanh == null)
            {
                throw new NotFoundException($"Không tìm thấy chi nhánh với id: {id}");
            }

            _mapper.Map(dto, chiNhanh);
            await _unitOfWork.ChiNhanhRepository.UpdateAsync(chiNhanh);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}