using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services;
using quanlybanthuoc.Services.Impl;

namespace quanlybanthuoc.Tests.Services
{
    public class DonHangServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DonHangService>> _loggerMock;
        private readonly Mock<IKhachHangService> _khachHangServiceMock;
        private readonly Mock<IDonHangRepository> _donHangRepoMock;
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepoMock;
        private readonly Mock<IKhachHangRepository> _khachHangRepoMock;
        private readonly Mock<IPhuongThucThanhToanRepository> _ptttRepoMock;
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<ILoHangRepository> _loHangRepoMock;
        private readonly Mock<IKhoHangRepository> _khoHangRepoMock;
        private readonly Mock<IChiTietDonHangRepository> _ctdhRepoMock;
        private readonly Mock<ILichSuDiemRepository> _lsdiemRepoMock;
        private readonly DonHangService _service;

        public DonHangServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DonHangService>>();
            _khachHangServiceMock = new Mock<IKhachHangService>();
            
            _donHangRepoMock = new Mock<IDonHangRepository>();
            _chiNhanhRepoMock = new Mock<IChiNhanhRepository>();
            _khachHangRepoMock = new Mock<IKhachHangRepository>();
            _ptttRepoMock = new Mock<IPhuongThucThanhToanRepository>();
            _thuocRepoMock = new Mock<IThuocRepository>();
            _loHangRepoMock = new Mock<ILoHangRepository>();
            _khoHangRepoMock = new Mock<IKhoHangRepository>();
            _ctdhRepoMock = new Mock<IChiTietDonHangRepository>();
            _lsdiemRepoMock = new Mock<ILichSuDiemRepository>();

            _unitOfWorkMock.Setup(u => u.DonHangRepository).Returns(_donHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiNhanhRepository).Returns(_chiNhanhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhachHangRepository).Returns(_khachHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.PhuongThucThanhToanRepository).Returns(_ptttRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LoHangRepository).Returns(_loHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhoHangRepository).Returns(_khoHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiTietDonHangRepository).Returns(_ctdhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LichSuDiemRepository).Returns(_lsdiemRepoMock.Object);

            _service = new DonHangService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _khachHangServiceMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_BranchNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateDonHangDto { IdchiNhanh = 1 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ChiNhanh)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task CreateAsync_Success_CreatesOrderAndUpdatesInventory()
        {
            // Arrange
            var dto = new CreateDonHangDto
            {
                IdchiNhanh = 1,
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<ChiTietDonHangItemDto>
                {
                    new ChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 5, DonGia = 10000 }
                }
            };

            var chiNhanh = new ChiNhanh { Id = 1, TrangThai = true };
            var pttt = new PhuongThucThanhToan { Id = 1, TrangThai = true, TenPhuongThuc = "Tiền mặt" };
            var thuoc = new Thuoc { Id = 1, TenThuoc = "Paracetamol", TrangThai = true, GiaBan = 10000 };
            var loHang = new LoHang 
            { 
                Id = 1, 
                SoLo = "LOT001", 
                NgayHetHan = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                KhoHangs = new List<KhoHang> { new KhoHang { IdchiNhanh = 1, SoLuongTon = 10 } }
            };
            var khoHang = new KhoHang { IdchiNhanh = 1, IdloHang = 1, SoLuongTon = 10 };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(chiNhanh);
            _ptttRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pttt);
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(thuoc);
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 1)).ReturnsAsync(khoHang);
            
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonHang { Id = 1 });

            // Act
            var result = await _service.CreateAsync(dto, 1);

            // Assert
            _donHangRepoMock.Verify(r => r.CreateAsync(It.IsAny<DonHang>()), Times.Once);
            _ctdhRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ChiTietDonHang>>()), Times.Once);
            _khoHangRepoMock.Verify(r => r.TruTonKhoAsync(1, 1, 5), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_OrderNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((DonHang)null);

            // Act & Assert
            await _service.Invoking(s => s.GetByIdAsync(1))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_Success_RevertsPointsAndStock()
        {
            // Arrange
            var donHang = new DonHang
            {
                Id = 1,
                IdkhachHang = 1,
                IdchiNhanh = 1,
                IdkhachHangNavigation = new KhachHang { Id = 1, DiemTichLuy = 100 },
                ChiTietDonHangs = new List<ChiTietDonHang>
                {
                    new ChiTietDonHang { Idthuoc = 1, SoLuong = 5, IdthuocNavigation = new Thuoc { Id = 1 } }
                }
            };

            var lichSuDiem = new LichSuDiem { Id = 1, DiemCong = 10, DiemTru = 0 };

            var loHang = new LoHang 
            { 
                Id = 1, 
                SoLo = "LOT001",
                KhoHangs = new List<KhoHang> { new KhoHang { IdchiNhanh = 1 } }
            };

            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(donHang);
            _lsdiemRepoMock.Setup(r => r.GetByDonHangIdAsync(1)).ReturnsAsync(lichSuDiem);
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            
            // Act
            await _service.DeleteAsync(1);

            // Assert
            _khachHangServiceMock.Verify(s => s.UpdateDiemTichLuyAsync(1, 0, 10), Times.Once);
            _khoHangRepoMock.Verify(r => r.CongTonKhoAsync(1, It.IsAny<int>(), 5), Times.AtLeastOnce);
            _donHangRepoMock.Verify(r => r.DeleteAsync(donHang), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Success_UpdatesOrderDetails()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateDonHangDto
            {
                IdphuongThucTt = 1,
                ChiTietDonHangs = new List<UpdateChiTietDonHangItemDto>
                {
                    new UpdateChiTietDonHangItemDto { Idthuoc = 1, SoLuong = 3, DonGia = 10000 }
                }
            };

            var donHang = new DonHang
            {
                Id = id,
                IdchiNhanh = 1,
                ChiTietDonHangs = new List<ChiTietDonHang>
                {
                    new ChiTietDonHang { Idthuoc = 1, SoLuong = 5, IdthuocNavigation = new Thuoc { Id = 1 } }
                }
            };

            var pttt = new PhuongThucThanhToan { Id = 1, TrangThai = true };
            var thuoc = new Thuoc { Id = 1, TenThuoc = "Paracetamol", TrangThai = true };
            var loHang = new LoHang 
            { 
                Id = 1, 
                KhoHangs = new List<KhoHang> { new KhoHang { IdchiNhanh = 1, SoLuongTon = 10 } }
            };

            _donHangRepoMock.Setup(r => r.GetByIdWithDetailsAsync(id)).ReturnsAsync(donHang);
            _ptttRepoMock.Setup(r => r.GetByIdAsync(dto.IdphuongThucTt)).ReturnsAsync(pttt);
            _loHangRepoMock.Setup(r => r.GetByThuocIdAsync(1)).ReturnsAsync(new List<LoHang> { loHang });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(thuoc);
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 1)).ReturnsAsync(loHang.KhoHangs.First());

            // Act
            await _service.UpdateAsync(id, dto);

            // Assert
            _ctdhRepoMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<ChiTietDonHang>>()), Times.Once);
            _ctdhRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ChiTietDonHang>>()), Times.Once);
            _donHangRepoMock.Verify(r => r.UpdateAsync(donHang), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }
    }
}
