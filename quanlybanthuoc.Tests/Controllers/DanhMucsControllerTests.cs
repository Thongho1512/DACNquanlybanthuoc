using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Tests.Controllers
{
    public class DanhMucsControllerTests
    {
        private readonly Mock<IDanhMucService> _danhMucServiceMock;
        private readonly Mock<ILogger<DanhMucsController>> _loggerMock;
        private readonly DanhMucsController _controller;

        public DanhMucsControllerTests()
        {
            _danhMucServiceMock = new Mock<IDanhMucService>();
            _loggerMock = new Mock<ILogger<DanhMucsController>>();
            _controller = new DanhMucsController(_loggerMock.Object, _danhMucServiceMock.Object);
        }

        [Fact]
        public async Task CreateDanhMuc_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateDanhMucDto { TenDanhMuc = "Antibiotics" };
            var expectedResult = new DanhMucDto { Id = 1, TenDanhMuc = "Antibiotics" };
            _danhMucServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateDanhMuc(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DanhMucDto>>(createdAtActionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data?.Id);
        }

        [Fact]
        public async Task GetDanhMucById_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var expectedResult = new DanhMucDto { Id = id };
            _danhMucServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDanhMucById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DanhMucDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task GetAllDanhMucs_Success_ReturnsOk()
        {
            // Arrange
            var expectedResult = new List<DanhMucDto> { new DanhMucDto { Id = 1 } };
            _danhMucServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetAllDanhMucs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<DanhMucDto>>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UpdateDanhMuc_Success_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateDanhMucDto { TenDanhMuc = "Updated" };

            // Act
            var result = await _controller.UpdateDanhMuc(id, dto);

            // Assert
            _danhMucServiceMock.Verify(s => s.UpdateAsync(id, dto), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDanhMuc_Success_ReturnsOk()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteDanhMuc(id);

            // Assert
            _danhMucServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
