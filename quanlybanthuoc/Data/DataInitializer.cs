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
                //await context.Database.MigrateAsync();
                Console.WriteLine("✓ Database ensured created");

                // ===================================================
                // TẠO 4 VAI TRÒ THEO TÀI LIỆU
                // ===================================================

                // 1. ADMIN - Quản trị viên (quyền cao nhất)
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "ADMIN"))
                {
                    await context.VaiTros.AddAsync(new VaiTro
                    {
                        TenVaiTro = "ADMIN",
                        MoTa = "Quản trị viên hệ thống - Quyền cao nhất, quản lý toàn bộ hệ thống",
                        TrangThai = true
                    });
                }

                // 2. MANAGER - Quản lý chuỗi
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "MANAGER"))
                {
                    await context.VaiTros.AddAsync(new VaiTro
                    {
                        TenVaiTro = "MANAGER",
                        MoTa = "Quản lý chuỗi - Giám sát hoạt động kinh doanh tổng thể, xem báo cáo tổng hợp",
                        TrangThai = true
                    });
                }

                // 3. STAFF - Nhân viên bán hàng
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "STAFF"))
                {
                    await context.VaiTros.AddAsync(new VaiTro
                    {
                        TenVaiTro = "STAFF",
                        MoTa = "Nhân viên bán hàng - Thực hiện giao dịch bán hàng, tư vấn khách hàng, xử lý thanh toán",
                        TrangThai = true
                    });
                }

                // 4. WAREHOUSE_STAFF - Nhân viên kho
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "WAREHOUSE_STAFF"))
                {
                    await context.VaiTros.AddAsync(new VaiTro
                    {
                        TenVaiTro = "WAREHOUSE_STAFF",
                        MoTa = "Nhân viên kho - Nhập hàng, xuất hàng, theo dõi hạn sử dụng, kiểm kê",
                        TrangThai = true
                    });
                }

                await context.SaveChangesAsync();
                Console.WriteLine("✓ Roles checked/created: ADMIN, MANAGER, STAFF, WAREHOUSE_STAFF");

                // ===================================================
                // TẠO TÀI KHOẢN ADMIN MẶC ĐỊNH
                // ===================================================
                var adminRole = await context.VaiTros
                    .FirstOrDefaultAsync(vt => vt.TenVaiTro == "ADMIN");

                if (adminRole == null)
                {
                    Console.WriteLine("✗ ADMIN role not found!");
                    return;
                }

                // Tạo admin user nếu chưa tồn tại
                if (!await context.NguoiDungs.AnyAsync(nd => nd.IdvaiTro == adminRole.Id))
                {
                    var adminUser = new NguoiDung
                    {
                        TenDangNhap = "admin",
                        MatKhau = PasswordHelper.HashPassword("admin123"),
                        TrangThai = true,
                        IdvaiTro = adminRole.Id,
                        NgayTao = DateOnly.FromDateTime(DateTime.Now),
                        HoTen = "Administrator",
                        IdchiNhanh = null // Admin không thuộc chi nhánh cụ thể
                    };

                    await context.NguoiDungs.AddAsync(adminUser);
                    await context.SaveChangesAsync();

                    Console.WriteLine("✓ Admin user created");
                    Console.WriteLine("  Username: admin");
                    Console.WriteLine("  Password: admin123");
                    Console.WriteLine("  Role: ADMIN");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                throw;
            }
        }
    }
}