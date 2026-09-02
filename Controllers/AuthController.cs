using CLASE_ORM.Entities.DTO.Usuario;
using CLASE_ORM.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CLASE_ORM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrarUsuarioDto dto)
        {
            var usuario = await _service.RegistrarAsync(dto);
            if (usuario is null) return Conflict(new { message = "El email ya esta registrado." });

            return Ok(usuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUsuarioDto dto)
        {
            var resultado = await _service.LoginAsync(dto);
            if (resultado is null) return Unauthorized(new { message = "Credenciales invalidas." });

            return Ok(new { token = resultado.Value.Token, usuario = resultado.Value.Usuario });
        }
    }
}
