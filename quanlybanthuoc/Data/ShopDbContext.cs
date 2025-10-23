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

    public virtual DbSet<ChiTietLoHang> ChiTietLoHangs { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonNhapHang> DonNhapHangs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhoHang> KhoHangs { get; set; }

    public virtual DbSet<LichSuDiem> LichSuDiems { get; set; }

    public virtual DbSet<LoHang> LoHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }

    public virtual DbSet<Thuoc> Thuocs { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-170JDGQ;Database=quanlybanthuoc;User Id=sa;Password=thaithong123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RefreshToken");
            entity.ToTable("RefreshToken");

            entity.HasOne(rt => rt.NguoiDung)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.IdNguoiDung)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_RefreshToken_NguoiDung");
        });

        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiNhanh__3214EC27D2BC06D3");

            entity.ToTable("ChiNhanh");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.TenChiNhanh).HasMaxLength(100);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3214EC27B4AC4877");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.Idthuoc).HasColumnName("IDThuoc");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IddonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.IddonHang)
                .HasConstraintName("FK__ChiTietDo__IDDon__5EBF139D");

            entity.HasOne(d => d.IdthuocNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.Idthuoc)
                .HasConstraintName("FK__ChiTietDo__IDThu__5FB337D6");
        });

        modelBuilder.Entity<ChiTietLoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietL__3214EC273172057E");

            entity.ToTable("ChiTietLoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiTietDh).HasColumnName("IDChiTietDH");
            entity.Property(e => e.IdkhoHang).HasColumnName("IDKhoHang");

            entity.HasOne(d => d.IdkhoHangNavigation).WithMany(p => p.ChiTietLoHangs)
                .HasForeignKey(d => d.IdkhoHang)
                .HasConstraintName("FK__ChiTietLo__IDKho__5629CD9C");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhMuc__3214EC27CA89E967");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonHang__3214EC273B0FFB89");

            entity.ToTable("DonHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdkhachHang).HasColumnName("IDKhachHang");
            entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");
            entity.Property(e => e.IdphuongThucTt).HasColumnName("IDPhuongThucTT");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienGiamGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__DonHang__IDChiNh__5AEE82B9");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdkhachHang)
                .HasConstraintName("FK__DonHang__IDKhach__59FA5E80");

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdnguoiDung)
                .HasConstraintName("FK__DonHang__IDNguoi__59063A47");

            entity.HasOne(d => d.IdphuongThucTtNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdphuongThucTt)
                .HasConstraintName("FK__DonHang__IDPhuon__5BE2A6F2");
        });

        modelBuilder.Entity<DonNhapHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonNhapH__3214EC27D7FDC980");

            entity.ToTable("DonNhapHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdnguoiNhan).HasColumnName("IDNguoiNhan");
            entity.Property(e => e.IdnhaCungCap).HasColumnName("IDNhaCungCap");
            entity.Property(e => e.SoDonNhap).HasMaxLength(50);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.DonNhapHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__DonNhapHa__IDChi__49C3F6B7");

            entity.HasOne(d => d.IdnguoiNhanNavigation).WithMany(p => p.DonNhapHangs)
                .HasForeignKey(d => d.IdnguoiNhan)
                .HasConstraintName("FK__DonNhapHa__IDNgu__4BAC3F29");

            entity.HasOne(d => d.IdnhaCungCapNavigation).WithMany(p => p.DonNhapHangs)
                .HasForeignKey(d => d.IdnhaCungCap)
                .HasConstraintName("FK__DonNhapHa__IDNha__4AB81AF0");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhachHan__3214EC277E2D73EF");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Sdt, "ix_khachhang_sdt");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
            entity.Property(e => e.TenKhachHang).HasMaxLength(100);
        });

        modelBuilder.Entity<KhoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhoHang__3214EC27A89ED70D");

            entity.ToTable("KhoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdloHang).HasColumnName("IDLoHang");

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.KhoHangs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__KhoHang__IDChiNh__52593CB8");

            entity.HasOne(d => d.IdloHangNavigation).WithMany(p => p.KhoHangs)
                .HasForeignKey(d => d.IdloHang)
                .HasConstraintName("FK__KhoHang__IDLoHan__534D60F1");
        });

        modelBuilder.Entity<LichSuDiem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichSuDi__3214EC27A75DB188");

            entity.ToTable("LichSuDiem");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.IdkhachHang).HasColumnName("IDKhachHang");

            entity.HasOne(d => d.IddonHangNavigation).WithMany(p => p.LichSuDiems)
                .HasForeignKey(d => d.IddonHang)
                .HasConstraintName("FK__LichSuDie__IDDon__6383C8BA");

            entity.HasOne(d => d.IdkhachHangNavigation).WithMany(p => p.LichSuDiems)
                .HasForeignKey(d => d.IdkhachHang)
                .HasConstraintName("FK__LichSuDie__IDKha__628FA481");
        });

        modelBuilder.Entity<LoHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoHang__3214EC27E3917D4A");

            entity.ToTable("LoHang");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IddonNhapHang).HasColumnName("IDDonNhapHang");
            entity.Property(e => e.Idthuoc).HasColumnName("IDThuoc");
            entity.Property(e => e.SoLo).HasMaxLength(50);

            entity.HasOne(d => d.IddonNhapHangNavigation).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.IddonNhapHang)
                .HasConstraintName("FK__LoHang__IDDonNha__4E88ABD4");

            entity.HasOne(d => d.IdthuocNavigation).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.Idthuoc)
                .HasConstraintName("FK__LoHang__IDThuoc__4F7CD00D");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC2717F62078");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "ix_nguoidung_tendangnhap");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.IdchiNhanh).HasColumnName("IDChiNhanh");
            entity.Property(e => e.IdvaiTro).HasColumnName("IDVaiTro");
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);

            entity.HasOne(d => d.IdchiNhanhNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdchiNhanh)
                .HasConstraintName("FK__NguoiDung__IDChi__3C69FB99");

            entity.HasOne(d => d.IdvaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdvaiTro)
                .HasConstraintName("FK__NguoiDung__IDVai__3B75D760");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhaCungC__3214EC276AE6BFA9");

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
            entity.HasKey(e => e.Id).HasName("PK__PhuongTh__3214EC2748AE5F67");

            entity.ToTable("PhuongThucThanhToan");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TenPhuongThuc).HasMaxLength(50);
        });

        modelBuilder.Entity<Thuoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Thuoc__3214EC27AFF14D1B");

            entity.ToTable("Thuoc");

            entity.HasIndex(e => e.TenThuoc, "ix_thuoc_tenthuoc");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HoatChat).HasMaxLength(100);
            entity.Property(e => e.IddanhMuc).HasColumnName("IDDanhMuc");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenThuoc).HasMaxLength(100);

            entity.HasOne(d => d.IddanhMucNavigation).WithMany(p => p.Thuocs)
                .HasForeignKey(d => d.IddanhMuc)
                .HasConstraintName("FK__Thuoc__IDDanhMuc__44FF419A");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VaiTro__3214EC27F3215076");

            entity.ToTable("VaiTro");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
