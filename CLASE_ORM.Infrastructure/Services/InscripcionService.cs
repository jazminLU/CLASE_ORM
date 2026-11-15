using CLASE_ORM.Application.DTOs.Inscripcion;
using CLASE_ORM.Application.Exceptions;
using CLASE_ORM.Application.Interfaces;
using CLASE_ORM.Domain.Entities;
using CLASE_ORM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CLASE_ORM.Infrastructure.Services
{
    public class InscripcionService : IInscripcionService
    {
        private readonly AppDbContext _context;

        public InscripcionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InscripcionDto>> GetMisMateriasAsync(int usuarioId)
        {
            var alumnoId = await ResolverAlumnoIdAsync(usuarioId);

            return await _context.Inscripciones
                .Where(i => i.AlumnoId == alumnoId)
                .Select(i => new InscripcionDto
                {
                    Id = i.Id,
                    MateriaId = i.MateriaId,
                    MateriaNombre = i.Materia.Nombre,
                    AlumnoId = i.AlumnoId,
                    AlumnoNombre = i.Alumno.Nombre,
                    Estado = i.Estado,
                    FechaSolicitud = i.FechaSolicitud
                })
                .ToListAsync();
        }

        public async Task<InscripcionDto> SolicitarAsync(int usuarioId, SolicitarInscripcionDto dto)
        {
            var alumnoId = await ResolverAlumnoIdAsync(usuarioId);

            var materiaExiste = await _context.Materias.AnyAsync(m => m.Id == dto.MateriaId);
            if (!materiaExiste) throw new NotFoundException($"No se encontró la materia con id {dto.MateriaId}.");

            var yaExiste = await _context.Inscripciones.AnyAsync(i =>
                i.AlumnoId == alumnoId &&
                i.MateriaId == dto.MateriaId &&
                (i.Estado == "Pendiente" || i.Estado == "Aprobada"));

            if (yaExiste) throw new ConflictException("Ya tenés una inscripción pendiente o aprobada para esta materia.");

            var inscripcion = new Inscripcion
            {
                AlumnoId = alumnoId,
                MateriaId = dto.MateriaId,
                Estado = "Pendiente"
            };

            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            return await _context.Inscripciones
                .Where(i => i.Id == inscripcion.Id)
                .Select(i => new InscripcionDto
                {
                    Id = i.Id,
                    MateriaId = i.MateriaId,
                    MateriaNombre = i.Materia.Nombre,
                    AlumnoId = i.AlumnoId,
                    AlumnoNombre = i.Alumno.Nombre,
                    Estado = i.Estado,
                    FechaSolicitud = i.FechaSolicitud
                })
                .FirstAsync();
        }

        public async Task<List<InscripcionDto>> GetPendientesAsync()
        {
            return await _context.Inscripciones
                .Where(i => i.Estado == "Pendiente")
                .OrderBy(i => i.FechaSolicitud)
                .Select(i => new InscripcionDto
                {
                    Id = i.Id,
                    MateriaId = i.MateriaId,
                    MateriaNombre = i.Materia.Nombre,
                    AlumnoId = i.AlumnoId,
                    AlumnoNombre = i.Alumno.Nombre,
                    Estado = i.Estado,
                    FechaSolicitud = i.FechaSolicitud
                })
                .ToListAsync();
        }

        public async Task AprobarAsync(int inscripcionId)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(inscripcionId)
                ?? throw new NotFoundException($"No se encontró la inscripción con id {inscripcionId}.");

            inscripcion.Estado = "Aprobada";
            await _context.SaveChangesAsync();
        }

        public async Task RechazarAsync(int inscripcionId)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(inscripcionId)
                ?? throw new NotFoundException($"No se encontró la inscripción con id {inscripcionId}.");

            inscripcion.Estado = "Rechazada";
            await _context.SaveChangesAsync();
        }

        private async Task<int> ResolverAlumnoIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new NotFoundException("No se encontró el usuario.");

            if (usuario.AlumnoId is null)
                throw new NotFoundException("Tu cuenta no tiene un alumno vinculado.");

            return usuario.AlumnoId.Value;
        }
    }
}
