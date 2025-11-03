using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(NguoiDung nguoiDung);
        string GenerateRefreshToken();
    }
}
