using AutoMapper;
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
    }
}