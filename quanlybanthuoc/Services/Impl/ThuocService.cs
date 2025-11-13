using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class ThuocService : IThuocService
    {
        private readonly ILogger<ThuocService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ThuocService(IUnitOfWork unitOfWork, ILogger<ThuocService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ThuocDto> CreateAsync(CreateThuocDto dto)
        {
            _logger.LogInformation("Creating new medicine");

            var entity = _mapper.Map<Thuoc>(dto);
            entity.TrangThai = true;

            await _unitOfWork.ThuocRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ThuocDto>(entity);
            return result;
        }

        public async Task<PagedResult<ThuocDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null,
            int? idDanhMuc = null)
        {
            _logger.LogInformation("Getting all medicines with filters");

            var pagedList = await _unitOfWork.ThuocRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                active,
                searchTerm,
                idDanhMuc);

            var thuocDtos = pagedList.Items.Select(thuoc =>
            {
                var dto = _mapper.Map<ThuocDto>(thuoc);
                dto.TenDanhMuc = thuoc.IddanhMucNavigation?.TenDanhMuc;
                return dto;
            }).ToList();

            return new PagedResult<ThuocDto>
            {
                Items = thuocDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<ThuocDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting medicine by id: {id}");

            var entity = await _unitOfWork.ThuocRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy thuốc với id: {id}");
            }

            var result = _mapper.Map<ThuocDto>(entity);
            return result;
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft deleting medicine with id: {id}");

            var entity = await _unitOfWork.ThuocRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy thuốc.");
            }

            await _unitOfWork.ThuocRepository.SoftDeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateThuocDto dto)
        {
            _logger.LogInformation($"Updating medicine with id: {id}");

            var thuoc = await _unitOfWork.ThuocRepository.GetByIdAsync(id);
            if (thuoc == null)
            {
                throw new NotFoundException($"Không tìm thấy thuốc với id: {id}");
            }

            _mapper.Map(dto, thuoc);
            await _unitOfWork.ThuocRepository.UpdateAsync(thuoc);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ThuocDto>> GetThuocSapHetHanAsync(int days, int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting medicines expiring in {days} days");

            var thuocs = await _unitOfWork.ThuocRepository.GetThuocSapHetHanAsync(days, idChiNhanh);
            var result = thuocs.Select(t => _mapper.Map<ThuocDto>(t));

            return result;
        }

        public async Task<IEnumerable<ThuocDto>> GetThuocTonKhoThapAsync(int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting medicines with low stock");

            var thuocs = await _unitOfWork.ThuocRepository.GetThuocTonKhoThapAsync(idChiNhanh);
            var result = thuocs.Select(t => _mapper.Map<ThuocDto>(t));

            return result;
        }

        public async Task<PagedResult<ThuocDto>> GetByChiNhanhIdAsync(
    int idChiNhanh,
    int pageNumber,
    int pageSize,
    bool active,
    string? searchTerm = null,
    int? idDanhMuc = null)
        {
            _logger.LogInformation($"Getting medicines by branch id: {idChiNhanh}");

            // Validate chi nhánh tồn tại
            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(idChiNhanh);
            if (chiNhanh == null || chiNhanh.TrangThai == false)
            {
                throw new NotFoundException($"Không tìm thấy chi nhánh với id: {idChiNhanh}");
            }

            var pagedList = await _unitOfWork.ThuocRepository.GetByChiNhanhIdAsync(
                idChiNhanh,
                pageNumber,
                pageSize,
                active,
                searchTerm,
                idDanhMuc);

            var thuocDtos = pagedList.Items.Select(thuoc =>
            {
                var dto = _mapper.Map<ThuocDto>(thuoc);
                dto.TenDanhMuc = thuoc.IddanhMucNavigation?.TenDanhMuc;
                return dto;
            }).ToList();

            return new PagedResult<ThuocDto>
            {
                Items = thuocDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }
    }
}