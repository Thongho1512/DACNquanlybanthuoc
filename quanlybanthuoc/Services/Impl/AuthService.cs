using Azure.Core;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Middleware.Exceptions;

namespace quanlybanthuoc.Services.Impl
{
    public class AuthService : IAuthService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly ILogger<AuthService> _logger;
        public readonly ITokenService _tokenService;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // check login info
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByTenDangNhapAsync(request.TenDangNhap);
            if (nguoiDung == null || !VerifyPassword(request.MatKhau, nguoiDung.MatKhau!))
            {
                throw new NotFoundException("Thông tin đăng nhập không hợp lệ.");
            }

            var accessToken = _tokenService.GenerateAccessToken(nguoiDung);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                IdNguoiDung = nguoiDung.Id,
                NgayTao = DateTime.Now,
                NgayHetHan = DateTime.Now.AddDays(7)
            };

            await _unitOfWork.RefreshTokenRepository.CreateAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TenDangNhap = nguoiDung.TenDangNhap!,
                VaiTro = nguoiDung.IdvaiTroNavigation?.TenVaiTro!
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.RefreshToken);
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

            await _unitOfWork.BeginTransactionAsync();
            var accessToken = _tokenService.GenerateAccessToken(token.NguoiDung!);
            var refreshToken = _tokenService.GenerateRefreshToken();
            try
            {
                token.NgayThuHoi = DateTime.Now;
                await _unitOfWork.RefreshTokenRepository.CreateAsync(new RefreshToken
                {
                    Token = refreshToken,
                    IdNguoiDung = token.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    NgayHetHan = DateTime.Now.AddDays(7)
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

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TenDangNhap = token.NguoiDung?.TenDangNhap!,
                VaiTro = token.NguoiDung?.IdvaiTroNavigation?.TenVaiTro!
            };
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
