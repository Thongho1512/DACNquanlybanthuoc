using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data;

public partial class ShopDbContext : DbContext
{
    public ShopDbContext()
    {
    }

    public ShopDbContext(DbContextOptions<ShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiNhanh> ChiNhanhs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonGiaoHang> DonGiaoHangs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonNhapHang> DonNhapHangs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhoHang> KhoHangs { get; set; }

    public virtual DbSet<LichSuDiem> LichSuDiems { get; set; }

    public virtual DbSet<LoHang> LoHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiGiaoHang> NguoiGiaoHangs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Thuoc> Thuocs { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<VwBaoCaoDoanhThu> VwBaoCaoDoanhThus { get; set; }

    public virtual DbSet<VwThongKeThuocBanChay> VwThongKeThuocBanChays { get; set; }

    public virtual DbSet<VwThongKeTonKho> VwThongKeTonKhos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-170JDGQ;Database=quanlybanthuoc;User Id=sa;Password=thaithong123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiNhanh__3214EC276A4EAE4E");

            entity.ToTable("ChiNhanh");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.TenChiNhanh).HasMaxLength(100);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3214EC274C1EB300");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.Idthuoc).HasColumnName("IDThuoc");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IddonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.IddonHang)
                .HasConstraintName("FK__ChiTietDo__IDDon__5535A963");

            entity.HasOne(d => d.IdthuocNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.Idthuoc)
                .HasConstraintName("FK__ChiTietDo__IDThu__5629CD9C");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhMuc__3214EC273AFD2730");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DonGiaoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonGiaoH__3214EC27327CC0D8");

            entity.ToTable("DonGiaoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(300);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.IdnguoiGiaoHang).HasColumnName("IDNguoiGiaoHang");
            entity.Property(e => e.LyDoHuy).HasMaxLength(500);
            entity.Property(e => e.NgayBatDauGiao).HasColumnType("datetime");
            entity.Property(e => e.NgayGiaoThanhCong).HasColumnType("datetime");
            entity.Property(e => e.NgayLayHang).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayXacNhan).HasColumnType("datetime");
            entity.Property(e => e.PhiGiaoHang)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoDienThoaiNguoiNhan).HasMaxLength(15);
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.TrangThaiGiaoHang).HasMaxLength(20);

            entity.HasOne(d => d.IddonHangNavigation).WithMany(p => p.DonGiaoHangs)
                .HasForeignKey(d => d.IddonHang)
                .HasConstraintName("FK_DonGiaoHang_DonHang");

            entity.HasOne(d => d.IdnguoiGiaoHangNavigation).WithMany(p => p.DonGiaoHangs)
                .HasForeignKey(d => d.IdnguoiGiaoHang)
                .HasConstraintName("FK_DonGiaoHang_NguoiGiaoHang");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonHang__3214EC277750CAF3");

            entity.ToTable("DonHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdkhachHang).HasColumnName("IDKhachHang");
            entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");
            entity.Property(e => e.IdphuongThucTt).HasColumnName("IDPhuongThucTT");
            entity.Property(e => e.LoaiDonHang)
                .HasMaxLength(20)
                .HasDefaultValue("TAI_CHO");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(50)
                .HasDefaultValue("PENDING_PAYMENT");
            entity.Property(e => e.MomoOrderId).HasMaxLength(100);
            entity.Property(e => e.MomoTransactionId).HasMaxLength(100);
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienGiamGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__DonHang__IDChiNh__5812160E");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdkhachHang)
                .HasConstraintName("FK__DonHang__IDKhach__59063A47");

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdnguoiDung)
                .HasConstraintName("FK__DonHang__IDNguoi__59FA5E80");

            entity.HasOne(d => d.IdphuongThucTtNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdphuongThucTt)
                .HasConstraintName("FK__DonHang__IDPhuon__5AEE82B9");
        });

        modelBuilder.Entity<DonNhapHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonNhapH__3214EC2796C33929");

            entity.ToTable("DonNhapHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdnguoiNhan).HasColumnName("IDNguoiNhan");
            entity.Property(e => e.IdnhaCungCap).HasColumnName("IDNhaCungCap");
            entity.Property(e => e.SoDonNhap).HasMaxLength(50);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.DonNhapHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__DonNhapHa__IDChi__5BE2A6F2");

            entity.HasOne(d => d.IdnguoiNhanNavigation).WithMany(p => p.DonNhapHangs)
                .HasForeignKey(d => d.IdnguoiNhan)
                .HasConstraintName("FK__DonNhapHa__IDNgu__5CD6CB2B");

            entity.HasOne(d => d.IdnhaCungCapNavigation).WithMany(p => p.DonNhapHangs)
                .HasForeignKey(d => d.IdnhaCungCap)
                .HasConstraintName("FK__DonNhapHa__IDNha__5DCAEF64");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhachHan__3214EC27D33ED7C9");

            entity.ToTable("KhachHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TenKhachHang).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
        });

        modelBuilder.Entity<KhoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhoHang__3214EC27B0C42972");

            entity.ToTable("KhoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdloHang).HasColumnName("IDLoHang");

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.KhoHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__KhoHang__IDChiNh__5EBF139D");

            entity.HasOne(d => d.IdloHangNavigation).WithMany(p => p.KhoHangs)
                .HasForeignKey(d => d.IdloHang)
                .HasConstraintName("FK__KhoHang__IDLoHan__5FB337D6");
        });

        modelBuilder.Entity<LichSuDiem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichSuDi__3214EC27515C334A");

            entity.ToTable("LichSuDiem");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.IdkhachHang).HasColumnName("IDKhachHang");

            entity.HasOne(d => d.IddonHangNavigation).WithMany(p => p.LichSuDiems)
                .HasForeignKey(d => d.IddonHang)
                .HasConstraintName("FK__LichSuDie__IDDon__60A75C0F");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.LichSuDiems)
                .HasForeignKey(d => d.IdkhachHang)
                .HasConstraintName("FK__LichSuDie__IDKha__619B8048");
        });

        modelBuilder.Entity<LoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoHang__3214EC27E2F8B59C");

            entity.ToTable("LoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IddonNhapHang).HasColumnName("IDDonNhapHang");
            entity.Property(e => e.Idthuoc).HasColumnName("IDThuoc");
            entity.Property(e => e.SoLo).HasMaxLength(50);

            entity.HasOne(d => d.IddonNhapHangNavigation).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.IddonNhapHang)
                .HasConstraintName("FK__LoHang__IDDonNha__628FA481");

            entity.HasOne(d => d.IdthuocNavigation).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.Idthuoc)
                .HasConstraintName("FK__LoHang__IDThuoc__6383C8BA");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC27F63D204A");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__55F68FC08881EDE8").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdvaiTro).HasColumnName("IDVaiTro");
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__NguoiDung__IDChi__6477ECF3");

            entity.HasOne(d => d.IdvaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdvaiTro)
                .HasConstraintName("FK__NguoiDung__IDVai__656C112C");
        });

        modelBuilder.Entity<NguoiGiaoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiGia__3214EC2700EFEB1C");

            entity.ToTable("NguoiGiaoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BienSoXe).HasMaxLength(20);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.NguoiGiaoHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK_NguoiGiaoHang_ChiNhanh");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhaCungC__3214EC27109C5C47");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(100);
        });

        modelBuilder.Entity<PhuongThucThanhToan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhuongTh__3214EC2792E6D7AF");

            entity.ToTable("PhuongThucThanhToan");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TenPhuongThuc).HasMaxLength(50);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07F3D5165D");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Token).HasMaxLength(500);
        });

        modelBuilder.Entity<Thuoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Thuoc__3214EC27B61315F8");

            entity.ToTable("Thuoc");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HoatChat).HasMaxLength(100);
            entity.Property(e => e.IddanhMuc).HasColumnName("IDDanhMuc");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenThuoc).HasMaxLength(100);
            entity.Property(e => e.HinhAnh).HasMaxLength(500);

            entity.HasOne(d => d.IddanhMucNavigation).WithMany(p => p.Thuocs)
                .HasForeignKey(d => d.IddanhMuc)
                .HasConstraintName("FK__Thuoc__IDDanhMuc__6754599E");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VaiTro__3214EC275737342A");

            entity.ToTable("VaiTro");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<VwBaoCaoDoanhThu>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_BaoCaoDoanhThu");

            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdkhachHang).HasColumnName("IDKhachHang");
            entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");
            entity.Property(e => e.IdphuongThucTt).HasColumnName("IDPhuongThucTT");
            entity.Property(e => e.TenChiNhanh).HasMaxLength(100);
            entity.Property(e => e.TenKhachHang).HasMaxLength(100);
            entity.Property(e => e.TenNhanVien).HasMaxLength(100);
            entity.Property(e => e.TenPhuongThuc).HasMaxLength(50);
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienGiamGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<VwThongKeThuocBanChay>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ThongKeThuocBanChay");

            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTrungBinh).HasColumnType("decimal(38, 6)");
            entity.Property(e => e.HoatChat).HasMaxLength(100);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
            entity.Property(e => e.TenThuoc).HasMaxLength(100);
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwThongKeTonKho>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ThongKeTonKho");

            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaTriTonKho).HasColumnType("decimal(29, 2)");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.Idthuoc).HasColumnName("IDThuoc");
            entity.Property(e => e.SoLo).HasMaxLength(50);
            entity.Property(e => e.TenChiNhanh).HasMaxLength(100);
            entity.Property(e => e.TenThuoc).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
