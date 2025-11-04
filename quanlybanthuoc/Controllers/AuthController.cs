using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        public AuthController(ILogger<AuthController> logger, IAuthService authService, IConfiguration configuration)
        {
            _logger = logger;
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("Controller login called");
            var result = await _authService.LoginAsync(loginRequest);

            // gửi refreshtoken trong httpOnly cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true, // JS can't access
                Secure = true, // only sent over HTTPS
                SameSite = SameSiteMode.Strict, // not sent with cross-site requests
                Expires = DateTimeOffset.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            });

            var response = new LoginResponse
            {
                AccessToken = result.AccessToken,
                NguoiDungDto = result.NguoiDungDto
            };
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("Controller refresh token called");

            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                new UnauthorizedAccessException("Không tìm thấy refresh token");
            }

            var result = await _authService.RefreshTokenAsync(refreshToken!);

            // gửi refreshtoken trong httpOnly cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true, // JS can't access
                Secure = true, // only sent over HTTPS
                SameSite = SameSiteMode.Strict, // not sent with cross-site requests
                Expires = DateTimeOffset.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            });

            var response = new LoginResponse
            {
                AccessToken = result.AccessToken,
                NguoiDungDto = result.NguoiDungDto
            };
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Controller logout called");
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                new UnauthorizedAccessException("Không tìm thấy refresh token");
            }
            await _authService.LogoutAsync(refreshToken!);

            Response.Cookies.Delete("refreshToken");

            var result = ApiResponse<string>.SuccessResponse("Đăng xuất thành công.");
            return Ok(result);
        }
    }
}
