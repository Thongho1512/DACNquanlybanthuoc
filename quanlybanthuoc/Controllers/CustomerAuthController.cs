using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/auth/customer")]
    [ApiController]
    public class CustomerAuthController : ControllerBase
    {
        private readonly ILogger<CustomerAuthController> _logger;
        private readonly ICustomerAuthService _customerAuthService;

        public CustomerAuthController(
            ILogger<CustomerAuthController> logger,
            ICustomerAuthService customerAuthService)
        {
            _logger = logger;
            _customerAuthService = customerAuthService;
        }

        /// <summary>
        /// Đăng nhập khách hàng bằng SDT + OTP
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request)
        {
            _logger.LogInformation($"Customer login: {request.Sdt}");

            try
            {
                var result = await _customerAuthService.LoginAsync(request);
                return Ok(ApiResponse<CustomerLoginResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in customer login: {request.Sdt}");
                return BadRequest(ApiResponse<CustomerLoginResponse>.FailureResponse(ex.Message));
            }
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterRequest request)
        {
            _logger.LogInformation($"Customer registration: {request.TenKhachHang}, SDT: {request.Sdt}");

            try
            {
                var result = await _customerAuthService.RegisterAsync(request);
                return Ok(ApiResponse<CustomerLoginResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in customer registration: {request.Sdt}");
                return BadRequest(ApiResponse<CustomerLoginResponse>.FailureResponse(ex.Message));
            }
        }

        /// <summary>
        /// Gửi OTP đến số điện thoại
        /// </summary>
        [HttpPost("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            _logger.LogInformation($"Sending OTP to: {request.Sdt}");

            try
            {
                var otp = await _customerAuthService.SendOtpAsync(request.Sdt);
                // Trong production, không nên trả về OTP
                // Chỉ trả về success message
                return Ok(ApiResponse<string>.SuccessResponse("OTP đã được gửi đến số điện thoại của bạn."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending OTP to: {request.Sdt}");
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }

        /// <summary>
        /// Xác thực OTP
        /// </summary>
        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            _logger.LogInformation($"Verifying OTP for: {request.Sdt}");

            try
            {
                var isValid = await _customerAuthService.VerifyOtpAsync(request.Sdt, request.Otp);
                if (isValid)
                {
                    return Ok(ApiResponse<string>.SuccessResponse("OTP hợp lệ."));
                }
                else
                {
                    return BadRequest(ApiResponse<string>.FailureResponse("OTP không hợp lệ hoặc đã hết hạn."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying OTP for: {request.Sdt}");
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }
    }
}

