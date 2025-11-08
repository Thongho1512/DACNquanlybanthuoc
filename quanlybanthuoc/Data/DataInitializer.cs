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

                // ===================================================
                // TẠO TÀI KHOẢN MẪU CHO CÁC VAI TRÒ KHÁC (Optional)
                // ===================================================
                var managerRole = await context.VaiTros
                    .FirstOrDefaultAsync(vt => vt.TenVaiTro == "MANAGER");

                if (managerRole != null && !await context.NguoiDungs.AnyAsync(nd => nd.IdvaiTro == managerRole.Id))
                {
                    var managerUser = new NguoiDung
                    {
                        TenDangNhap = "manager",
                        MatKhau = PasswordHelper.HashPassword("manager123"),
                        TrangThai = true,
                        IdvaiTro = managerRole.Id,
                        NgayTao = DateOnly.FromDateTime(DateTime.Now),
                        HoTen = "Quản Lý Chuỗi",
                        IdchiNhanh = null // Manager quản lý tất cả chi nhánh
                    };

                    await context.NguoiDungs.AddAsync(managerUser);
                    Console.WriteLine("✓ Manager user created (manager/manager123)");
                }

                var staffRole = await context.VaiTros
                    .FirstOrDefaultAsync(vt => vt.TenVaiTro == "STAFF");

                if (staffRole != null && !await context.NguoiDungs.AnyAsync(nd => nd.IdvaiTro == staffRole.Id))
                {
                    var staffUser = new NguoiDung
                    {
                        TenDangNhap = "staff",
                        MatKhau = PasswordHelper.HashPassword("staff123"),
                        TrangThai = true,
                        IdvaiTro = staffRole.Id,
                        NgayTao = DateOnly.FromDateTime(DateTime.Now),
                        HoTen = "Nhân Viên Bán Hàng",
                        IdchiNhanh = null // Gán vào chi nhánh 1 (cần tạo chi nhánh trước)
                    };

                    await context.NguoiDungs.AddAsync(staffUser);
                    Console.WriteLine("✓ Staff user created (staff/staff123)");
                }

                var warehouseRole = await context.VaiTros
                    .FirstOrDefaultAsync(vt => vt.TenVaiTro == "WAREHOUSE_STAFF");

                if (warehouseRole != null && !await context.NguoiDungs.AnyAsync(nd => nd.IdvaiTro == warehouseRole.Id))
                {
                    var warehouseUser = new NguoiDung
                    {
                        TenDangNhap = "warehouse",
                        MatKhau = PasswordHelper.HashPassword("warehouse123"),
                        TrangThai = true,
                        IdvaiTro = warehouseRole.Id,
                        NgayTao = DateOnly.FromDateTime(DateTime.Now),
                        HoTen = "Nhân Viên Kho",
                        IdchiNhanh = null // Gán vào chi nhánh 1
                    };

                    await context.NguoiDungs.AddAsync(warehouseUser);
                    Console.WriteLine("✓ Warehouse staff user created (warehouse/warehouse123)");
                }

                await context.SaveChangesAsync();

                Console.WriteLine("========================================");
                Console.WriteLine("Data initialization completed!");
                Console.WriteLine("4 Roles: ADMIN, MANAGER, STAFF, WAREHOUSE_STAFF");
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