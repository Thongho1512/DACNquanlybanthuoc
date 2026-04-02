using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.Auth;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services;
using quanlybanthuoc.Services.Impl;

namespace quanlybanthuoc.Tests.Services
{
    public class AuthServiceCFGTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;

        // Repositories
        private readonly Mock<INguoiDungRepository> _nguoiDungRepoMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;

        private readonly AuthService _service;

        public AuthServiceCFGTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _configurationMock = new Mock<IConfiguration>();

            _nguoiDungRepoMock = new Mock<INguoiDungRepository>();
            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();

            _unitOfWorkMock.Setup(u => u.NguoiDungRepository).Returns(_nguoiDungRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.RefreshTokenRepository).Returns(_refreshTokenRepoMock.Object);

            _service = new AuthService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _tokenServiceMock.Object,
                _mapperMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task TC1_LoginAsync_UsernameNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var request = new LoginRequest { TenDangNhap = "wrong_user", MatKhau = "any" };
            _nguoiDungRepoMock.Setup(r => r.GetByTenDangNhapAsync("wrong_user")).ReturnsAsync((NguoiDung)null);

            // Act & Assert
            await _service.Invoking(s => s.LoginAsync(request))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thông tin đăng nhập không hợp lệ.");
        }

        [Fact]
        public async Task TC2_LoginAsync_WrongPassword_ThrowsNotFoundException()
        {
            // Arrange
            var request = new LoginRequest { TenDangNhap = "user1", MatKhau = "wrong_password" };
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correct_password");
            
            var user = new NguoiDung { TenDangNhap = "user1", MatKhau = hashedPassword, TrangThai = true };
            _nguoiDungRepoMock.Setup(r => r.GetByTenDangNhapAsync("user1")).ReturnsAsync(user);

            // Act & Assert
            await _service.Invoking(s => s.LoginAsync(request))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thông tin đăng nhập không hợp lệ.");
        }

        [Fact]
        public async Task TC3_LoginAsync_AccountLocked_ThrowsNotFoundException()
        {
            // Arrange
            var request = new LoginRequest { TenDangNhap = "user1", MatKhau = "password" };
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            
            var user = new NguoiDung { TenDangNhap = "user1", MatKhau = hashedPassword, TrangThai = false }; // Locked
            _nguoiDungRepoMock.Setup(r => r.GetByTenDangNhapAsync("user1")).ReturnsAsync(user);

            // Act & Assert
            await _service.Invoking(s => s.LoginAsync(request))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Thông tin đăng nhập không hợp lệ.");
        }

        [Fact]
        public async Task TC4_LoginAsync_Success_ReturnsLoginResponse()
        {
            // Arrange
            var request = new LoginRequest { TenDangNhap = "user1", MatKhau = "password" };
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            
            var user = new NguoiDung { Id = 1, TenDangNhap = "user1", MatKhau = hashedPassword, TrangThai = true };
            _nguoiDungRepoMock.Setup(r => r.GetByTenDangNhapAsync("user1")).ReturnsAsync(user);
            
            _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns("access_token");
            _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("refresh_token");
            _configurationMock.Setup(c => c["Jwt:RefreshTokenExpirationDays"]).Returns("7");

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("access_token");
            result.RefreshToken.Should().Be("refresh_token");
            _refreshTokenRepoMock.Verify(r => r.CreateAsync(It.Is<RefreshToken>(rt => rt.Token == "refresh_token" && rt.IdNguoiDung == 1)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
