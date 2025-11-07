using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;

namespace quanlybanthuoc.Services
{
    public interface IThuocService
    {
        Task<ThuocDto> CreateAsync(CreateThuocDto dto);
        Task UpdateAsync(int id, UpdateThuocDto dto);
        Task<ThuocDto?> GetByIdAsync(int id);
        Task<PagedResult<ThuocDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null,
            int? idDanhMuc = null);
        Task SoftDeleteAsync(int id);
        Task<IEnumerable<ThuocDto>> GetThuocSapHetHanAsync(int days, int? idChiNhanh = null);
        Task<IEnumerable<ThuocDto>> GetThuocTonKhoThapAsync(int? idChiNhanh = null);
    }
}