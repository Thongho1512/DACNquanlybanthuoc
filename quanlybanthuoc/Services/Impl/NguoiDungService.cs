using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Helpers;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly ILogger<NguoiDungService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        

        public NguoiDungService(IUnitOfWork unitOfWork, ILogger<NguoiDungService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<NguoiDungDto> createAsync(CreateNguoiDungDto dto)
        {
            _logger.LogInformation("Create new user");
            var entity = _mapper.Map<NguoiDung>(dto);
            entity.NgayTao = DateOnly.FromDateTime(DateTime.Now);
            entity.TrangThai = true;
            entity.MatKhau = PasswordHelper.HashPassword(dto.MatKhau!);
            await _unitOfWork.NguoiDungRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<NguoiDungDto>(entity);
            return result;
        }

        public async Task<PagedResult<NguoiDungDto>> GetAllAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null)
        {
            var pagedListNguoiDungDtos = await _unitOfWork.NguoiDungRepository.GetPagedListAsync(pageNumber, pageSize, active, searchTerm);

            var NguoiDungDtosItems = pagedListNguoiDungDtos.Items.
                Select(nguoiDung => _mapper.Map<NguoiDungDto>(nguoiDung)).ToList();

            return new PagedResult<NguoiDungDto>
            {
                Items = NguoiDungDtosItems,
                TotalCount = pagedListNguoiDungDtos.TotalCount,
                PageNumber = pagedListNguoiDungDtos.PageNumber,
                PageSize = pagedListNguoiDungDtos.PageSize,
            };
        }

        public async Task<NguoiDungDto?> getByIdAsync(int id)
        {
            _logger.LogInformation($"Get user by id: {id}");
            var entity = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }
            var result = _mapper.Map<NguoiDungDto>(entity);
            return result;
        }


        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft delete by id: {id}");
            var entity = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);

        }

        public async Task updateAsync(int id, UpdateNguoiDungDto dto)
        {
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }
            _mapper.Map(dto, nguoiDung);
            await _unitOfWork.NguoiDungRepository.UpdateAsync(nguoiDung);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
