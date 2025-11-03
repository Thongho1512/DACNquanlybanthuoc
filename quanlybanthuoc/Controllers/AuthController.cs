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
        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("Controller login called");
            var result = ApiResponse<LoginResponse>.SuccessResponse(await _authService.LoginAsync(loginRequest));
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            _logger.LogInformation("Controller refresh token called");
            var result = ApiResponse<LoginResponse>.SuccessResponse(await _authService.RefreshTokenAsync(refreshTokenRequest));
            return Ok(result);
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            _logger.LogInformation("Controller logout called");
            await _authService.LogoutAsync(refreshTokenRequest);
            var result = ApiResponse<string>.SuccessResponse("Đăng xuất thành công.");
            return Ok(result);
        }
    }
}
