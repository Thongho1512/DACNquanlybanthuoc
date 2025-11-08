using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NhaCungCap;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class NhaCungCapService : INhaCungCapService
    {
        private readonly ILogger<NhaCungCapService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NhaCungCapService(IUnitOfWork unitOfWork, ILogger<NhaCungCapService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<NhaCungCapDto> CreateAsync(CreateNhaCungCapDto dto)
        {
            _logger.LogInformation("Creating new supplier");

            var entity = _mapper.Map<NhaCungCap>(dto);
            entity.TrangThai = true;

            await _unitOfWork.NhaCungCapRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<NhaCungCapDto>(entity);
            return result;
        }

        public async Task<PagedResult<NhaCungCapDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            _logger.LogInformation("Getting all suppliers");

            var pagedList = await _unitOfWork.NhaCungCapRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                active,
                searchTerm);

            var nccDtos = pagedList.Items.Select(ncc => _mapper.Map<NhaCungCapDto>(ncc)).ToList();

            return new PagedResult<NhaCungCapDto>
            {
                Items = nccDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<NhaCungCapDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting supplier by id: {id}");

            var entity = await _unitOfWork.NhaCungCapRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy nhà cung cấp với id: {id}");
            }

            var result = _mapper.Map<NhaCungCapDto>(entity);
            return result;
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft deleting supplier with id: {id}");

            var entity = await _unitOfWork.NhaCungCapRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy nhà cung cấp.");
            }

            await _unitOfWork.NhaCungCapRepository.SoftDeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateNhaCungCapDto dto)
        {
            _logger.LogInformation($"Updating supplier with id: {id}");

            var ncc = await _unitOfWork.NhaCungCapRepository.GetByIdAsync(id);
            if (ncc == null)
            {
                throw new NotFoundException($"Không tìm thấy nhà cung cấp với id: {id}");
            }

            _mapper.Map(dto, ncc);
            await _unitOfWork.NhaCungCapRepository.UpdateAsync(ncc);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}