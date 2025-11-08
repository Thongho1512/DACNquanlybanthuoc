using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Dtos.VaiTro;

namespace quanlybanthuoc.Services
{
    public interface IVaiTroService
    {
        Task<VaiTro> CreateAsync(CreateVaiTroDto dto);
        Task UpdateAsync(int id, UpdateVaiTroDto dto);
        Task<ThuocDto?> GetByIdAsync(int id);
        Task<PagedResult<VaiTroDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null,
            int? idDanhMuc = null);
        Task SoftDeleteAsync(int id);
    }
}
