using CLASE_ORM.Application.DTOs.Materia;
using CLASE_ORM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CLASE_ORM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaService _service;

        public MateriaController(IMateriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? nombre = null)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize, nombre);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var materia = await _service.GetByIdAsync(id);
            return Ok(materia);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CrearMateriaDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ActualizarMateriaDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
