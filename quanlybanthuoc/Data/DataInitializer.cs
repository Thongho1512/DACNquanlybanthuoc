using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Helpers;

namespace quanlybanthuoc.Data
{
    public class DataInitializer
    {
        public static async Task SeedData(ShopDbContext context)
        {
            try
            {
                // Đảm bảo DB được tạo
                await context.Database.MigrateAsync();
                Console.WriteLine("✓ Database ensured created");

                // create default roles
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "ADMIN"))
                {
                    await context.VaiTros.AddAsync(new VaiTro { TenVaiTro = "ADMIN", TrangThai = true });
                }

                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "USER"))
                {
                    await context.VaiTros.AddAsync(new VaiTro { TenVaiTro = "USER", TrangThai = true });
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✓ Roles checked/created: ADMIN, USER");

                // Lấy vai trò ADMIN
                var vaiTro = await context.VaiTros
                    .FirstOrDefaultAsync(vt => vt.TenVaiTro == "ADMIN");

                if (vaiTro == null)
                {
                    Console.WriteLine("✗ ADMIN role not found!");
                    return;
                }

                // create default admin user
                if (!await context.NguoiDungs.AnyAsync(nd => nd.IdvaiTro == vaiTro.Id))
                {
                    var nguoiDung = new NguoiDung
                    {
                        TenDangNhap = "admin",
                        MatKhau = PasswordHelper.HashPassword("admin123"),
                        TrangThai = true,
                        IdvaiTro = vaiTro.Id,
                        NgayTao = DateOnly.FromDateTime(DateTime.Now),
                        HoTen = "Administrator"
                    };

                    await context.NguoiDungs.AddAsync(nguoiDung);
                    await context.SaveChangesAsync();
                    Console.WriteLine("✓ Admin user created");
                    Console.WriteLine("  Username: admin");
                    Console.WriteLine("  Password: admin123");
                }

                Console.WriteLine("========================================");
                Console.WriteLine("Data initialization completed!");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                throw;
            }
        }
    }
}