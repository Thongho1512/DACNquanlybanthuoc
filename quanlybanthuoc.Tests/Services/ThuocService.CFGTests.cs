using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services.Impl;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Tests.Services
{
    public class ThuocServiceCFGTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ThuocService>> _loggerMock;
        
        private readonly Mock<IThuocRepository> _thuocRepoMock;
        private readonly Mock<IChiNhanhRepository> _chiNhanhRepoMock;
        
        private readonly ThuocService _service;

        public ThuocServiceCFGTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ThuocService>>();
            
            _thuocRepoMock = new Mock<IThuocRepository>();
            _chiNhanhRepoMock = new Mock<IChiNhanhRepository>();

            _unitOfWorkMock.Setup(u => u.ThuocRepository).Returns(_thuocRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ChiNhanhRepository).Returns(_chiNhanhRepoMock.Object);

            _service = new ThuocService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );
        }

        #region --- CreateAsync PATHS ---

        [Fact]
        public async Task TC1_CreateAsync_InvalidPrice_ThrowsBadRequestException()
        {
            // Arrange (Path: giaBan <= 0)
            var dto = new CreateThuocDto { TenThuoc = "A", GiaBan = 0 };

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto))
                .Should().ThrowAsync<BadRequestException>()
                .WithMessage("Lỗi: giá bán không hợp lệ");
        }

        [Fact]
        public async Task TC2_CreateAsync_Success()
        {
            // Arrange (Path: Normal success)
            var dto = new CreateThuocDto { TenThuoc = "A", GiaBan = 1000 };
            var entity = new Thuoc { Id = 1, TenThuoc = "A", GiaBan = 1000 };
            
            _mapperMock.Setup(m => m.Map<Thuoc>(dto)).Returns(entity);
            _mapperMock.Setup(m => m.Map<ThuocDto>(entity)).Returns(new ThuocDto { Id = 1 });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            _thuocRepoMock.Verify(r => r.CreateAsync(It.IsAny<Thuoc>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region --- GetByChiNhanhIdAsync PATHS ---

        [Fact]
        public async Task TC3_GetByChiNhanhIdAsync_BranchNotFound_ThrowsNotFoundException()
        {
            // Arrange (P1 True)
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ChiNhanh)null);

            // Act & Assert
            await _service.Invoking(s => s.GetByChiNhanhIdAsync(999, 1, 10, true))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy chi nhánh với id: 999");
        }

        [Fact]
        public async Task TC4_GetByChiNhanhIdAsync_Success()
        {
            // Arrange (P1 False)
            _chiNhanhRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChiNhanh { Id = 1, TrangThai = true });
            
            var pagedResult = new PagedResult<Thuoc> 
            { 
                Items = new List<Thuoc> { new Thuoc { Id = 1, TenThuoc = "A" } },
                TotalCount = 1
            };
            
            _thuocRepoMock.Setup(r => r.GetByChiNhanhIdAsync(1, 1, 10, true, null, null))
                .ReturnsAsync(pagedResult);
                
            _mapperMock.Setup(m => m.Map<ThuocDto>(It.IsAny<Thuoc>())).Returns(new ThuocDto { Id = 1 });

            // Act
            var result = await _service.GetByChiNhanhIdAsync(1, 1, 10, true);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
        }

        #endregion
    }
}
