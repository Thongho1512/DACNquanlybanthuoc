using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.PhuongThucThanhToan;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class PhuongThucThanhToanService : IPhuongThucThanhToanService
    {
        private readonly ILogger<PhuongThucThanhToanService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhuongThucThanhToanService(
            IUnitOfWork unitOfWork,
            ILogger<PhuongThucThanhToanService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PhuongThucThanhToanDto> CreateAsync(CreatePhuongThucThanhToanDto dto)
        {
            _logger.LogInformation("Creating new payment method");

            var entity = _mapper.Map<PhuongThucThanhToan>(dto);
            entity.TrangThai = true;

            await _unitOfWork.PhuongThucThanhToanRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<PhuongThucThanhToanDto>(entity);
            return result;
        }

        public async Task<IEnumerable<PhuongThucThanhToanDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all payment methods");

            var entities = await _unitOfWork.PhuongThucThanhToanRepository.GetAllAsync();
            var result = entities.Select(e => _mapper.Map<PhuongThucThanhToanDto>(e));

            return result;
        }

        public async Task<PhuongThucThanhToanDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting payment method by id: {id}");

            var entity = await _unitOfWork.PhuongThucThanhToanRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy phương thức thanh toán với id: {id}");
            }

            var result = _mapper.Map<PhuongThucThanhToanDto>(entity);
            return result;
        }

        public async Task UpdateAsync(int id, UpdatePhuongThucThanhToanDto dto)
        {
            _logger.LogInformation($"Updating payment method with id: {id}");

            var entity = await _unitOfWork.PhuongThucThanhToanRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy phương thức thanh toán với id: {id}");
            }

            _mapper.Map(dto, entity);
            await _unitOfWork.PhuongThucThanhToanRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting payment method with id: {id}");

            var entity = await _unitOfWork.PhuongThucThanhToanRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy phương thức thanh toán.");
            }

            entity.TrangThai = false;
            await _unitOfWork.PhuongThucThanhToanRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}