using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Services
{
    public class TipoServicioService : ITipoServicioService
    {
        private readonly ApplicationDbContext _context;

        public TipoServicioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoServicioReadDto>> GetAllAsync()
        {
            return await _context.TiposServicio
                .Select(t => new TipoServicioReadDto
                {
                    IdTipoServicio = t.IdTipoServicio,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion
                })
                .ToListAsync();
        }

        public async Task<TipoServicioReadDto?> GetByIdAsync(int id)
        {
            var tipo = await _context.TiposServicio.FindAsync(id);
            if (tipo == null) return null;

            return new TipoServicioReadDto
            {
                IdTipoServicio = tipo.IdTipoServicio,
                Nombre = tipo.Nombre,
                Descripcion = tipo.Descripcion
            };
        }

        public async Task<TipoServicioReadDto> CreateAsync(TipoServicioCreateDto dto)
        {
            var nuevoTipo = new TipoServicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.TiposServicio.Add(nuevoTipo);
            await _context.SaveChangesAsync();

            return new TipoServicioReadDto
            {
                IdTipoServicio = nuevoTipo.IdTipoServicio,
                Nombre = nuevoTipo.Nombre,
                Descripcion = nuevoTipo.Descripcion
            };
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, TipoServicioCreateDto dto)
        {
            var tipo = await _context.TiposServicio.FindAsync(id);
            if (tipo == null) return (false, "Tipo de servicio no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) tipo.Nombre = dto.Nombre;
            if (dto.Descripcion != null) tipo.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return (true, "Actualizado correctamente.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var tipo = await _context.TiposServicio.FindAsync(id);
            if (tipo == null) return (false, "Tipo de servicio no encontrado.");

            // verificar si hay servicios usando este tipo antes de borrar
            var tieneServicios = await _context.Servicio.AnyAsync(s => s.IdTipoServicio == id);
            if (tieneServicios) return (false, "No se puede eliminar: hay servicios asociados a este tipo.");

            _context.TiposServicio.Remove(tipo);
            await _context.SaveChangesAsync();
            return (true, "Eliminado correctamente.");
        }
    }
}
