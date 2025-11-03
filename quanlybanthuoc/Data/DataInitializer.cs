using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Helpers;
using System.Threading.Tasks;

namespace quanlybanthuoc.Data
{
    public class DataInitializer
    {
        public static async Task SeedData(ShopDbContext context)
        {
            // Đảm bảo DB được tạo
            context.Database.EnsureCreated();

            // create default roles
            if (!context.VaiTros.Any())
            {
                var roles = new List<VaiTro>
                {
                    new VaiTro{ TenVaiTro = "ADMIN", TrangThai = true},
                    new VaiTro{ TenVaiTro = "USER", TrangThai=true}
                };

                await context.VaiTros.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            var vaiTro = context.VaiTros.FirstOrDefault(vaiTro => vaiTro.TenVaiTro == "ADMIN");
            // create default admin user
            if (vaiTro != null && !context.NguoiDungs.Any(nguoiDung => nguoiDung.IdvaiTro == vaiTro.Id))
            {
                var nguoiDung = new NguoiDung
                {
                    TenDangNhap = "admin1",
                    MatKhau = PasswordHelper.HashPassword("admin123"),
                    TrangThai = true,
                    IdvaiTro = vaiTro!.Id,
                    NgayTao = DateOnly.FromDateTime(DateTime.Now),
                    HoTen = "Administrator"
                };

                await context.NguoiDungs.AddAsync(nguoiDung);
                await context.SaveChangesAsync();
            }
        }
    }
}
