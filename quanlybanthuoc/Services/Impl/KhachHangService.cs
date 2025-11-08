using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class KhachHangService : IKhachHangService
    {
        private readonly ILogger<KhachHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public KhachHangService(IUnitOfWork unitOfWork, ILogger<KhachHangService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<KhachHangDto> CreateAsync(CreateKhachHangDto dto)
        {
            _logger.LogInformation("Creating new customer");

            // Check if customer with same phone already exists
            var existing = await _unitOfWork.KhachHangRepository.GetBySdtAsync(dto.Sdt!);
            if (existing != null)
            {
                throw new BadRequestException("Số điện thoại đã được đăng ký.");
            }

            var entity = _mapper.Map<KhachHang>(dto);
            entity.TrangThai = true;
            entity.DiemTichLuy = 0;
            entity.NgayDangKy = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.KhachHangRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<KhachHangDto>(entity);
            return result;
        }

        public async Task<PagedResult<KhachHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            _logger.LogInformation("Getting all customers");

            var pagedList = await _unitOfWork.KhachHangRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                active,
                searchTerm);

            var khDtos = pagedList.Items.Select(kh => _mapper.Map<KhachHangDto>(kh)).ToList();

            return new PagedResult<KhachHangDto>
            {
                Items = khDtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<KhachHangDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting customer by id: {id}");

            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy khách hàng với id: {id}");
            }

            var result = _mapper.Map<KhachHangDto>(entity);
            return result;
        }

        public async Task<KhachHangDto?> GetBySdtAsync(string sdt)
        {
            _logger.LogInformation($"Getting customer by phone: {sdt}");

            var entity = await _unitOfWork.KhachHangRepository.GetBySdtAsync(sdt);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy khách hàng với số điện thoại: {sdt}");
            }

            var result = _mapper.Map<KhachHangDto>(entity);
            return result;
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft deleting customer with id: {id}");

            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy khách hàng.");
            }

            await _unitOfWork.KhachHangRepository.SoftDeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateKhachHangDto dto)
        {
            _logger.LogInformation($"Updating customer with id: {id}");

            var khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(id);
            if (khachHang == null)
            {
                throw new NotFoundException($"Không tìm thấy khách hàng với id: {id}");
            }

            _mapper.Map(dto, khachHang);
            await _unitOfWork.KhachHangRepository.UpdateAsync(khachHang);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateDiemTichLuyAsync(int khachHangId, int diemCong, int diemTru)
        {
            _logger.LogInformation($"Updating loyalty points for customer: {khachHangId}");

            var khachHang = await _unitOfWork.KhachHangRepository.GetByIdAsync(khachHangId);
            if (khachHang == null)
            {
                throw new NotFoundException("Không tìm thấy khách hàng.");
            }

            khachHang.DiemTichLuy = (khachHang.DiemTichLuy ?? 0) + diemCong - diemTru;

            if (khachHang.DiemTichLuy < 0)
            {
                khachHang.DiemTichLuy = 0;
            }

            await _unitOfWork.KhachHangRepository.UpdateAsync(khachHang);
            await _unitOfWork.SaveChangesAsync();

            return khachHang.DiemTichLuy.Value;
        }
    }
}