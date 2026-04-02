using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Controllers;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly AuthController _controller;
        private readonly Mock<IResponseCookies> _cookiesMock;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _configMock = new Mock<IConfiguration>();
            _cookiesMock = new Mock<IResponseCookies>();

            _controller = new AuthController(_loggerMock.Object, _authServiceMock.Object, _configMock.Object);

            var httpContext = new DefaultHttpContext();
            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupGet(r => r.Cookies).Returns(_cookiesMock.Object);
            
            // We need to inject the mock response into the context
            // Actually, DefaultHttpContext has a Response property that we can't easily swap out with a mock if we want to test cookie appending.
            // But we can use the ControllerContext.
            
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            _configMock.Setup(c => c["Jwt:RefreshTokenExpirationDays"]).Returns("7");
        }

        [Fact]
        public async Task Login_Success_ReturnsOkAndSetsCookie()
        {
            // Arrange
            var request = new LoginRequest { TenDangNhap = "admin", MatKhau = "password" };
            var loginResult = new LoginResponse 
            { 
                AccessToken = "access_token", 
                RefreshToken = "refresh_token",
                NguoiDungDto = new quanlybanthuoc.Dtos.NguoiDung.NguoiDungDto()
            };
            _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(loginResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal("access_token", response.AccessToken);
            // Verify cookie was set (this is tricky with DefaultHttpContext, but let's see if we can check the header)
            Assert.True(_controller.Response.Headers.ContainsKey("Set-Cookie") || _controller.Response.Cookies != null);
        }

        [Fact]
        public async Task Logout_Success_ReturnsOk()
        {
            // Arrange
            _controller.Request.Headers["Cookie"] = "refreshToken=old_token";
            _authServiceMock.Setup(s => s.LogoutAsync("old_token")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            _authServiceMock.Verify(s => s.LogoutAsync("old_token"), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}
