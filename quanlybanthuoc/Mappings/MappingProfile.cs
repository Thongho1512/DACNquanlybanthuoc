using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Dtos.NhaCungCap;
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
        }
    }
}