using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Tests.Controllers
{
    public class ThuocsControllerTests
    {
        private readonly Mock<IThuocService> _thuocServiceMock;
        private readonly Mock<ILogger<ThuocsController>> _loggerMock;
        private readonly ThuocsController _controller;

        public ThuocsControllerTests()
        {
            _thuocServiceMock = new Mock<IThuocService>();
            _loggerMock = new Mock<ILogger<ThuocsController>>();
            _controller = new ThuocsController(_loggerMock.Object, _thuocServiceMock.Object);
        }

        [Fact]
        public async Task CreateThuoc_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateThuocDto { TenThuoc = "Paracetamol" };
            var expectedResult = new ThuocDto { Id = 1, TenThuoc = "Paracetamol" };
            _thuocServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateThuoc(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ThuocDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetAllThuocs_Success_ReturnsOk()
        {
            // Arrange
            var pagedResult = new PagedResult<ThuocDto> { Items = new List<ThuocDto>() };
            _thuocServiceMock.Setup(s => s.GetAllAsync(1, 10, true, null, null))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAllThuocs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<ThuocDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task GetThuocById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new ThuocDto { Id = id, TenThuoc = "Paracetamol" };
            _thuocServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetThuocById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ThuocDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(id, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task UpdateThuoc_Success_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateThuocDto { TenThuoc = "Paracetamol Updated" };

            // Act
            var result = await _controller.UpdateThuoc(id, dto);

            // Assert
            _thuocServiceMock.Verify(s => s.UpdateAsync(id, dto), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteThuoc_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteThuoc(id);

            // Assert
            _thuocServiceMock.Verify(s => s.SoftDeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
