using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Tests.Controllers
{
    public class NguoiDungsControllerTests
    {
        private readonly Mock<INguoiDungService> _nguoiDungServiceMock;
        private readonly Mock<ILogger<NguoiDungsController>> _loggerMock;
        private readonly NguoiDungsController _controller;

        public NguoiDungsControllerTests()
        {
            _nguoiDungServiceMock = new Mock<INguoiDungService>();
            _loggerMock = new Mock<ILogger<NguoiDungsController>>();
            _controller = new NguoiDungsController(_loggerMock.Object, _nguoiDungServiceMock.Object);
        }

        [Fact]
        public async Task CreateNguoiDung_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateNguoiDungDto { TenDangNhap = "staff1" };
            var expectedResult = new NguoiDungDto { Id = 1, TenDangNhap = "staff1" };
            _nguoiDungServiceMock.Setup(s => s.createAsync(dto)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateNguoiDung(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<NguoiDungDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetAllNguoiDungsIsActive_Success_ReturnsOk()
        {
            // Arrange
            var pagedResult = new PagedResult<NguoiDungDto> { Items = new List<NguoiDungDto>() };
            _nguoiDungServiceMock.Setup(s => s.GetAllAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAllNguoiDungsIsActive();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<NguoiDungDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task GetNguoiDungById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new NguoiDungDto { Id = id };
            _nguoiDungServiceMock.Setup(s => s.getByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetNguoiDungById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<NguoiDungDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UpdateNguoiDung_Success_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateNguoiDungDto { HoTen = "Updated Name" };

            // Act
            var result = await _controller.UpdateNguoiDung(id, dto);

            // Assert
            _nguoiDungServiceMock.Verify(s => s.updateAsync(id, dto), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteNguoiDung_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteNguoiDung(id);

            // Assert
            _nguoiDungServiceMock.Verify(s => s.SoftDeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
