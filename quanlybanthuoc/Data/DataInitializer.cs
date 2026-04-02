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
                // await context.Database.MigrateAsync();
                
                // SỬA LỖI THIẾU CỘT TRỰC TIẾP QUA SQL (NẾU CÓ)
                await context.Database.ExecuteSqlRawAsync(@"
                    -- Thêm cột HinhAnh cho Thuoc
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Thuoc') AND name = 'HinhAnh')
                        ALTER TABLE Thuoc ADD HinhAnh NVARCHAR(500) NULL;
                    
                    -- Thêm cột TrangThai cho Thuoc
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Thuoc') AND name = 'TrangThai')
                        ALTER TABLE Thuoc ADD TrangThai BIT DEFAULT 1;

                    -- Thêm cột TrangThai cho ChiNhanh
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ChiNhanh') AND name = 'TrangThai')
                        ALTER TABLE ChiNhanh ADD TrangThai BIT DEFAULT 1;
                ");

                Console.WriteLine("✓ Database columns ensured (HinhAnh, TrangThai)");

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
                // TẠO DỮ LIỆU MẪU CHO TEST (CHI NHÁNH & THUỐC)
                // ===================================================
                
                // 1. Đảm bảo Chi nhánh ID = 1 (Active)
                var cn1 = await context.ChiNhanhs.FirstOrDefaultAsync(cn => cn.TenChiNhanh == "Chi nhánh trung tâm (Test)");
                if (cn1 == null)
                {
                    cn1 = new ChiNhanh
                    {
                        TenChiNhanh = "Chi nhánh trung tâm (Test)",
                        DiaChi = "123 Test St",
                        TrangThai = true
                    };
                    await context.ChiNhanhs.AddAsync(cn1);
                }
                else
                {
                    cn1.TrangThai = true;
                }

                // 2. Đảm bảo Chi nhánh ID = 2 (Inactive - Cho TC2)
                var cn2 = await context.ChiNhanhs.FirstOrDefaultAsync(cn => cn.TenChiNhanh == "Chi nhánh tạm dừng (Test)");
                if (cn2 == null)
                {
                    cn2 = new ChiNhanh
                    {
                        TenChiNhanh = "Chi nhánh tạm dừng (Test)",
                        DiaChi = "456 Test St",
                        TrangThai = false
                    };
                    await context.ChiNhanhs.AddAsync(cn2);
                }
                else
                {
                    cn2.TrangThai = false;
                }

                // 3. Đảm bảo Thuốc ID = 1 (Active)
                var t1 = await context.Thuocs.FirstOrDefaultAsync(t => t.TenThuoc == "Paracetamol (Test)");
                if (t1 == null)
                {
                    t1 = new Thuoc
                    {
                        TenThuoc = "Paracetamol (Test)",
                        DonVi = "Viên",
                        GiaBan = 1000,
                        TrangThai = true
                    };
                    await context.Thuocs.AddAsync(t1);
                }
                else
                {
                    t1.TrangThai = true;
                }

                // 4. Đảm bảo Thuốc ID = 2 (Inactive - Cho TC4)
                var t2 = await context.Thuocs.FirstOrDefaultAsync(t => t.TenThuoc == "Thuốc ngừng bán (Test)");
                if (t2 == null)
                {
                    t2 = new Thuoc
                    {
                        TenThuoc = "Thuốc ngừng bán (Test)",
                        DonVi = "Chai",
                        GiaBan = 5000,
                        TrangThai = false
                    };
                    await context.Thuocs.AddAsync(t2);
                }
                else
                {
                    t2.TrangThai = false;
                }

                await context.SaveChangesAsync();
                Console.WriteLine($"✓ Test data seeded:");
                Console.WriteLine($"  - Branch 1 (Active): ID={cn1.Id}, Name='{cn1.TenChiNhanh}'");
                Console.WriteLine($"  - Branch 2 (Inactive): ID={cn2.Id}, Name='{cn2.TenChiNhanh}'");
                Console.WriteLine($"  - Medicine 1 (Active): ID={t1.Id}, Name='{t1.TenThuoc}'");
                Console.WriteLine($"  - Medicine 2 (Inactive): ID={t2.Id}, Name='{t2.TenThuoc}'");
                Console.WriteLine("✓ Test data (Branches & Medicines) seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                throw;
            }
        }
    }
}