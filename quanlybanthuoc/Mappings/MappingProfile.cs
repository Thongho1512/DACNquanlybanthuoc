using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Dtos.NhaCungCap;
using quanlybanthuoc.Dtos.PhuongThucThanhToan;
using quanlybanthuoc.Dtos.Thuoc; 

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

            // Thuoc mappings
            CreateMap<Thuoc, ThuocDto>();
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
                .ForMember(dest => dest.TenNguoiDung, opt => opt.MapFrom(src => src.IdnguoiDungNavigation.HoTen))
                .ForMember(dest => dest.TenKhachHang, opt => opt.MapFrom(src => src.IdkhachHangNavigation.TenKhachHang))
                .ForMember(dest => dest.TenChiNhanh, opt => opt.MapFrom(src => src.IdchiNhanhNavigation.TenChiNhanh))
                .ForMember(dest => dest.TenPhuongThucTt, opt => opt.MapFrom(src => src.IdphuongThucTtNavigation.TenPhuongThuc));

            CreateMap<CreateDonHangDto, DonHang>();

            // ChiTietDonHang mappings
            CreateMap<ChiTietDonHang, ChiTietDonHangDto>()
                .ForMember(dest => dest.TenThuoc, opt => opt.MapFrom(src => src.IdthuocNavigation.TenThuoc));

            // PhuongThucThanhToan mappings
            CreateMap<PhuongThucThanhToan, PhuongThucThanhToanDto>();
            CreateMap<CreatePhuongThucThanhToanDto, PhuongThucThanhToan>();
            CreateMap<UpdatePhuongThucThanhToanDto, PhuongThucThanhToan>();
        }
    }
}