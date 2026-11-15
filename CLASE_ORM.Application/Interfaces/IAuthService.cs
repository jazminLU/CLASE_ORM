using CLASE_ORM.Application.DTOs.Usuario;

namespace CLASE_ORM.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioDto> RegistrarAsync(RegistrarUsuarioDto dto);
        Task<(string Token, UsuarioDto Usuario)> LoginAsync(LoginUsuarioDto dto);
    }
}
