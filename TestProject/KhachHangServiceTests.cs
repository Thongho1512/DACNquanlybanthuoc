using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Middleware.Exceptions;
using AutoMapper;

namespace TestProject
{
    public class KhachHangServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IKhachHangRepository> _khachHangRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<KhachHangService>> _loggerMock;
        private readonly KhachHangService _service;

        public KhachHangServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _khachHangRepositoryMock = new Mock<IKhachHangRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<KhachHangService>>();

            _unitOfWorkMock.Setup(x => x.KhachHangRepository).Returns(_khachHangRepositoryMock.Object);

            _service = new KhachHangService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateKhachHangDto 
            { 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = "0123456789"
            };
            var khachHangEntity = new KhachHang 
            { 
                Id = 1, 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = "0123456789",
                DiemTichLuy = 0
            };
            var expectedDto = new KhachHangDto 
            { 
                Id = 1, 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = "0123456789"
            };

            _mapperMock.Setup(x => x.Map<KhachHang>(createDto))
                .Returns(khachHangEntity);
            _khachHangRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<KhachHang>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<KhachHangDto>(khachHangEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenKhachHang.Should().Be("Nguy?n V?n A");
            result.Sdt.Should().Be("0123456789");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            int customerId = 1;
            var khachHangEntity = new KhachHang 
            { 
                Id = customerId, 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = "0123456789",
                DiemTichLuy = 100
            };
            var expectedDto = new KhachHangDto 
            { 
                Id = customerId, 
                TenKhachHang = "Nguy?n V?n A",
                DiemTichLuy = 100
            };

            _khachHangRepositoryMock.Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(khachHangEntity);
            _mapperMock.Setup(x => x.Map<KhachHangDto>(khachHangEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(customerId);

            // Assert
            result.Should().NotBeNull();
            result?.Id.Should().Be(customerId);
            result?.DiemTichLuy.Should().Be(100);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _khachHangRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((KhachHang?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(invalidId));
        }

        #endregion

        #region GetBySdtAsync Tests

        [Fact]
        public async Task GetBySdtAsync_WithValidSdt_ReturnsCorrectDto()
        {
            // Arrange
            string sdt = "0123456789";
            var khachHangEntity = new KhachHang 
            { 
                Id = 1, 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = sdt
            };
            var expectedDto = new KhachHangDto 
            { 
                Id = 1, 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = sdt
            };

            _khachHangRepositoryMock.Setup(x => x.GetBySdtAsync(sdt))
                .ReturnsAsync(khachHangEntity);
            _mapperMock.Setup(x => x.Map<KhachHangDto>(khachHangEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetBySdtAsync(sdt);

            // Assert
            result.Should().NotBeNull();
            result?.Sdt.Should().Be(sdt);
        }

        [Fact]
        public async Task GetBySdtAsync_WithInvalidSdt_ThrowsNotFoundException()
        {
            // Arrange
            string invalidSdt = "9999999999";
            _khachHangRepositoryMock.Setup(x => x.GetBySdtAsync(invalidSdt))
                .ReturnsAsync((KhachHang?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetBySdtAsync(invalidSdt));
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            int customerId = 1;
            var updateDto = new UpdateKhachHangDto 
            { 
                TenKhachHang = "Nguy?n V?n B",
                Sdt = "0987654321"
            };
            var existingKhachHang = new KhachHang 
            { 
                Id = customerId, 
                TenKhachHang = "Nguy?n V?n A",
                Sdt = "0123456789"
            };

            _khachHangRepositoryMock.Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(existingKhachHang);
            _khachHangRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<KhachHang>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(customerId, updateDto);

            // Assert
            _khachHangRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<KhachHang>()), Times.Once);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithValidParams_ReturnsPagedResult()
        {
            // Arrange
            var khachHangList = new List<KhachHang>
            {
                new KhachHang { Id = 1, TenKhachHang = "Khách 1", Sdt = "0111111111" },
                new KhachHang { Id = 2, TenKhachHang = "Khách 2", Sdt = "0222222222" }
            };
            var pagedResult = new PagedResult<KhachHang>
            {
                Items = khachHangList,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _khachHangRepositoryMock.Setup(x => x.GetPagedListAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(1, 10, true);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            int customerId = 1;
            var khachHang = new KhachHang 
            { 
                Id = customerId, 
                TenKhachHang = "Khách Hàng",
                TrangThai = true
            };

            _khachHangRepositoryMock.Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(khachHang);
            _khachHangRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<KhachHang>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.SoftDeleteAsync(customerId);

            // Assert
            khachHang.TrangThai.Should().BeFalse();
        }

        #endregion
    }
}
