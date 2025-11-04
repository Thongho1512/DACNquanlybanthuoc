using Microsoft.IdentityModel.Tokens;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace quanlybanthuoc.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(NguoiDung nguoiDung)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            _logger.LogInformation($" id = {nguoiDung.Id.ToString()}");
            _logger.LogInformation($"ten dang nhap: {nguoiDung.TenDangNhap}");
            _logger.LogInformation($"ten vai tro: {nguoiDung.IdvaiTroNavigation?.TenVaiTro}");

            var tenVaiTro = getTenVaiTro(nguoiDung.IdvaiTro!.Value).Result;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
                new Claim(ClaimTypes.Name, nguoiDung.TenDangNhap!),
                new Claim(ClaimTypes.Role, tenVaiTro)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                signingCredentials: cred
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private  async Task<string> getTenVaiTro(int idVaiTro)
        {
            var vaiTro = await _unitOfWork.VaiTroRepository.GetByIdAsync(idVaiTro);
            return vaiTro!.TenVaiTro!;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) // use 'using' to ensure proper disposal because RandomNumberGenerator implements IDisposable
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
