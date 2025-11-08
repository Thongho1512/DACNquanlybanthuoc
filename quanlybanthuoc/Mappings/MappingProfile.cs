// File: quanlybanthuoc/Mappings/MappingProfile.cs
using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Dtos.DonNhapHang;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Dtos.KhoHang;
using quanlybanthuoc.Dtos.LoHang;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Dtos.NhaCungCap;
using quanlybanthuoc.Dtos.PhuongThucThanhToan;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Dtos.VaiTro;

namespace quanlybanthuoc.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // NguoiDung mappings
            CreateMap<NguoiDungDto, NguoiDung>();
            CreateMap<CreateNguoiDungDto, NguoiDung>();
            CreateMap<NguoiDung, NguoiDungDto>();
            CreateMap<UpdateNguoiDungDto, NguoiDung>();
            //  VaiTro mappings
            CreateMap<VaiTro, VaiTroDto>();
            CreateMap<CreateVaiTroDto, VaiTro>();
            CreateMap<UpdateVaiTroDto, VaiTro>();

            // Thuoc mappings
            CreateMap<Thuoc, ThuocDto>()
                .ForMember(dest => dest.TenDanhMuc,
                    opt => opt.MapFrom(src => src.IddanhMucNavigation != null ? src.IddanhMucNavigation.TenDanhMuc : null));
            CreateMap<CreateThuocDto, Thuoc>();
            CreateMap<UpdateThuocDto, Thuoc>();

            // DanhMuc mappings
            CreateMap<DanhMuc, DanhMucDto>();
            CreateMap<CreateDanhMucDto, DanhMuc>();
            CreateMap<UpdateDanhMucDto, DanhMuc>();

            // NhaCungCap mappings
            CreateMap<NhaCungCap, NhaCungCapDto>();
            CreateMap<CreateNhaCungCapDto, NhaCungCap>();
            CreateMap<UpdateNhaCungCapDto, NhaCungCap>();

            // ChiNhanh mappings
            CreateMap<ChiNhanh, ChiNhanhDto>();
            CreateMap<CreateChiNhanhDto, ChiNhanh>();
            CreateMap<UpdateChiNhanhDto, ChiNhanh>();

            // KhachHang mappings
            CreateMap<KhachHang, KhachHangDto>();
            CreateMap<CreateKhachHangDto, KhachHang>();
            CreateMap<UpdateKhachHangDto, KhachHang>();

            // DonHang mappings
            CreateMap<DonHang, DonHangDto>()
                .ForMember(dest => dest.TenNguoiDung, opt => opt.MapFrom(src => src.IdnguoiDungNavigation != null ? src.IdnguoiDungNavigation.HoTen : null))
                .ForMember(dest => dest.TenKhachHang, opt => opt.MapFrom(src => src.IdkhachHangNavigation != null ? src.IdkhachHangNavigation.TenKhachHang : null))
                .ForMember(dest => dest.TenChiNhanh, opt => opt.MapFrom(src => src.IdchiNhanhNavigation != null ? src.IdchiNhanhNavigation.TenChiNhanh : null))
                .ForMember(dest => dest.TenPhuongThucTt, opt => opt.MapFrom(src => src.IdphuongThucTtNavigation != null ? src.IdphuongThucTtNavigation.TenPhuongThuc : null));

            CreateMap<CreateDonHangDto, DonHang>();

            // ChiTietDonHang mappings
            CreateMap<ChiTietDonHang, ChiTietDonHangDto>()
                .ForMember(dest => dest.TenThuoc, opt => opt.MapFrom(src => src.IdthuocNavigation != null ? src.IdthuocNavigation.TenThuoc : null));

            // PhuongThucThanhToan mappings
            CreateMap<PhuongThucThanhToan, PhuongThucThanhToanDto>();
            CreateMap<CreatePhuongThucThanhToanDto, PhuongThucThanhToan>();
            CreateMap<UpdatePhuongThucThanhToanDto, PhuongThucThanhToan>();

            //  LoHang mappings
            CreateMap<LoHang, LoHangDto>()
                .ForMember(dest => dest.TenThuoc,
                    opt => opt.MapFrom(src => src.IdthuocNavigation != null ? src.IdthuocNavigation.TenThuoc : null))
                .ForMember(dest => dest.SoDonNhap,
                    opt => opt.MapFrom(src => src.IddonNhapHangNavigation != null ? src.IddonNhapHangNavigation.SoDonNhap : null));
            CreateMap<CreateLoHangDto, LoHang>();
            CreateMap<UpdateLoHangDto, LoHang>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // KhoHang mappings
            CreateMap<KhoHang, KhoHangDto>()
                .ForMember(dest => dest.TenChiNhanh,
                    opt => opt.MapFrom(src => src.IdchiNhanhNavigation != null ? src.IdchiNhanhNavigation.TenChiNhanh : null))
                .ForMember(dest => dest.TenThuoc,
                    opt => opt.MapFrom(src => src.IdloHangNavigation != null && src.IdloHangNavigation.IdthuocNavigation != null
                        ? src.IdloHangNavigation.IdthuocNavigation.TenThuoc : null))
                .ForMember(dest => dest.SoLo,
                    opt => opt.MapFrom(src => src.IdloHangNavigation != null ? src.IdloHangNavigation.SoLo : null))
                .ForMember(dest => dest.NgayHetHan,
                    opt => opt.MapFrom(src => src.IdloHangNavigation != null ? src.IdloHangNavigation.NgayHetHan : null));
            CreateMap<CreateKhoHangDto, KhoHang>();
            CreateMap<UpdateKhoHangDto, KhoHang>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DonNhapHang mappings
            CreateMap<DonNhapHang, DonNhapHangDto>()
                .ForMember(dest => dest.TenChiNhanh,
                    opt => opt.MapFrom(src => src.IdchiNhanhNavigation != null ? src.IdchiNhanhNavigation.TenChiNhanh : null))
                .ForMember(dest => dest.TenNhaCungCap,
                    opt => opt.MapFrom(src => src.IdnhaCungCapNavigation != null ? src.IdnhaCungCapNavigation.TenNhaCungCap : null))
                .ForMember(dest => dest.TenNguoiNhan,
                    opt => opt.MapFrom(src => src.IdnguoiNhanNavigation != null ? src.IdnguoiNhanNavigation.HoTen : null));
            CreateMap<CreateDonNhapHangDto, DonNhapHang>();
        }
    }
}