using CLASE_ORM.Application.DTOs.Inscripcion;

namespace CLASE_ORM.Application.Interfaces
{
    public interface IInscripcionService
    {
        Task<List<InscripcionDto>> GetMisMateriasAsync(int usuarioId);
        Task<InscripcionDto> SolicitarAsync(int usuarioId, SolicitarInscripcionDto dto);
        Task<List<InscripcionDto>> GetPendientesAsync();
        Task AprobarAsync(int inscripcionId);
        Task RechazarAsync(int inscripcionId);
    }
}
