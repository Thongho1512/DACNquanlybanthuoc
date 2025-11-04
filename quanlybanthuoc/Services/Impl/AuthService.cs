using AutoMapper;
using Azure.Core;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Middleware.Exceptions;
using System.Globalization;

namespace quanlybanthuoc.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, ITokenService tokenService, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
            _mapper = mapper;
            _configuration = configuration;
        }

        
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("get user's information");
            // check login info
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByTenDangNhapAsync(request.TenDangNhap);
            if (nguoiDung == null || !VerifyPassword(request.MatKhau, nguoiDung.MatKhau!) || nguoiDung.TrangThai == false)
            {
                throw new NotFoundException("Thông tin đăng nhập không hợp lệ.");
            }
            Console.WriteLine("User found and password verified");

            var accessToken = _tokenService.GenerateAccessToken(nguoiDung);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var nguoiDungDto = _mapper.Map<NguoiDungDto>(nguoiDung);

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                IdNguoiDung = nguoiDung.Id,
                NgayTao = DateTime.Now,
                NgayHetHan = DateTime.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!)),
                
            };

            await _unitOfWork.RefreshTokenRepository.CreateAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                NguoiDungDto = nguoiDungDto
            };
        }

        public async Task LogoutAsync(string request)
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request);
            if(token == null)
            {
                throw new NotFoundException("Refresh token không hợp lệ.");
            }
            if(token.ConHieuLuc == true)
            {
                token.NgayThuHoi = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Thu hồi refresh token thành công.");
            }
            
        }

        public async Task<LoginResponse> RefreshTokenAsync(string request)
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request);
            if(token == null)
            {
                throw new NotFoundException("Refresh token không hợp lệ.");
            }
            if(token.ConHieuLuc == false)
            {
                throw new UnauthorizedException("Refresh token đã hết hạn hoặc bị thu hồi.");
            }

            if(token.NgayHetHan < DateTime.Now)
            {
                throw new UnauthorizedException("Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.");
            }

            var nguoiDung = token.NguoiDung;

            await _unitOfWork.BeginTransactionAsync();
            var accessToken = _tokenService.GenerateAccessToken(nguoiDung!);
            var refreshToken = _tokenService.GenerateRefreshToken();
            try
            {
                token.NgayThuHoi = DateTime.Now;
                await _unitOfWork.RefreshTokenRepository.CreateAsync(new RefreshToken
                {
                    Token = refreshToken,
                    IdNguoiDung = token.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    NgayHetHan = DateTime.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
                });
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi làm mới refresh token.");
                throw new Exception("Lỗi khi làm mới refresh token.");
            }

            var nguoiDungDto = _mapper.Map<NguoiDungDto>(nguoiDung);


            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                NguoiDungDto = nguoiDungDto
            };
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
