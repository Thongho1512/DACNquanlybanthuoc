using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Middleware.Exceptions;
using AutoMapper;

namespace TestProject
{
    public class DanhMucServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IDanhMucRepository> _danhMucRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DanhMucService>> _loggerMock;
        private readonly DanhMucService _service;

        public DanhMucServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _danhMucRepositoryMock = new Mock<IDanhMucRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DanhMucService>>();

            _unitOfWorkMock.Setup(x => x.DanhMucRepository).Returns(_danhMucRepositoryMock.Object);

            // constructor: (IUnitOfWork unitOfWork, ILogger<DanhMucService> logger, IMapper mapper)
            _service = new DanhMucService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateDanhMucDto 
            { 
                TenDanhMuc = "Vitamin",
                MoTa = "Danh m?c vitamin"
            };
            var danhMucEntity = new DanhMuc 
            { 
                Id = 1, 
                TenDanhMuc = "Vitamin",
                MoTa = "Danh m?c vitamin",
                TrangThai = true
            };
            var expectedDto = new DanhMucDto 
            { 
                Id = 1, 
                TenDanhMuc = "Vitamin"
            };

            _mapperMock.Setup(x => x.Map<DanhMuc>(createDto))
                .Returns(danhMucEntity);
            _danhMucRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<DanhMuc>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<DanhMucDto>(danhMucEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenDanhMuc.Should().Be("Vitamin");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            int categoryId = 1;
            var danhMucEntity = new DanhMuc 
            { 
                Id = categoryId, 
                TenDanhMuc = "Vitamin",
                TrangThai = true
            };
            var expectedDto = new DanhMucDto 
            { 
                Id = categoryId, 
                TenDanhMuc = "Vitamin"
            };

            _danhMucRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(danhMucEntity);
            _mapperMock.Setup(x => x.Map<DanhMucDto>(danhMucEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result?.TenDanhMuc.Should().Be("Vitamin");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _danhMucRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((DanhMuc?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(invalidId));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Arrange
            var danhMucList = new List<DanhMuc>
            {
                new DanhMuc { Id = 1, TenDanhMuc = "Vitamin", TrangThai = true },
                new DanhMuc { Id = 2, TenDanhMuc = "Kháng Sinh", TrangThai = true }
            };
            var expectedDtoList = new List<DanhMucDto>
            {
                new DanhMucDto { Id = 1, TenDanhMuc = "Vitamin" },
                new DanhMucDto { Id = 2, TenDanhMuc = "Kháng Sinh" }
            };

            _danhMucRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(danhMucList);
            _mapperMock.Setup(x => x.Map<List<DanhMucDto>>(danhMucList))
                .Returns(expectedDtoList);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().TenDanhMuc.Should().Be("Vitamin");
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            int categoryId = 1;
            var updateDto = new UpdateDanhMucDto 
            { 
                TenDanhMuc = "Vitamin C?p Nh?t" 
            };
            var existingDanhMuc = new DanhMuc 
            { 
                Id = categoryId, 
                TenDanhMuc = "Vitamin"
            };

            _danhMucRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(existingDanhMuc);
            _danhMucRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DanhMuc>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(categoryId, updateDto);

            // Assert
            _danhMucRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DanhMuc>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            int categoryId = 1;
            var danhMuc = new DanhMuc 
            { 
                Id = categoryId, 
                TenDanhMuc = "Vitamin"
            };

            _danhMucRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(danhMuc);
            _danhMucRepositoryMock.Setup(x => x.DeleteAsync(danhMuc))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(categoryId);

            // Assert
            _danhMucRepositoryMock.Verify(x => x.DeleteAsync(danhMuc), Times.Once);
        }

        #endregion
    }
}
