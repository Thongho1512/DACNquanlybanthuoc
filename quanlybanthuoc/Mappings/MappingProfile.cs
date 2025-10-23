using AutoMapper;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos.NguoiDung;

namespace quanlybanthuoc.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create your mappings here
            // NguoiDung mappings
            CreateMap<NguoiDungDto, NguoiDung>();
            CreateMap<CreateNguoiDungDto, NguoiDung>();
            CreateMap<NguoiDung, NguoiDungDto>();
        }
    }
}
