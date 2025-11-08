using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.VaiTro;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class VaiTroService : IVaiTroService
    {
        private readonly ILogger<VaiTroService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VaiTroService(IUnitOfWork unitOfWork, ILogger<VaiTroService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<VaiTroDto> CreateAsync(CreateVaiTroDto dto)
        {
            _logger.LogInformation("Creating new role");

            // Check if role name already exists
            var existing = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync(dto.TenVaiTro!);
            if (existing != null)
            {
                throw new BadRequestException("Tên vai trò đã tồn tại.");
            }

            var entity = _mapper.Map<VaiTro>(dto);
            entity.TrangThai = true;

            await _unitOfWork.VaiTroRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<VaiTroDto>(entity);
            return result;
        }

        public async Task<PagedResult<VaiTroDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            _logger.LogInformation("Getting all roles with pagination");

            var pagedList = await _unitOfWork.VaiTroRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                active,
                searchTerm);

            var vaiTroDtos = pagedList.Items.Select(vt => _mapper.Map<VaiTroDto>(vt)).ToList();

            return new PagedResult<VaiTroDto>
            {
                Items = vaiTroDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<IEnumerable<VaiTroDto>> GetAllActiveAsync()
        {
            _logger.LogInformation("Getting all active roles");

            var vaiTros = await _unitOfWork.VaiTroRepository.GetAllAsync();
            var result = vaiTros.Select(vt => _mapper.Map<VaiTroDto>(vt));

            return result;
        }

        public async Task<VaiTroDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting role by id: {id}");

            var entity = await _unitOfWork.VaiTroRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy vai trò với id: {id}");
            }

            var result = _mapper.Map<VaiTroDto>(entity);
            return result;
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft deleting role with id: {id}");

            var entity = await _unitOfWork.VaiTroRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy vai trò.");
            }

            // Check if role is being used by users
            if (entity.NguoiDungs != null && entity.NguoiDungs.Any())
            {
                throw new BadRequestException("Không thể xóa vai trò đang được sử dụng bởi người dùng.");
            }

            // Prevent deleting system roles
            if (entity.TenVaiTro == "ADMIN" || entity.TenVaiTro == "USER")
            {
                throw new BadRequestException("Không thể xóa vai trò hệ thống.");
            }

            await _unitOfWork.VaiTroRepository.SoftDeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateVaiTroDto dto)
        {
            _logger.LogInformation($"Updating role with id: {id}");

            var vaiTro = await _unitOfWork.VaiTroRepository.GetByIdAsync(id);
            if (vaiTro == null)
            {
                throw new NotFoundException($"Không tìm thấy vai trò với id: {id}");
            }

            // Prevent updating system roles' names
            if ((vaiTro.TenVaiTro == "ADMIN" || vaiTro.TenVaiTro == "USER") &&
                dto.TenVaiTro != vaiTro.TenVaiTro)
            {
                throw new BadRequestException("Không thể thay đổi tên vai trò hệ thống.");
            }

            // Check if new name already exists
            if (dto.TenVaiTro != vaiTro.TenVaiTro)
            {
                var existing = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync(dto.TenVaiTro!);
                if (existing != null)
                {
                    throw new BadRequestException("Tên vai trò đã tồn tại.");
                }
            }

            _mapper.Map(dto, vaiTro);
            await _unitOfWork.VaiTroRepository.UpdateAsync(vaiTro);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}