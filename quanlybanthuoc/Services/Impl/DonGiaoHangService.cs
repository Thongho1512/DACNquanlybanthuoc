using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonGiaoHang;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class DonGiaoHangService : IDonGiaoHangService
    {
        private readonly ILogger<DonGiaoHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ShopDbContext _dbContext;

        public DonGiaoHangService(
            IUnitOfWork unitOfWork,
            ILogger<DonGiaoHangService> logger,
            IMapper mapper,
            ShopDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<DonGiaoHangDto> CreateAsync(CreateDonGiaoHangDto dto)
        {
            _logger.LogInformation($"Creating delivery order for order {dto.IddonHang}");

            // Validate đơn hàng tồn tại và chưa có đơn giao hàng
            var donHang = await _unitOfWork.DonHangRepository.GetByIdAsync(dto.IddonHang);
            if (donHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn hàng với ID: {dto.IddonHang}");
            }

            // Kiểm tra đơn hàng đã thanh toán chưa
            if (donHang.TrangThaiThanhToan != "PAID" && donHang.TrangThaiThanhToan != "PAID_ON_DELIVERY")
            {
                throw new BadRequestException("Chỉ có thể tạo đơn giao hàng cho đơn hàng đã thanh toán hoặc thanh toán khi nhận hàng.");
            }

            // Kiểm tra đã có đơn giao hàng chưa
            var existingDeliveries = await _unitOfWork.DonGiaoHangRepository.GetByDonHangIdAsync(dto.IddonHang);
            if (existingDeliveries.Any(dg => dg.TrangThaiGiaoHang != "HUY"))
            {
                throw new BadRequestException("Đơn hàng này đã có đơn giao hàng đang hoạt động.");
            }

            var donGiaoHang = new DonGiaoHang
            {
                IddonHang = dto.IddonHang,
                DiaChiGiaoHang = dto.DiaChiGiaoHang,
                SoDienThoaiNguoiNhan = dto.SoDienThoaiNguoiNhan,
                TenNguoiNhan = dto.TenNguoiNhan,
                PhiGiaoHang = dto.PhiGiaoHang ?? 0,
                TrangThaiGiaoHang = "CHO_XAC_NHAN",
                NgayTao = DateTime.Now,
                GhiChu = dto.GhiChu
            };

            await _unitOfWork.DonGiaoHangRepository.CreateAsync(donGiaoHang);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Created delivery order ID: {donGiaoHang.Id}");

            var result = await GetByIdAsync(donGiaoHang.Id);
            return result!;
        }

        public async Task<DonGiaoHangDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting delivery order by id: {id}");

            var donGiaoHang = await _unitOfWork.DonGiaoHangRepository.GetByIdWithDetailsAsync(id);
            if (donGiaoHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn giao hàng với ID: {id}");
            }

            var dto = _mapper.Map<DonGiaoHangDto>(donGiaoHang);
            dto.TenNguoiGiaoHang = donGiaoHang.IdnguoiGiaoHangNavigation?.HoTen;
            dto.SdtNguoiGiaoHang = donGiaoHang.IdnguoiGiaoHangNavigation?.Sdt;

            return dto;
        }

        public async Task<PagedResult<DonGiaoHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNguoiGiaoHang = null,
            string? trangThaiGiaoHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null)
        {
            _logger.LogInformation("Getting all delivery orders with filters");

            var pagedList = await _unitOfWork.DonGiaoHangRepository.GetPagedListAsync(
                pageNumber,
                pageSize,
                idChiNhanh,
                idNguoiGiaoHang,
                trangThaiGiaoHang,
                tuNgay,
                denNgay);

            var dtos = pagedList.Items.Select(dg => new DonGiaoHangDto
            {
                Id = dg.Id,
                IddonHang = dg.IddonHang,
                IdnguoiGiaoHang = dg.IdnguoiGiaoHang,
                TrangThaiGiaoHang = dg.TrangThaiGiaoHang,
                DiaChiGiaoHang = dg.DiaChiGiaoHang,
                SoDienThoaiNguoiNhan = dg.SoDienThoaiNguoiNhan,
                TenNguoiNhan = dg.TenNguoiNhan,
                PhiGiaoHang = dg.PhiGiaoHang,
                NgayTao = dg.NgayTao,
                NgayXacNhan = dg.NgayXacNhan,
                NgayLayHang = dg.NgayLayHang,
                NgayBatDauGiao = dg.NgayBatDauGiao,
                NgayGiaoThanhCong = dg.NgayGiaoThanhCong,
                GhiChu = dg.GhiChu,
                LyDoHuy = dg.LyDoHuy,
                TenNguoiGiaoHang = dg.IdnguoiGiaoHangNavigation?.HoTen,
                SdtNguoiGiaoHang = dg.IdnguoiGiaoHangNavigation?.Sdt
            }).ToList();

            return new PagedResult<DonGiaoHangDto>
            {
                Items = dtos,
                TotalCount = pagedList.TotalCount,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
            };
        }

        public async Task<DonGiaoHangDto> AssignDeliveryPersonAsync(int id, AssignDeliveryPersonDto dto)
        {
            _logger.LogInformation($"Assigning delivery person {dto.IdnguoiGiaoHang} to delivery order {id}");

            var donGiaoHang = await _unitOfWork.DonGiaoHangRepository.GetByIdAsync(id);
            if (donGiaoHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn giao hàng với ID: {id}");
            }

            // Validate người giao hàng tồn tại và đang hoạt động
            var nguoiGiaoHang = await _dbContext.NguoiGiaoHangs
                .FirstOrDefaultAsync(ng => ng.Id == dto.IdnguoiGiaoHang);
            
            if (nguoiGiaoHang == null || nguoiGiaoHang.TrangThai != true)
            {
                throw new NotFoundException($"Không tìm thấy người giao hàng với ID: {dto.IdnguoiGiaoHang} hoặc người giao hàng không hoạt động.");
            }

            // Chỉ có thể phân công khi đơn ở trạng thái CHO_XAC_NHAN
            if (donGiaoHang.TrangThaiGiaoHang != "CHO_XAC_NHAN")
            {
                throw new BadRequestException($"Không thể phân công người giao hàng. Đơn đang ở trạng thái: {donGiaoHang.TrangThaiGiaoHang}");
            }

            donGiaoHang.IdnguoiGiaoHang = dto.IdnguoiGiaoHang;
            donGiaoHang.TrangThaiGiaoHang = "DA_PHAN_CONG";
            donGiaoHang.NgayXacNhan = DateTime.Now;

            await _unitOfWork.DonGiaoHangRepository.UpdateAsync(donGiaoHang);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Assigned delivery person {dto.IdnguoiGiaoHang} to delivery order {id}");

            return await GetByIdAsync(id);
        }

        public async Task<DonGiaoHangDto> UpdateStatusAsync(int id, UpdateDeliveryStatusDto dto)
        {
            _logger.LogInformation($"Updating delivery status to {dto.TrangThaiGiaoHang} for delivery order {id}");

            var donGiaoHang = await _unitOfWork.DonGiaoHangRepository.GetByIdAsync(id);
            if (donGiaoHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn giao hàng với ID: {id}");
            }

            // Validate trạng thái hợp lệ
            var validStatuses = new[] { "CHO_XAC_NHAN", "DA_PHAN_CONG", "DANG_CHUAN_BI", "DANG_GIAO", "DA_GIAO", "HUY" };
            if (!validStatuses.Contains(dto.TrangThaiGiaoHang))
            {
                throw new BadRequestException($"Trạng thái không hợp lệ: {dto.TrangThaiGiaoHang}");
            }

            // Cập nhật trạng thái và các ngày tương ứng
            donGiaoHang.TrangThaiGiaoHang = dto.TrangThaiGiaoHang;
            donGiaoHang.GhiChu = dto.GhiChu;

            switch (dto.TrangThaiGiaoHang)
            {
                case "DANG_CHUAN_BI":
                    if (!donGiaoHang.NgayLayHang.HasValue)
                        donGiaoHang.NgayLayHang = DateTime.Now;
                    break;
                case "DANG_GIAO":
                    if (!donGiaoHang.NgayBatDauGiao.HasValue)
                        donGiaoHang.NgayBatDauGiao = DateTime.Now;
                    break;
                case "DA_GIAO":
                    donGiaoHang.NgayGiaoThanhCong = DateTime.Now;
                    break;
            }

            await _unitOfWork.DonGiaoHangRepository.UpdateAsync(donGiaoHang);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Updated delivery status to {dto.TrangThaiGiaoHang} for delivery order {id}");

            return await GetByIdAsync(id);
        }

        public async Task<DonGiaoHangDto> CancelAsync(int id, CancelDeliveryDto dto)
        {
            _logger.LogInformation($"Cancelling delivery order {id}");

            var donGiaoHang = await _unitOfWork.DonGiaoHangRepository.GetByIdAsync(id);
            if (donGiaoHang == null)
            {
                throw new NotFoundException($"Không tìm thấy đơn giao hàng với ID: {id}");
            }

            // Chỉ có thể hủy khi chưa giao thành công
            if (donGiaoHang.TrangThaiGiaoHang == "DA_GIAO")
            {
                throw new BadRequestException("Không thể hủy đơn giao hàng đã giao thành công.");
            }

            donGiaoHang.TrangThaiGiaoHang = "HUY";
            donGiaoHang.LyDoHuy = dto.LyDoHuy;

            await _unitOfWork.DonGiaoHangRepository.UpdateAsync(donGiaoHang);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Cancelled delivery order {id}");

            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<DonGiaoHangDto>> GetByNguoiGiaoHangIdAsync(int idNguoiGiaoHang)
        {
            _logger.LogInformation($"Getting delivery orders for delivery person {idNguoiGiaoHang}");

            var donGiaoHangs = await _unitOfWork.DonGiaoHangRepository.GetByNguoiGiaoHangIdAsync(idNguoiGiaoHang);

            return donGiaoHangs.Select(dg => new DonGiaoHangDto
            {
                Id = dg.Id,
                IddonHang = dg.IddonHang,
                IdnguoiGiaoHang = dg.IdnguoiGiaoHang,
                TrangThaiGiaoHang = dg.TrangThaiGiaoHang,
                DiaChiGiaoHang = dg.DiaChiGiaoHang,
                SoDienThoaiNguoiNhan = dg.SoDienThoaiNguoiNhan,
                TenNguoiNhan = dg.TenNguoiNhan,
                PhiGiaoHang = dg.PhiGiaoHang,
                NgayTao = dg.NgayTao,
                NgayXacNhan = dg.NgayXacNhan,
                NgayLayHang = dg.NgayLayHang,
                NgayBatDauGiao = dg.NgayBatDauGiao,
                NgayGiaoThanhCong = dg.NgayGiaoThanhCong,
                GhiChu = dg.GhiChu,
                LyDoHuy = dg.LyDoHuy
            });
        }
    }
}

