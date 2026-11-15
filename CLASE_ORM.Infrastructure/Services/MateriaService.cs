using CLASE_ORM.Application.Common;
using CLASE_ORM.Application.DTOs.Materia;
using CLASE_ORM.Application.Exceptions;
using CLASE_ORM.Application.Interfaces;
using CLASE_ORM.Domain.Entities;
using CLASE_ORM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CLASE_ORM.Infrastructure.Services
{
    public class MateriaService : IMateriaService
    {
        private readonly AppDbContext _context;

        public MateriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<MateriaDto>> GetAllAsync(int pageNumber, int pageSize, string? nombre)
        {
            var query = _context.Materias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(m => m.Nombre.Contains(nombre));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.Nombre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MateriaDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre
                })
                .ToListAsync();

            return new PagedResult<MateriaDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<MateriaDto> GetByIdAsync(int id)
        {
            var materia = await _context.Materias
                .Where(m => m.Id == id)
                .Select(m => new MateriaDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre
                })
                .FirstOrDefaultAsync();

            if (materia is null) throw new NotFoundException($"No se encontró la materia con id {id}.");

            return materia;
        }

        public async Task<Materia> CreateAsync(CrearMateriaDto dto)
        {
            var materia = new Materia
            {
                Nombre = dto.Nombre
            };

            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            return materia;
        }

        public async Task UpdateAsync(int id, ActualizarMateriaDto dto)
        {
            var existing = await _context.Materias.FindAsync(id)
                ?? throw new NotFoundException($"No se encontró la materia con id {id}.");

            existing.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var materia = await _context.Materias.FindAsync(id)
                ?? throw new NotFoundException($"No se encontró la materia con id {id}.");

            _context.Materias.Remove(materia);
            await _context.SaveChangesAsync();
        }

    }
}
