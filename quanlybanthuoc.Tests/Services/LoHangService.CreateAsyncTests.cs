using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.LoHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services.Impl;

namespace quanlybanthuoc.Tests.Services
{
    public class LoHangServiceCreateAsyncTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<LoHangService>> _loggerMock;

        // Repositories
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepoMock;
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<ILoHangRepository> _loHangRepoMock;
        private readonly Mock<IKhoHangRepository> _khoHangRepoMock;

        private readonly LoHangService _service;

        public LoHangServiceCreateAsyncTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<LoHangService>>();

            _chiNhanhRepoMock = new Mock<IChiNhanhRepository>();
            _thuocRepoMock = new Mock<IThuocRepository>();
            _loHangRepoMock = new Mock<ILoHangRepository>();
            _khoHangRepoMock = new Mock<IKhoHangRepository>();

            _unitOfWorkMock.Setup(u => u.ChiNhanhRepository).Returns(_chiNhanhRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.LoHangRepository).Returns(_loHangRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.KhoHangRepository).Returns(_khoHangRepoMock.Object);

            _service = new LoHangService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        #region --- EXCEPTION PATHS ---

        [Fact]
        public async Task TC1_CreateAsync_BranchNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateLoHangDto();
            int idChiNhanh = 0;
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh)).ReturnsAsync((ChiNhanh)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, idChiNhanh))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task TC2_CreateAsync_BranchInactive_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateLoHangDto();
            int idChiNhanh = 1;
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = false });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, idChiNhanh))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Chi nhánh không tồn tại hoặc không hoạt động.");
        }

        [Fact]
        public async Task TC3_CreateAsync_MedicineNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateLoHangDto { Idthuoc = 999 };
            int idChiNhanh = 1;
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(dto.Idthuoc)).ReturnsAsync((Thuoc)null);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, idChiNhanh))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thuốc không tồn tại.");
        }

        [Fact]
        public async Task TC4_CreateAsync_MedicineInactive_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateLoHangDto { Idthuoc = 2 };
            int idChiNhanh = 1;
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(dto.Idthuoc))
                .ReturnsAsync(new Thuoc { Id = dto.Idthuoc, TrangThai = false });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, idChiNhanh))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thuốc không tồn tại.");
        }

        [Fact]
        public async Task TC5_CreateAsync_InvalidExpiryDate_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateLoHangDto 
            { 
                Idthuoc = 1,
                NgaySanXuat = new DateOnly(2025, 6, 1),
                NgayHetHan = new DateOnly(2025, 1, 1)
            };
            int idChiNhanh = 1;
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(dto.Idthuoc))
                .ReturnsAsync(new Thuoc { Id = dto.Idthuoc, TrangThai = true });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, idChiNhanh))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Ngày hết hạn phải sau ngày sản xuất.");
        }

        [Fact]
        public async Task TC6_CreateAsync_ExpiredMedicine_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateLoHangDto 
            { 
                Idthuoc = 1,
                NgaySanXuat = new DateOnly(2023, 1, 1),
                NgayHetHan = new DateOnly(2024, 1, 1) // In the past relative to 2026-03-31
            };
            int idChiNhanh = 1;
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(dto.Idthuoc))
                .ReturnsAsync(new Thuoc { Id = dto.Idthuoc, TrangThai = true });

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto, idChiNhanh))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Không thể nhập thuốc đã hết hạn.");
        }

        #endregion

        #region --- SUCCESS PATHS ---

        [Fact]
        public async Task TC7_CreateAsync_Success_CreateNewStock()
        {
            // Arrange
            var dto = new CreateLoHangDto 
            { 
                Idthuoc = 5,
                SoLo = "BATCH01",
                SoLuong = 10,
                GiaNhap = 1000,
                NgaySanXuat = new DateOnly(2025, 1, 1),
                NgayHetHan = new DateOnly(2026, 12, 31)
            };
            int idChiNhanh = 1;

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(dto.Idthuoc))
                .ReturnsAsync(new Thuoc { Id = dto.Idthuoc, TrangThai = true });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(idChiNhanh, It.IsAny<int>()))
                .ReturnsAsync((KhoHang)null); // New stock

            _mapperMock.Setup(m => m.Map<LoHangDto>(It.IsAny<LoHang>()))
                .Returns(new LoHangDto { Id = 1, TenThuoc = "Test Thuoc" });

            // Act
            var result = await _service.CreateAsync(dto, idChiNhanh);

            // Assert
            result.Should().NotBeNull();
            _loHangRepoMock.Verify(r => r.CreateAsync(It.Is<LoHang>(lh => lh.SoLo == "BATCH01")), Times.Once);
            _khoHangRepoMock.Verify(r => r.CreateAsync(It.Is<KhoHang>(kh => kh.SoLuongTon == 10)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task TC8_CreateAsync_Success_UpdateExistingStock()
        {
            // Arrange
            var dto = new CreateLoHangDto 
            { 
                Idthuoc = 5,
                SoLo = "BATCH01",
                SoLuong = 10,
                GiaNhap = 1000,
                NgaySanXuat = new DateOnly(2025, 1, 1),
                NgayHetHan = new DateOnly(2026, 12, 31)
            };
            int idChiNhanh = 1;
            var existingStock = new KhoHang { Id = 10, SoLuongTon = 20 };

            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(idChiNhanh))
                .ReturnsAsync(new ChiNhanh { Id = idChiNhanh, TrangThai = true });
            _thuocRepoMock.Setup(r => r.GetByIdAsync(dto.Idthuoc))
                .ReturnsAsync(new Thuoc { Id = dto.Idthuoc, TrangThai = true });
            _khoHangRepoMock.Setup(r => r.GetByChiNhanhAndLoHangAsync(idChiNhanh, It.IsAny<int>()))
                .ReturnsAsync(existingStock); // Existing stock

            _mapperMock.Setup(m => m.Map<LoHangDto>(It.IsAny<LoHang>()))
                .Returns(new LoHangDto { Id = 1 });

            // Act
            await _service.CreateAsync(dto, idChiNhanh);

            // Assert
            _khoHangRepoMock.Verify(r => r.CongTonKhoAsync(idChiNhanh, It.IsAny<int>(), 10), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        #endregion
    }
}
