using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Tests.Controllers
{
    public class ChiNhanhsControllerTests
    {
        private readonly Mock<IChiNhanhService> _chiNhanhServiceMock;
        private readonly Mock<ILogger<ChiNhanhsController>> _loggerMock;
        private readonly ChiNhanhsController _controller;

        public ChiNhanhsControllerTests()
        {
            _chiNhanhServiceMock = new Mock<IChiNhanhService>();
            _loggerMock = new Mock<ILogger<ChiNhanhsController>>();
            _controller = new ChiNhanhsController(_loggerMock.Object, _chiNhanhServiceMock.Object);
        }

        [Fact]
        public async Task CreateChiNhanh_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateChiNhanhDto { TenChiNhanh = "Main Branch" };
            var expectedResult = new ChiNhanhDto { Id = 1, TenChiNhanh = "Main Branch" };
            _chiNhanhServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateChiNhanh(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ChiNhanhDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetChiNhanhById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new ChiNhanhDto { Id = id };
            _chiNhanhServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetChiNhanhById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ChiNhanhDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task GetAllChiNhanhs_Success_ReturnsOk()
        {
            // Arrange
            var pagedResult = new PagedResult<ChiNhanhDto> { Items = new List<ChiNhanhDto>() };
            _chiNhanhServiceMock.Setup(s => s.GetAllAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAllChiNhanhs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<ChiNhanhDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UpdateChiNhanh_Success_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateChiNhanhDto { TenChiNhanh = "Main Branch Updated" };

            // Act
            var result = await _controller.UpdateChiNhanh(id, dto);

            // Assert
            _chiNhanhServiceMock.Verify(s => s.UpdateAsync(id, dto), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteChiNhanh_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteChiNhanh(id);

            // Assert
            _chiNhanhServiceMock.Verify(s => s.SoftDeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
