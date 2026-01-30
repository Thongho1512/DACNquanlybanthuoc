using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;

namespace quanlybanthuoc.Services.Impl
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerAuthService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

        public CustomerAuthService(
            IUnitOfWork unitOfWork,
            ILogger<CustomerAuthService> logger,
            IConfiguration configuration,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<CustomerLoginResponse> LoginAsync(CustomerLoginRequest request)
        {
            _logger.LogInformation($"Customer login attempt with SDT: {request.Sdt}");

            // Tìm khách hàng theo SDT
            var khachHang = await _unitOfWork.KhachHangRepository.GetBySdtAsync(request.Sdt);
            
            if (khachHang == null || khachHang.TrangThai == false)
            {
                throw new NotFoundException("Số điện thoại chưa được đăng ký hoặc tài khoản đã bị khóa.");
            }

            // Xác thực mật khẩu hoặc OTP
            bool isAuthenticated = false;

            // Nếu có mật khẩu, verify mật khẩu
            if (!string.IsNullOrEmpty(request.MatKhau))
            {
                if (string.IsNullOrEmpty(khachHang.MatKhau))
                {
                    throw new UnauthorizedException("Tài khoản chưa được thiết lập mật khẩu. Vui lòng đăng ký lại.");
                }
                isAuthenticated = VerifyPassword(request.MatKhau, khachHang.MatKhau);
            }
            // Nếu có OTP, verify OTP (tương thích với hệ thống cũ)
            else if (!string.IsNullOrEmpty(request.Otp))
            {
                isAuthenticated = VerifyOtp(request.Sdt, request.Otp);
            }
            else
            {
                throw new BadRequestException("Vui lòng nhập mật khẩu hoặc OTP.");
            }

            if (!isAuthenticated)
            {
                throw new UnauthorizedException("Mật khẩu hoặc OTP không đúng.");
            }

            // Tạo token cho khách hàng
            var accessToken = GenerateCustomerToken(khachHang);
            var khachHangDto = _mapper.Map<KhachHangDto>(khachHang);

            _logger.LogInformation($"Customer {khachHang.Id} logged in successfully");

            return new CustomerLoginResponse
            {
                AccessToken = accessToken,
                KhachHangDto = khachHangDto
            };
        }

        public async Task<CustomerLoginResponse> RegisterAsync(CustomerRegisterRequest request)
        {
            _logger.LogInformation($"Customer registration: {request.TenKhachHang}, SDT: {request.Sdt}");

            // Kiểm tra SDT đã tồn tại chưa
            var existingKhachHang = await _unitOfWork.KhachHangRepository.GetBySdtAsync(request.Sdt);
            if (existingKhachHang != null)
            {
                throw new BadRequestException("Số điện thoại này đã được đăng ký. Vui lòng đăng nhập.");
            }

            // Validate mật khẩu
            if (string.IsNullOrWhiteSpace(request.MatKhau) || request.MatKhau.Length < 6)
            {
                throw new BadRequestException("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            // Hash mật khẩu
            var hashedPassword = PasswordHelper.HashPassword(request.MatKhau);

            // Tạo khách hàng mới
            var khachHang = new KhachHang
            {
                TenKhachHang = request.TenKhachHang,
                Sdt = request.Sdt,
                MatKhau = hashedPassword,
                DiemTichLuy = 0,
                NgayDangKy = DateOnly.FromDateTime(DateTime.Now),
                TrangThai = true
            };

            await _unitOfWork.KhachHangRepository.CreateAsync(khachHang);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Customer registered successfully with ID: {khachHang.Id}");

            // Tự động đăng nhập sau khi đăng ký
            var accessToken = GenerateCustomerToken(khachHang);
            var khachHangDto = _mapper.Map<KhachHangDto>(khachHang);

            return new CustomerLoginResponse
            {
                AccessToken = accessToken,
                KhachHangDto = khachHangDto
            };
        }

        public async Task<string> SendOtpAsync(string sdt)
        {
            _logger.LogInformation($"Sending OTP to SDT: {sdt}");

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.Now.AddMinutes(5); // OTP hết hạn sau 5 phút

            // Store OTP (trong production nên dùng Redis hoặc database)
            _otpStore[sdt] = (otp, expiry);

            // TODO: Gửi OTP qua SMS service (Twilio, AWS SNS, etc.)
            // Hiện tại chỉ log ra console
            _logger.LogInformation($"OTP for {sdt}: {otp} (Expires at {expiry})");

            // Trong production, gửi SMS thật:
            // await _smsService.SendSmsAsync(sdt, $"Ma OTP cua ban la: {otp}. Co hieu luc trong 5 phut.");

            return otp; // Trả về OTP để test (trong production không nên trả về)
        }

        public Task<bool> VerifyOtpAsync(string sdt, string otp)
        {
            return Task.FromResult(VerifyOtp(sdt, otp));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        private bool VerifyOtp(string sdt, string otp)
        {
            if (!_otpStore.ContainsKey(sdt))
            {
                return false;
            }

            var (storedOtp, expiry) = _otpStore[sdt];

            if (DateTime.Now > expiry)
            {
                _otpStore.Remove(sdt);
                return false;
            }

            if (storedOtp != otp)
            {
                return false;
            }

            // Xóa OTP sau khi verify thành công
            _otpStore.Remove(sdt);
            return true;
        }

        private string GenerateCustomerToken(KhachHang khachHang)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("CustomerId", khachHang.Id.ToString()),
                new Claim("Sdt", khachHang.Sdt ?? ""),
                new Claim(ClaimTypes.Name, khachHang.TenKhachHang ?? ""),
                new Claim(ClaimTypes.Role, "CUSTOMER")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24), // Token hết hạn sau 24 giờ
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

