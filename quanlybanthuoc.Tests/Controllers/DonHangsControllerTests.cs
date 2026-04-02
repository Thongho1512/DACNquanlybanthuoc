using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Tests.Controllers
{
    public class DonHangsControllerTests
    {
        private readonly Mock<IDonHangService> _donHangServiceMock;
        private readonly Mock<ILogger<DonHangsController>> _loggerMock;
        private readonly DonHangsController _controller;

        public DonHangsControllerTests()
        {
            _donHangServiceMock = new Mock<IDonHangService>();
            _loggerMock = new Mock<ILogger<DonHangsController>>();
            _controller = new DonHangsController(_loggerMock.Object, _donHangServiceMock.Object);

            // Mock HttpContext for User identification
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser"),
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task CreateDonHang_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateDonHangDto { IdchiNhanh = 1 };
            var expectedResult = new DonHangDto { Id = 1 };
            _donHangServiceMock.Setup(s => s.CreateAsync(dto, 1)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateDonHang(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonHangDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetDonHangById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new DonHangDto { Id = id };
            _donHangServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDonHangById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DonHangDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(id, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetAllDonHangs_Success_ReturnsOk()
        {
            // Arrange
            var pagedResult = new PagedResult<DonHangDto> { Items = new List<DonHangDto>() };
            _donHangServiceMock.Setup(s => s.GetAllAsync(1, 10, null, null, null, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAllDonHangs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<DonHangDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task DeleteDonHang_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteDonHang(id);

            // Assert
            _donHangServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
