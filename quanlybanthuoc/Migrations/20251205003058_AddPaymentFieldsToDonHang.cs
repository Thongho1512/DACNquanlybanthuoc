using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlybanthuoc.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToDonHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiNhanh",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChiNhanh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiNhanh__3214EC276A4EAE4E", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhMuc__3214EC273AFD2730", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhachHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DiemTichLuy = table.Column<int>(type: "int", nullable: true),
                    NgayDangKy = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__3214EC27D33ED7C9", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhaCungCap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhaCungC__3214EC27109C5C47", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PhuongThucThanhToan",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhuongTh__3214EC2792E6D7AF", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaiTro__3214EC275737342A", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NguoiGiaoHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BienSoXe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IDChiNhanh = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiGia__3214EC2700EFEB1C", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NguoiGiaoHang_ChiNhanh",
                        column: x => x.IDChiNhanh,
                        principalTable: "ChiNhanh",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDDanhMuc = table.Column<int>(type: "int", nullable: true),
                    TenThuoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HoatChat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DonVi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Thuoc__3214EC27B61315F8", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Thuoc__IDDanhMuc__6754599E",
                        column: x => x.IDDanhMuc,
                        principalTable: "DanhMuc",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDVaiTro = table.Column<int>(type: "int", nullable: true),
                    IDChiNhanh = table.Column<int>(type: "int", nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__3214EC27F63D204A", x => x.ID);
                    table.ForeignKey(
                        name: "FK__NguoiDung__IDChi__6477ECF3",
                        column: x => x.IDChiNhanh,
                        principalTable: "ChiNhanh",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__NguoiDung__IDVai__656C112C",
                        column: x => x.IDVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDNguoiDung = table.Column<int>(type: "int", nullable: true),
                    IDKhachHang = table.Column<int>(type: "int", nullable: true),
                    IDChiNhanh = table.Column<int>(type: "int", nullable: true),
                    IDPhuongThucTT = table.Column<int>(type: "int", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TienGiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: true),
                    LoaiDonHang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "TAI_CHO"),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "PENDING_PAYMENT"),
                    MomoOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MomoTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHang__3214EC277750CAF3", x => x.ID);
                    table.ForeignKey(
                        name: "FK__DonHang__IDChiNh__5812160E",
                        column: x => x.IDChiNhanh,
                        principalTable: "ChiNhanh",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__DonHang__IDKhach__59063A47",
                        column: x => x.IDKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__DonHang__IDNguoi__59FA5E80",
                        column: x => x.IDNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__DonHang__IDPhuon__5AEE82B9",
                        column: x => x.IDPhuongThucTT,
                        principalTable: "PhuongThucThanhToan",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DonNhapHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChiNhanh = table.Column<int>(type: "int", nullable: true),
                    IDNhaCungCap = table.Column<int>(type: "int", nullable: true),
                    IDNguoiNhan = table.Column<int>(type: "int", nullable: true),
                    SoDonNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayNhap = table.Column<DateOnly>(type: "date", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonNhapH__3214EC2796C33929", x => x.ID);
                    table.ForeignKey(
                        name: "FK__DonNhapHa__IDChi__5BE2A6F2",
                        column: x => x.IDChiNhanh,
                        principalTable: "ChiNhanh",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__DonNhapHa__IDNgu__5CD6CB2B",
                        column: x => x.IDNguoiNhan,
                        principalTable: "NguoiDung",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__DonNhapHa__IDNha__5DCAEF64",
                        column: x => x.IDNhaCungCap,
                        principalTable: "NhaCungCap",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNguoiDung = table.Column<int>(type: "int", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayThuHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaHetHan = table.Column<bool>(type: "bit", nullable: true),
                    BiThuHoi = table.Column<bool>(type: "bit", nullable: true),
                    ConHieuLuc = table.Column<bool>(type: "bit", nullable: true),
                    NguoiDungId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RefreshT__3214EC07F3D5165D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDDonHang = table.Column<int>(type: "int", nullable: true),
                    IDThuoc = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__3214EC274C1EB300", x => x.ID);
                    table.ForeignKey(
                        name: "FK__ChiTietDo__IDDon__5535A963",
                        column: x => x.IDDonHang,
                        principalTable: "DonHang",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__ChiTietDo__IDThu__5629CD9C",
                        column: x => x.IDThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DonGiaoHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDDonHang = table.Column<int>(type: "int", nullable: false),
                    IDNguoiGiaoHang = table.Column<int>(type: "int", nullable: true),
                    TrangThaiGiaoHang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChiGiaoHang = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SoDienThoaiNguoiNhan = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhiGiaoHang = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    NgayXacNhan = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayLayHang = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayBatDauGiao = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayGiaoThanhCong = table.Column<DateTime>(type: "datetime", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LyDoHuy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonGiaoH__3214EC27327CC0D8", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DonGiaoHang_DonHang",
                        column: x => x.IDDonHang,
                        principalTable: "DonHang",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonGiaoHang_NguoiGiaoHang",
                        column: x => x.IDNguoiGiaoHang,
                        principalTable: "NguoiGiaoHang",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LichSuDiem",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDKhachHang = table.Column<int>(type: "int", nullable: true),
                    IDDonHang = table.Column<int>(type: "int", nullable: true),
                    DiemCong = table.Column<int>(type: "int", nullable: true),
                    DiemTru = table.Column<int>(type: "int", nullable: true),
                    NgayGiaoDich = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LichSuDi__3214EC27515C334A", x => x.ID);
                    table.ForeignKey(
                        name: "FK__LichSuDie__IDDon__60A75C0F",
                        column: x => x.IDDonHang,
                        principalTable: "DonHang",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__LichSuDie__IDKha__619B8048",
                        column: x => x.IDKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LoHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDDonNhapHang = table.Column<int>(type: "int", nullable: true),
                    IDThuoc = table.Column<int>(type: "int", nullable: true),
                    SoLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgaySanXuat = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayHetHan = table.Column<DateOnly>(type: "date", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoHang__3214EC27E2F8B59C", x => x.ID);
                    table.ForeignKey(
                        name: "FK__LoHang__IDDonNha__628FA481",
                        column: x => x.IDDonNhapHang,
                        principalTable: "DonNhapHang",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__LoHang__IDThuoc__6383C8BA",
                        column: x => x.IDThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "KhoHang",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChiNhanh = table.Column<int>(type: "int", nullable: true),
                    IDLoHang = table.Column<int>(type: "int", nullable: true),
                    TonKhoToiThieu = table.Column<int>(type: "int", nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true),
                    NgayCapNhat = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhoHang__3214EC27B0C42972", x => x.ID);
                    table.ForeignKey(
                        name: "FK__KhoHang__IDChiNh__5EBF139D",
                        column: x => x.IDChiNhanh,
                        principalTable: "ChiNhanh",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__KhoHang__IDLoHan__5FB337D6",
                        column: x => x.IDLoHang,
                        principalTable: "LoHang",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_IDDonHang",
                table: "ChiTietDonHang",
                column: "IDDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_IDThuoc",
                table: "ChiTietDonHang",
                column: "IDThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_DonGiaoHang_IDDonHang",
                table: "DonGiaoHang",
                column: "IDDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonGiaoHang_IDNguoiGiaoHang",
                table: "DonGiaoHang",
                column: "IDNguoiGiaoHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDChiNhanh",
                table: "DonHang",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDKhachHang",
                table: "DonHang",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDNguoiDung",
                table: "DonHang",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDPhuongThucTT",
                table: "DonHang",
                column: "IDPhuongThucTT");

            migrationBuilder.CreateIndex(
                name: "IX_DonNhapHang_IDChiNhanh",
                table: "DonNhapHang",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_DonNhapHang_IDNguoiNhan",
                table: "DonNhapHang",
                column: "IDNguoiNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DonNhapHang_IDNhaCungCap",
                table: "DonNhapHang",
                column: "IDNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_KhoHang_IDChiNhanh",
                table: "KhoHang",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_KhoHang_IDLoHang",
                table: "KhoHang",
                column: "IDLoHang");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiem_IDDonHang",
                table: "LichSuDiem",
                column: "IDDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiem_IDKhachHang",
                table: "LichSuDiem",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_LoHang_IDDonNhapHang",
                table: "LoHang",
                column: "IDDonNhapHang");

            migrationBuilder.CreateIndex(
                name: "IX_LoHang_IDThuoc",
                table: "LoHang",
                column: "IDThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_IDChiNhanh",
                table: "NguoiDung",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_IDVaiTro",
                table: "NguoiDung",
                column: "IDVaiTro");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__55F68FC08881EDE8",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true,
                filter: "[TenDangNhap] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiGiaoHang_IDChiNhanh",
                table: "NguoiGiaoHang",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_NguoiDungId",
                table: "RefreshToken",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_IDDanhMuc",
                table: "Thuoc",
                column: "IDDanhMuc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "DonGiaoHang");

            migrationBuilder.DropTable(
                name: "KhoHang");

            migrationBuilder.DropTable(
                name: "LichSuDiem");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "NguoiGiaoHang");

            migrationBuilder.DropTable(
                name: "LoHang");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "DonNhapHang");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "PhuongThucThanhToan");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "DanhMuc");

            migrationBuilder.DropTable(
                name: "ChiNhanh");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
