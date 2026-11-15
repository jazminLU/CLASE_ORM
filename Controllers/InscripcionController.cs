using System.Security.Claims;
using CLASE_ORM.Application.DTOs.Inscripcion;
using CLASE_ORM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CLASE_ORM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InscripcionController : ControllerBase
    {
        private readonly IInscripcionService _service;

        public InscripcionController(IInscripcionService service)
        {
            _service = service;
        }

        [HttpGet("mis-materias")]
        public async Task<IActionResult> MisMaterias()
        {
            var usuarioId = ObtenerUsuarioId();
            var result = await _service.GetMisMateriasAsync(usuarioId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar(SolicitarInscripcionDto dto)
        {
            var usuarioId = ObtenerUsuarioId();
            var result = await _service.SolicitarAsync(usuarioId, dto);
            return Ok(result);
        }

        [HttpGet("pendientes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pendientes()
        {
            var result = await _service.GetPendientesAsync();
            return Ok(result);
        }

        [HttpPut("{id}/aprobar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Aprobar(int id)
        {
            await _service.AprobarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/rechazar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Rechazar(int id)
        {
            await _service.RechazarAsync(id);
            return NoContent();
        }

        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
