using CLASE_ORM.Application.DTOs.Usuario;
using CLASE_ORM.Application.Interfaces;
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
            return Ok(usuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUsuarioDto dto)
        {
            var resultado = await _service.LoginAsync(dto);
            return Ok(new { token = resultado.Token, usuario = resultado.Usuario });
        }
    }
}
