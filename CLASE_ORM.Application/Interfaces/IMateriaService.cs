using CLASE_ORM.Application.Common;
using CLASE_ORM.Application.DTOs.Materia;
using CLASE_ORM.Domain.Entities;

namespace CLASE_ORM.Application.Interfaces
{
    public interface IMateriaService
    {
        Task<PagedResult<MateriaDto>> GetAllAsync(int pageNumber, int pageSize, string? nombre);
        Task<MateriaDto> GetByIdAsync(int id);
        Task<Materia> CreateAsync(CrearMateriaDto dto);
        Task UpdateAsync(int id, ActualizarMateriaDto dto);
        Task DeleteAsync(int id);
    }
}
