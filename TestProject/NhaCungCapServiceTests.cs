using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NhaCungCap;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Middleware.Exceptions;
using AutoMapper;

namespace TestProject
{
    public class NhaCungCapServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INhaCungCapRepository> _nhaCungCapRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<NhaCungCapService>> _loggerMock;
        private readonly NhaCungCapService _service;

        public NhaCungCapServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _nhaCungCapRepositoryMock = new Mock<INhaCungCapRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<NhaCungCapService>>();

            _unitOfWorkMock.Setup(x => x.NhaCungCapRepository).Returns(_nhaCungCapRepositoryMock.Object);

            // Correct constructor parameter order: (IUnitOfWork unitOfWork, ILogger<NhaCungCapService> logger, IMapper mapper)
            _service = new NhaCungCapService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateNhaCungCapDto 
            { 
                TenNhaCungCap = "Công ty D??c Hàng ??u",
                Sdt = "0912345678",
                Email = "contact@duoc.com",
                DiaChi = "123 ???ng ABC, Hà N?i"
            };
            var nhaCungCapEntity = new NhaCungCap 
            { 
                Id = 1, 
                TenNhaCungCap = "Công ty D??c Hàng ??u",
                Sdt = "0912345678",
                Email = "contact@duoc.com",
                DiaChi = "123 ???ng ABC, Hà N?i"
            };
            var expectedDto = new NhaCungCapDto 
            { 
                Id = 1, 
                TenNhaCungCap = "Công ty D??c Hàng ??u"
            };

            _mapperMock.Setup(x => x.Map<NhaCungCap>(createDto))
                .Returns(nhaCungCapEntity);
            // CreateAsync on repository returns Task<NhaCungCap>
            _nhaCungCapRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<NhaCungCap>()))
                .ReturnsAsync(nhaCungCapEntity);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            _mapperMock.Setup(x => x.Map<NhaCungCapDto>(nhaCungCapEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenNhaCungCap.Should().Be("Công ty D??c Hàng ??u");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            int supplierId = 1;
            var nhaCungCapEntity = new NhaCungCap 
            { 
                Id = supplierId, 
                TenNhaCungCap = "Công ty D??c Hàng ??u",
                Sdt = "0912345678"
            };
            var expectedDto = new NhaCungCapDto 
            { 
                Id = supplierId, 
                TenNhaCungCap = "Công ty D??c Hàng ??u"
            };

            _nhaCungCapRepositoryMock.Setup(x => x.GetByIdAsync(supplierId))
                .ReturnsAsync(nhaCungCapEntity);
            _mapperMock.Setup(x => x.Map<NhaCungCapDto>(nhaCungCapEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(supplierId);

            // Assert
            result.Should().NotBeNull();
            result?.TenNhaCungCap.Should().Be("Công ty D??c Hàng ??u");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _nhaCungCapRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((NhaCungCap?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(invalidId));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithValidParams_ReturnsPagedResult()
        {
            // Arrange
            var nhaCungCapList = new List<NhaCungCap>
            {
                new NhaCungCap { Id = 1, TenNhaCungCap = "Công ty 1", Sdt = "0111111111" },
                new NhaCungCap { Id = 2, TenNhaCungCap = "Công ty 2", Sdt = "0222222222" }
            };
            var pagedResult = new PagedResult<NhaCungCap>
            {
                Items = nhaCungCapList,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            // Repository GetPagedListAsync signature: (int pageNumber, int pageSize, bool active, string? searchTerm = null)
            _nhaCungCapRepositoryMock.Setup(x => x.GetPagedListAsync(1, 10, true, null))
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
            int supplierId = 1;
            var updateDto = new UpdateNhaCungCapDto 
            { 
                TenNhaCungCap = "Công ty D??c M?i" 
            };
            var existingSupplier = new NhaCungCap 
            { 
                Id = supplierId, 
                TenNhaCungCap = "Công ty D??c C?"
            };

            _nhaCungCapRepositoryMock.Setup(x => x.GetByIdAsync(supplierId))
                .ReturnsAsync(existingSupplier);
            _nhaCungCapRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<NhaCungCap>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _service.UpdateAsync(supplierId, updateDto);

            // Assert
            _nhaCungCapRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<NhaCungCap>()), Times.Once);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            int supplierId = 1;
            var nhaCungCap = new NhaCungCap 
            { 
                Id = supplierId, 
                TenNhaCungCap = "Công ty D??c"
            };

            _nhaCungCapRepositoryMock.Setup(x => x.GetByIdAsync(supplierId))
                .ReturnsAsync(nhaCungCap);
            _nhaCungCapRepositoryMock.Setup(x => x.SoftDeleteAsync(nhaCungCap))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _service.SoftDeleteAsync(supplierId);

            // Assert
            _nhaCungCapRepositoryMock.Verify(x => x.SoftDeleteAsync(nhaCungCap), Times.Once);
        }

        #endregion
    }
}
