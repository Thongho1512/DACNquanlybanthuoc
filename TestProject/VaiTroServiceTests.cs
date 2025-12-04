using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.VaiTro;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Middleware.Exceptions;
using AutoMapper;

namespace TestProject
{
    public class VaiTroServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IVaiTroRepository> _vaiTroRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<VaiTroService>> _loggerMock;
        private readonly VaiTroService _service;

        public VaiTroServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _vaiTroRepositoryMock = new Mock<IVaiTroRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<VaiTroService>>();

            _unitOfWorkMock.Setup(x => x.VaiTroRepository).Returns(_vaiTroRepositoryMock.Object);

            _service = new VaiTroService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateVaiTroDto 
            { 
                TenVaiTro = "ADMIN",
                MoTa = "Vai trò qu?n tr? viên"
            };
            var vaiTroEntity = new VaiTro 
            { 
                Id = 1, 
                TenVaiTro = "ADMIN",
                MoTa = "Vai trò qu?n tr? viên",
                TrangThai = true
            };
            var expectedDto = new VaiTroDto 
            { 
                Id = 1, 
                TenVaiTro = "ADMIN"
            };

            _mapperMock.Setup(x => x.Map<VaiTro>(createDto))
                .Returns(vaiTroEntity);
            _vaiTroRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<VaiTro>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<VaiTroDto>(vaiTroEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TenVaiTro.Should().Be("ADMIN");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            int roleId = 1;
            var vaiTroEntity = new VaiTro 
            { 
                Id = roleId, 
                TenVaiTro = "ADMIN",
                TrangThai = true
            };
            var expectedDto = new VaiTroDto 
            { 
                Id = roleId, 
                TenVaiTro = "ADMIN"
            };

            _vaiTroRepositoryMock.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(vaiTroEntity);
            _mapperMock.Setup(x => x.Map<VaiTroDto>(vaiTroEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(roleId);

            // Assert
            result.Should().NotBeNull();
            result?.TenVaiTro.Should().Be("ADMIN");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _vaiTroRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((VaiTro?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(invalidId));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithValidParams_ReturnsPagedResult()
        {
            // Arrange
            var vaiTroList = new List<VaiTro>
            {
                new VaiTro { Id = 1, TenVaiTro = "ADMIN", TrangThai = true },
                new VaiTro { Id = 2, TenVaiTro = "STAFF", TrangThai = true }
            };
            var pagedResult = new PagedResult<VaiTro>
            {
                Items = vaiTroList,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _vaiTroRepositoryMock.Setup(x => x.GetPagedListAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(1, 10, true);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }

        #endregion

        #region GetAllActiveAsync Tests

        [Fact]
        public async Task GetAllActiveAsync_ReturnsAllActiveRoles()
        {
            // Arrange
            var activeRoles = new List<VaiTro>
            {
                new VaiTro { Id = 1, TenVaiTro = "ADMIN", TrangThai = true },
                new VaiTro { Id = 2, TenVaiTro = "STAFF", TrangThai = true }
            };

            _vaiTroRepositoryMock.Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(activeRoles);

            // Act
            var result = await _service.GetAllActiveAsync();

            // Assert
            result.Should().HaveCount(2);
            result.All(x => x.TrangThai == true).Should().BeTrue();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            int roleId = 1;
            var updateDto = new UpdateVaiTroDto 
            { 
                TenVaiTro = "MANAGER" 
            };
            var existingRole = new VaiTro 
            { 
                Id = roleId, 
                TenVaiTro = "ADMIN"
            };

            _vaiTroRepositoryMock.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(existingRole);
            _vaiTroRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<VaiTro>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(roleId, updateDto);

            // Assert
            _vaiTroRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<VaiTro>()), Times.Once);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            int roleId = 1;
            var role = new VaiTro 
            { 
                Id = roleId, 
                TenVaiTro = "ADMIN",
                TrangThai = true
            };

            _vaiTroRepositoryMock.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(role);
            _vaiTroRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<VaiTro>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.SoftDeleteAsync(roleId);

            // Assert
            role.TrangThai.Should().BeFalse();
        }

        #endregion
    }
}
