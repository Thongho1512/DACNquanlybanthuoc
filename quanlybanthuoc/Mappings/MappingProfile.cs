using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos.NguoiDung;
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
        }
    }
}