using CLASE_ORM.Application.Common;
using CLASE_ORM.Application.DTOs.Alumno;
using CLASE_ORM.Domain.Entities;

namespace CLASE_ORM.Application.Interfaces
{
    public interface IAlumnoService
    {
        Task<PagedResult<AlumnoDto>> GetAllAsync(int pageNumber, int pageSize, string? nombre);
        Task<AlumnoDto> GetByIdAsync(int id);
        Task<Alumno> CreateAsync(CrearAlumnoDto dto);
        Task UpdateAsync(int id, ActualizarAlumnoDto dto);
        Task DeleteAsync(int id);
    }
}
