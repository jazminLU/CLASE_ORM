using CLASE_ORM.Entities.DTO.Usuario;

namespace CLASE_ORM.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioDto?> RegistrarAsync(RegistrarUsuarioDto dto);
        Task<(string Token, UsuarioDto Usuario)?> LoginAsync(LoginUsuarioDto dto);
    }
}
