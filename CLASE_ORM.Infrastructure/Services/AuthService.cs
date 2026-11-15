using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CLASE_ORM.Application.DTOs.Usuario;
using CLASE_ORM.Application.Exceptions;
using CLASE_ORM.Application.Interfaces;
using CLASE_ORM.Domain.Entities;
using CLASE_ORM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CLASE_ORM.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<UsuarioDto> RegistrarAsync(RegistrarUsuarioDto dto)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (existe) throw new ConflictException("El email ya esta registrado.");

            var esPrimerUsuario = !await _context.Usuarios.AnyAsync();
            var rol = esPrimerUsuario ? "Admin" : "Usuario";

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = rol
            };

            if (rol == "Usuario")
            {
                var alumno = new Alumno { Nombre = dto.Nombre };
                _context.Alumnos.Add(alumno);
                await _context.SaveChangesAsync();

                usuario.AlumnoId = alumno.Id;
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }

        public async Task<(string Token, UsuarioDto Usuario)> LoginAsync(LoginUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario is null) throw new UnauthorizedAccessException("Credenciales invalidas.");

            var valido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
            if (!valido) throw new UnauthorizedAccessException("Credenciales invalidas.");

            var token = GenerarToken(usuario);

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            };

            return (token, usuarioDto);
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireMinutes = Convert.ToDouble(_config["Jwt:ExpireMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
