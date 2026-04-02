using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.DonNhapHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services.Impl;

namespace quanlybanthuoc.Tests.Services
{
    public class DonNhapHangServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DonNhapHangService>> _loggerMock;
        private readonly Mock<IDonNhapHangRepository> _donNhapRepoMock;
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepoMock;
        private readonly Mock<INhaCungCapRepository> _nccRepoMock;
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<ILoHangRepository> _loHangRepoMock;
        private readonly Mock<IKhoHangRepository> _khoHangRepoMock;
        private readonly DonNhapHangService _service;

        public DonNhapHangServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DonNhapHangService>>();

            _donNhapRepoMock = new Mock<IDonNhapHangRepository>();
            _chiNhanhRepoMock = new Mock<IChiNhanhRepository>();
            _nccRepoMock = new Mock<INhaCungCapRepository>();
            _thuocRepoMock = new Mock<IThuocRepository>();
            _loHangRepoMock = new Mock<ILoHangRepository>();
            _khoHangRepoMock = new Mock<IKhoHangRepository>();

            _unitOfWorkMock.Setup(u => u.DonNhapHangRepository).Returns(_donNhapRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiNhanhRepository).Returns(_chiNhanhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NhaCungCapRepository).Returns(_nccRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LoHangRepository).Returns(_loHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhoHangRepository).Returns(_khoHangRepoMock.Object);

            _service = new DonNhapHangService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_BranchNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto { IdchiNhanh = 1 };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ChiNhanh)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_DuplicateSoDonNhap_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto { IdchiNhanh = 1, IdnhaCungCap = 1, SoDonNhap = "IMPORT001" };
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nccRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _donNhapRepoMock.Setup(r => r.GetBySoDonNhapAsync("IMPORT001")).ReturnsAsync(new DonNhapHang());

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, 1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Số đơn nhập đã tồn tại.");
        }

        [Fact]
        public async Task CreateAsync_Success_CreatesOrderAndBatches()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto
            {
                IdchiNhanh = 1,
                IdnhaCungCap = 1,
                SoDonNhap = "IMPORT001",
                NgayNhap = DateOnly.FromDateTime(DateTime.Now),
                LoHangs = new List<LoHangNhapDto>
                {
                    new LoHangNhapDto 
                    { 
                        Idthuoc = 1, 
                        SoLo = "LOT001", 
                        SoLuong = 100, 
                        GiaNhap = 8000,
                        NgaySanXuat = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)),
                        NgayHetHan = DateOnly.FromDateTime(DateTime.Now.AddMonths(11))
                    }
                }
            };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            _nccRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _donNhapRepoMock.Setup(r => r.GetBySoDonNhapAsync("IMPORT001")).ReturnsAsync((DonNhapHang)null);
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TrangThai = true });
            
            _donNhapRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new DonNhapHang { Id = 1 });

            // Act
            var result = await _service.CreateAsync(dto, 1);

            // Assert
            _donNhapRepoMock.Verify(r => r.CreateAsync(It.IsAny<DonNhapHang>()), Times.Once);
            _loHangRepoMock.Verify(r => r.CreateAsync(It.IsAny<LoHang>()), Times.Once);
            _khoHangRepoMock.Verify(r => r.CreateAsync(It.IsAny<KhoHang>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_BatchUsed_ThrowsBadRequestException()
        {
            // Arrange
            var donNhapHang = new DonNhapHang
            {
                Id = 1,
                LoHangs = new List<LoHang>
                {
                    new LoHang 
                    { 
                        Id = 1, 
                        SoLo = "LOT001", 
                        SoLuong = 100,
                        KhoHangs = new List<KhoHang> 
                        { 
                            new KhoHang { IdchiNhanh = 1, SoLuongTon = 50 } // Already sold 50
                        }
                    }
                }
            };

            _donNhapRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(donNhapHang);

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(1))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Không thể xóa đơn nhập hàng vì lô LOT001 đã được sử dụng.");
        }

        [Fact]
        public async Task UpdateAsync_Success_UpdatesBatches()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateDonNhapHangDto
            {
                IdnhaCungCap = 1,
                SoDonNhap = "IMPORT001_MOD",
                NgayNhap = DateOnly.FromDateTime(DateTime.Now),
                LoHangs = new List<UpdateLoHangNhapDto>
                {
                    new UpdateLoHangNhapDto 
                    { 
                        Id = 1, 
                        Idthuoc = 1, 
                        SoLo = "LOT001", 
                        SoLuong = 150, // Increased
                        GiaNhap = 8000,
                        NgaySanXuat = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)),
                        NgayHetHan = DateOnly.FromDateTime(DateTime.Now.AddMonths(11))
                    }
                }
            };

            var donNhapHang = new DonNhapHang
            {
                Id = id,
                IdchiNhanh = 1,
                SoDonNhap = "IMPORT001",
                LoHangs = new List<LoHang>
                {
                    new LoHang { Id = 1, Idthuoc = 1, SoLuong = 100 }
                }
            };

            _donNhapRepoMock.Setup(r => r.GetByIdWithDetailsAsync(id)).ReturnsAsync(donNhapHang);
            _nccRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NhaCungCap { Id = 1, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Thuoc { Id = 1, TrangThai = true });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(1, 1)).ReturnsAsync(new KhoHang { SoLuongTon = 100 });

            // Act
            await _service.UpdateAsync(id, dto);

            // Assert
            _loHangRepoMock.Verify(r => r.UpdateAsync(It.IsAny<LoHang>()), Times.Once);
            _khoHangRepoMock.Verify(r => r.CongTonKhoAsync(1, 1, 50), Times.Once);
            _donNhapRepoMock.Verify(r => r.UpdateAsync(donNhapHang), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }
    }
}
