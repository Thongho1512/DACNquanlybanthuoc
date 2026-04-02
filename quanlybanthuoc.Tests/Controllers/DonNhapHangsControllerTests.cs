using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonNhapHang;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Tests.Controllers
{
    public class DonNhapHangsControllerTests
    {
        private readonly Mock<IDonNhapHangService> _donNhapHangServiceMock;
        private readonly Mock<ILogger<DonNhapHangsController>> _loggerMock;
        private readonly DonNhapHangsController _controller;

        public DonNhapHangsControllerTests()
        {
            _donNhapHangServiceMock = new Mock<IDonNhapHangService>();
            _loggerMock = new Mock<ILogger<DonNhapHangsController>>();
            _controller = new DonNhapHangsController(_loggerMock.Object, _donNhapHangServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task CreateDonNhapHang_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateDonNhapHangDto { SoDonNhap = "IMPORT001" };
            var expectedResult = new DonNhapHangDto { Id = 1, SoDonNhap = "IMPORT001" };
            _donNhapHangServiceMock.Setup(s => s.CreateAsync(dto, 1)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateDonNhapHang(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonNhapHangDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetDonNhapHangById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new DonNhapHangDto { Id = id };
            _donNhapHangServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDonNhapHangById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonNhapHangDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UpdateDonNhapHang_Success_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateDonNhapHangDto { SoDonNhap = "IMPORT001_UPD" };

            // Act
            var result = await _controller.UpdateDonNhapHang(id, dto);

            // Assert
            _donNhapHangServiceMock.Verify(s => s.UpdateAsync(id, dto), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDonNhapHang_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteDonNhapHang(id);

            // Assert
            _donNhapHangServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
