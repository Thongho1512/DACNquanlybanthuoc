using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonGiaoHang;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Tests.Controllers
{
    public class DonGiaoHangsControllerTests
    {
        private readonly Mock<IDonGiaoHangService> _donGiaoHangServiceMock;
        private readonly Mock<ILogger<DonGiaoHangsController>> _loggerMock;
        private readonly DonGiaoHangsController _controller;

        public DonGiaoHangsControllerTests()
        {
            _donGiaoHangServiceMock = new Mock<IDonGiaoHangService>();
            _loggerMock = new Mock<ILogger<DonGiaoHangsController>>();
            _controller = new DonGiaoHangsController(_loggerMock.Object, _donGiaoHangServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "ADMIN"),
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task CreateDonGiaoHang_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateDonGiaoHangDto { IddonHang = 1 };
            var expectedResult = new DonGiaoHangDto { Id = 1 };
            _donGiaoHangServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateDonGiaoHang(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonGiaoHangDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetDonGiaoHangById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new DonGiaoHangDto { Id = id };
            _donGiaoHangServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDonGiaoHangById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonGiaoHangDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UpdateDeliveryStatus_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateDeliveryStatusDto { TrangThaiGiaoHang = "DANG_GIAO" };
            var donGiaoHang = new DonGiaoHangDto { Id = id };
            _donGiaoHangServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(donGiaoHang);
            _donGiaoHangServiceMock.Setup(s => s.UpdateStatusAsync(id, dto)).ReturnsAsync(new DonGiaoHangDto { Id = id, TrangThaiGiaoHang = "DANG_GIAO" });

            // Act
            var result = await _controller.UpdateDeliveryStatus(id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonGiaoHangDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
