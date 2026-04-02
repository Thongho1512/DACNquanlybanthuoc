using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Tests.Controllers
{
    public class KhachHangsControllerTests
    {
        private readonly Mock<IKhachHangService> _khachHangServiceMock;
        private readonly Mock<ILogger<KhachHangsController>> _loggerMock;
        private readonly KhachHangsController _controller;

        public KhachHangsControllerTests()
        {
            _khachHangServiceMock = new Mock<IKhachHangService>();
            _loggerMock = new Mock<ILogger<KhachHangsController>>();
            _controller = new KhachHangsController(_loggerMock.Object, _khachHangServiceMock.Object);
        }

        [Fact]
        public async Task CreateKhachHang_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateKhachHangDto { TenKhachHang = "John Doe" };
            var expectedResult = new KhachHangDto { Id = 1, TenKhachHang = "John Doe" };
            _khachHangServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateKhachHang(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<KhachHangDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetKhachHangById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new KhachHangDto { Id = id };
            _khachHangServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetKhachHangById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<KhachHangDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task GetKhachHangBySdt_Success_ReturnsOk()
        {
            // Arrange
            var sdt = "0123456789";
            var expectedResult = new KhachHangDto { Id = 1, Sdt = sdt };
            _khachHangServiceMock.Setup(s => s.GetBySdtAsync(sdt)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetKhachHangBySdt(sdt);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<KhachHangDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(sdt, apiResponse.Data?.Sdt);
        }

        [Fact]
        public async Task GetAllKhachHangs_Success_ReturnsOk()
        {
            // Arrange
            var pagedResult = new PagedResult<KhachHangDto> { Items = new List<KhachHangDto>() };
            _khachHangServiceMock.Setup(s => s.GetAllAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAllKhachHangs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<KhachHangDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UpdateKhachHang_Success_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateKhachHangDto { TenKhachHang = "John Doe Updated" };

            // Act
            var result = await _controller.UpdateKhachHang(id, dto);

            // Assert
            _khachHangServiceMock.Verify(s => s.UpdateAsync(id, dto), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteKhachHang_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteKhachHang(id);

            // Assert
            _khachHangServiceMock.Verify(s => s.SoftDeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
