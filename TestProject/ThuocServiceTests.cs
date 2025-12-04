using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Middleware.Exceptions;
using AutoMapper;

namespace TestProject
{
    public class ThuocServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IThuocRepository> _thuocRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ThuocService>> _loggerMock;
        private readonly ThuocService _service;

        public ThuocServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _thuocRepositoryMock = new Mock<IThuocRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ThuocService>>();

            _unitOfWorkMock.Setup(x => x.ThuocRepository).Returns(_thuocRepositoryMock.Object);

            _service = new ThuocService(_loggerMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
        }

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            int thuocId = 1;
            var thuocEntity = new Thuoc { Id = thuocId, TenThuoc = "Thu?c Test", TrangThai = true };
            var expectedDto = new ThuocDto { Id = thuocId, TenThuoc = "Thu?c Test" };

            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(thuocId))
                .ReturnsAsync(thuocEntity);
            _mapperMock.Setup(x => x.Map<ThuocDto>(thuocEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(thuocId);

            // Assert
            result.Should().NotBeNull();
            result?.Id.Should().Be(thuocId);
            result?.TenThuoc.Should().Be("Thu?c Test");
            _thuocRepositoryMock.Verify(x => x.GetByIdAsync(thuocId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((Thuoc?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(invalidId));
        }

        [Fact]
        public async Task GetByIdAsync_WithInactiveThuoc_ThrowsNotFoundException()
        {
            // Arrange
            int thuocId = 1;
            var inactiveThuoc = new Thuoc { Id = thuocId, TenThuoc = "Thu?c Inactive", TrangThai = false };
            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(thuocId))
                .ReturnsAsync(inactiveThuoc);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(thuocId));
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateThuocDto 
            { 
                TenThuoc = "Thu?c M?i",
                GiaBan = 50000,
                DonVi = "Viên"
            };
            var thuocEntity = new Thuoc 
            { 
                Id = 1, 
                TenThuoc = "Thu?c M?i",
                GiaBan = 50000,
                DonVi = "Viên",
                TrangThai = true
            };
            var expectedDto = new ThuocDto 
            { 
                Id = 1, 
                TenThuoc = "Thu?c M?i",
                GiaBan = 50000 
            };

            _mapperMock.Setup(x => x.Map<Thuoc>(createDto))
                .Returns(thuocEntity);
            _thuocRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Thuoc>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<ThuocDto>(thuocEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenThuoc.Should().Be("Thu?c M?i");
            _thuocRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Thuoc>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNullDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            int thuocId = 1;
            var updateDto = new UpdateThuocDto { TenThuoc = "Thu?c C?p Nh?t", GiaBan = 60000 };
            var existingThuoc = new Thuoc { Id = thuocId, TenThuoc = "Thu?c C?", GiaBan = 50000 };

            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(thuocId))
                .ReturnsAsync(existingThuoc);
            _thuocRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Thuoc>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(thuocId, updateDto);

            // Assert
            _thuocRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Thuoc>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            var updateDto = new UpdateThuocDto { TenThuoc = "Thu?c" };
            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((Thuoc?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(invalidId, updateDto));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithValidParams_ReturnsPagedResult()
        {
            // Arrange
            var thuocList = new List<Thuoc>
            {
                new Thuoc { Id = 1, TenThuoc = "Thu?c 1", TrangThai = true },
                new Thuoc { Id = 2, TenThuoc = "Thu?c 2", TrangThai = true }
            };
            var pagedResult = new PagedResult<Thuoc>
            {
                Items = thuocList,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };
            var expectedDtoList = thuocList.Select(t => new ThuocDto { Id = t.Id, TenThuoc = t.TenThuoc }).ToList();

            _thuocRepositoryMock.Setup(x => x.GetPagedListAsync(1, 10, true, null, null))
                .ReturnsAsync(pagedResult);
            _mapperMock.Setup(x => x.Map<List<ThuocDto>>(thuocList))
                .Returns(expectedDtoList);

            // Act
            var result = await _service.GetAllAsync(1, 10, true);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            int thuocId = 1;
            var thuoc = new Thuoc { Id = thuocId, TenThuoc = "Thu?c", TrangThai = true };

            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(thuocId))
                .ReturnsAsync(thuoc);
            _thuocRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Thuoc>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.SoftDeleteAsync(thuocId);

            // Assert
            thuoc.TrangThai.Should().BeFalse();
            _thuocRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Thuoc>()), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _thuocRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((Thuoc?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.SoftDeleteAsync(invalidId));
        }

        #endregion
    }
}
