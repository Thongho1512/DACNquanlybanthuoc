using quanlybanthuoc.Dtos.NguoiDung;
using System.Text.Json.Serialization;

namespace quanlybanthuoc.Dtos.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
        public NguoiDungDto NguoiDungDto { get; set; } = new NguoiDungDto();

    }
}
