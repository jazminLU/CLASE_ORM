using CLASE_ORM.Application.Common;
using CLASE_ORM.Application.DTOs.Alumno;
using CLASE_ORM.Application.Exceptions;
using CLASE_ORM.Application.Interfaces;
using CLASE_ORM.Domain.Entities;
using CLASE_ORM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CLASE_ORM.Infrastructure.Services
{
    public class AlumnoService : IAlumnoService
    {

        private readonly AppDbContext _context;

        public AlumnoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AlumnoDto>> GetAllAsync(int pageNumber, int pageSize, string? nombre)
        {
            var query = _context.Alumnos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(a => a.Nombre.Contains(nombre));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.Nombre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlumnoDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Materias = a.Inscripciones
                        .Where(i => i.Estado == "Aprobada")
                        .Select(i => i.Materia.Nombre)
                        .ToList()
                })
                .ToListAsync();

            return new PagedResult<AlumnoDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<AlumnoDto> GetByIdAsync(int id)
        {

            var alumno = await _context.Alumnos
                .Where(a => a.Id == id)
                .Select(a => new AlumnoDto
                {

                    Id = a.Id,
                    Nombre = a.Nombre,
                    Materias = a.Inscripciones
                        .Where(i => i.Estado == "Aprobada")
                        .Select(i => i.Materia.Nombre)
                        .ToList()

                })
                .FirstOrDefaultAsync();

            if (alumno is null) throw new NotFoundException($"No se encontró el alumno con id {id}.");

            return alumno;

        }

        public async Task<Alumno> CreateAsync(CrearAlumnoDto dto)
        {

            var alumno = new Alumno
            {
                Nombre = dto.Nombre
            };

            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();

            return alumno;

        }

        public async Task UpdateAsync(int id, ActualizarAlumnoDto dto)
        {

            var existing = await _context.Alumnos.FindAsync(id)
                ?? throw new NotFoundException($"No se encontró el alumno con id {id}.");

            existing.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {

            var alumno = await _context.Alumnos.FindAsync(id)
                ?? throw new NotFoundException($"No se encontró el alumno con id {id}.");

            _context.Alumnos.Remove(alumno);
            await _context.SaveChangesAsync();

        }
    }
}
