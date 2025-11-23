using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhoHang;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class KhoHangService : IKhoHangService
    {
        private readonly ILogger<KhoHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public KhoHangService(
            IUnitOfWork unitOfWork,
            ILogger<KhoHangService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<KhoHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            bool? tonKhoThap = null)
        {
            _logger.LogInformation("Getting all warehouse stocks");

            var pagedList = await _unitOfWork.KhoHangRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                idChiNhanh,
                tonKhoThap);

            var dtoList = pagedList.Items.Select(kh => new KhoHangDto
            {
                Id = kh.Id,
                IdchiNhanh = kh.IdchiNhanh,
                IdloHang = kh.IdloHang,
                TonKhoToiThieu = kh.TonKhoToiThieu,
                SoLuongTon = kh.SoLuongTon,
                NgayCapNhat = kh.NgayCapNhat,
                TenChiNhanh = kh.IdchiNhanhNavigation?.TenChiNhanh,
                TenThuoc = kh.IdloHangNavigation?.IdthuocNavigation?.TenThuoc,
                SoLo = kh.IdloHangNavigation?.SoLo,
                NgayHetHan = kh.IdloHangNavigation?.NgayHetHan
            }).ToList();

            return new PagedResult<KhoHangDto>
            {
                Items = dtoList,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<KhoHangDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting warehouse stock by id: {id}");

            var entity = await _unitOfWork.KhoHangRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy kho hàng với id: {id}");
            }

            var result = _mapper.Map<KhoHangDto>(entity);
            return result;
        }

        public async Task<IEnumerable<KhoHangDto>> GetTonKhoThapAsync(int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting low stock items");

            var items = await _unitOfWork.KhoHangRepository.GetTonKhoThapAsync(idChiNhanh);

            var result = items.Select(kh => new KhoHangDto
            {
                Id = kh.Id,
                IdchiNhanh = kh.IdchiNhanh,
                IdloHang = kh.IdloHang,
                TonKhoToiThieu = kh.TonKhoToiThieu,
                SoLuongTon = kh.SoLuongTon,
                NgayCapNhat = kh.NgayCapNhat,
                TenChiNhanh = kh.IdchiNhanhNavigation?.TenChiNhanh,
                TenThuoc = kh.IdloHangNavigation?.IdthuocNavigation?.TenThuoc,
                SoLo = kh.IdloHangNavigation?.SoLo,
                NgayHetHan = kh.IdloHangNavigation?.NgayHetHan
            });

            return result;
        }

        public async Task UpdateAsync(int id, UpdateKhoHangDto dto)
        {
            _logger.LogInformation($"Updating warehouse stock with id: {id}");

            var khoHang = await _unitOfWork.KhoHangRepository.GetByIdAsync(id);
            if (khoHang == null)
            {
                throw new NotFoundException($"Không tìm thấy kho hàng với id: {id}");
            }

            if (dto.TonKhoToiThieu.HasValue)
            {
                khoHang.TonKhoToiThieu = dto.TonKhoToiThieu.Value;
            }

            if (dto.SoLuongTon.HasValue)
            {
                khoHang.SoLuongTon = dto.SoLuongTon.Value;
            }

            khoHang.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.KhoHangRepository.UpdateAsync(khoHang);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<KhoHangDto> CreateAsync(CreateKhoHangDto dto)
        {
            _logger.LogInformation("Creating new warehouse stock");

            // Validate chi nhánh
            var chiNhanh = await _unitOfWork.ChiNhanhRepository.GetByIdAsync(dto.IdchiNhanh);
            if (chiNhanh == null || chiNhanh.TrangThai == false)
            {
                throw new NotFoundException("Chi nhánh không tồn tại hoặc không hoạt động.");
            }

            // Validate lô hàng
            var loHang = await _unitOfWork.LoHangRepository.GetByIdAsync(dto.IdloHang);
            if (loHang == null)
            {
                throw new NotFoundException("Lô hàng không tồn tại.");
            }

            // Kiểm tra đã tồn tại chưa
            var existing = await _unitOfWork.KhoHangRepository
                .GetByChiNhanhAndLoHangAsync(dto.IdchiNhanh, dto.IdloHang);
            if (existing != null)
            {
                throw new BadRequestException("Kho hàng cho lô hàng này tại chi nhánh đã tồn tại.");
            }

            var entity = new KhoHang
            {
                IdchiNhanh = dto.IdchiNhanh,
                IdloHang = dto.IdloHang,
                TonKhoToiThieu = dto.TonKhoToiThieu,
                SoLuongTon = dto.SoLuongTon,
                NgayCapNhat = DateOnly.FromDateTime(DateTime.Now)
            };

            await _unitOfWork.KhoHangRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<KhoHangDto>(entity);
            return result;
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting warehouse stock with id: {id}");

            var entity = await _unitOfWork.KhoHangRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy kho hàng với id: {id}");
            }

            // Kiểm tra còn tồn kho không
            if (entity.SoLuongTon > 0)
            {
                throw new BadRequestException(
                    $"Không thể xóa kho hàng vì còn {entity.SoLuongTon} sản phẩm trong kho.");
            }

            _unitOfWork.KhoHangRepository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}