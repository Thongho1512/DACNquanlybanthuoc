using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Middleware.Exceptions;
using AutoMapper;

namespace TestProject
{
    public class ChiNhanhServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ChiNhanhService>> _loggerMock;
        private readonly ChiNhanhService _service;

        public ChiNhanhServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _chiNhanhRepositoryMock = new Mock<IChiNhanhRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ChiNhanhService>>();

            _unitOfWorkMock.Setup(x => x.ChiNhanhRepository).Returns(_chiNhanhRepositoryMock.Object);

            // constructor: (IUnitOfWork unitOfWork, ILogger<ChiNhanhService> logger, IMapper mapper)
            _service = new ChiNhanhService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateChiNhanhDto 
            { 
                TenChiNhanh = "Chi Nhánh Hà N?i",
                DiaChi = "123 ???ng Lý Th??ng Ki?t, Hà N?i"
            };
            var chiNhanhEntity = new ChiNhanh 
            { 
                Id = 1, 
                TenChiNhanh = "Chi Nhánh Hà N?i",
                DiaChi = "123 ???ng Lý Th??ng Ki?t, Hà N?i",
                TrangThai = true
            };
            var expectedDto = new ChiNhanhDto 
            { 
                Id = 1, 
                TenChiNhanh = "Chi Nhánh Hà N?i"
            };

            _mapperMock.Setup(x => x.Map<ChiNhanh>(createDto))
                .Returns(chiNhanhEntity);
            _chiNhanhRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<ChiNhanh>()))
                .ReturnsAsync(chiNhanhEntity);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            _mapperMock.Setup(x => x.Map<ChiNhanhDto>(chiNhanhEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenChiNhanh.Should().Be("Chi Nhánh Hà N?i");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            int branchId = 1;
            var chiNhanhEntity = new ChiNhanh 
            { 
                Id = branchId, 
                TenChiNhanh = "Chi Nhánh Hà N?i",
                DiaChi = "123 ???ng Lý Th??ng Ki?t",
                TrangThai = true
            };
            var expectedDto = new ChiNhanhDto 
            { 
                Id = branchId, 
                TenChiNhanh = "Chi Nhánh Hà N?i"
            };

            _chiNhanhRepositoryMock.Setup(x => x.GetByIdAsync(branchId))
                .ReturnsAsync(chiNhanhEntity);
            _mapperMock.Setup(x => x.Map<ChiNhanhDto>(chiNhanhEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(branchId);

            // Assert
            result.Should().NotBeNull();
            result?.TenChiNhanh.Should().Be("Chi Nhánh Hà N?i");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _chiNhanhRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((ChiNhanh?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(invalidId));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithValidParams_ReturnsPagedResult()
        {
            // Arrange
            var chiNhanhList = new List<ChiNhanh>
            {
                new ChiNhanh { Id = 1, TenChiNhanh = "Chi Nhánh Hà N?i", TrangThai = true },
                new ChiNhanh { Id = 2, TenChiNhanh = "Chi Nhánh TP HCM", TrangThai = true }
            };
            var pagedResult = new PagedResult<ChiNhanh>
            {
                Items = chiNhanhList,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _chiNhanhRepositoryMock.Setup(x => x.GetPagedListAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(1, 10, true);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            int branchId = 1;
            var updateDto = new UpdateChiNhanhDto 
            { 
                TenChiNhanh = "Chi Nhánh Hà N?i C?p Nh?t" 
            };
            var existingBranch = new ChiNhanh 
            { 
                Id = branchId, 
                TenChiNhanh = "Chi Nhánh Hà N?i"
            };

            _chiNhanhRepositoryMock.Setup(x => x.GetByIdAsync(branchId))
                .ReturnsAsync(existingBranch);
            _chiNhanhRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ChiNhanh>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _service.UpdateAsync(branchId, updateDto);

            // Assert
            _chiNhanhRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ChiNhanh>()), Times.Once);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            int branchId = 1;
            var chiNhanh = new ChiNhanh 
            { 
                Id = branchId, 
                TenChiNhanh = "Chi Nhánh",
                TrangThai = true
            };

            _chiNhanhRepositoryMock.Setup(x => x.GetByIdAsync(branchId))
                .ReturnsAsync(chiNhanh);
            _chiNhanhRepositoryMock.Setup(x => x.SoftDeleteAsync(It.IsAny<ChiNhanh>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _service.SoftDeleteAsync(branchId);

            // Assert
            _chiNhanhRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<ChiNhanh>()), Times.Once);
        }

        #endregion
    }
}
