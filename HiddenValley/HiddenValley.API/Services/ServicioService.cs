using Microsoft.EntityFrameworkCore;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;
using HiddenValley.API.Data;
using HiddenValley.API.Models; 

namespace HiddenValley.API.Services
{
    public class ServicioService : IServicioService
    {
        private readonly ApplicationDbContext _context;

        public ServicioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServicioReadDto>> GetPagedAsync(int pageNumber, int pageSize, string? nombre, int? id)
        {
            var query = _context.Servicio!.AsQueryable();

            if (id.HasValue)
                query = query.Where(s => s.IdServicio == id);

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(s => s.Nombre!.Contains(nombre));

        return await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(s => new ServicioReadDto {
            IdServicio = s.IdServicio,
            Nombre = s.Nombre,
            Descripcion = s.Descripcion,
            Precio = s.Precio
        })
        .ToListAsync();
        }

        public async Task<ServicioCreateDto> CreateServicioAsync(ServicioCreateDto servicioDto)
        {
            var nuevoServicio = new Servicio {
                Nombre = servicioDto.Nombre,
                Descripcion = servicioDto.Descripcion,
                Precio = servicioDto.Precio
            };

            _context.Servicio.Add(nuevoServicio);
            await _context.SaveChangesAsync();

            return new ServicioCreateDto {
                Nombre = nuevoServicio.Nombre,
                Descripcion = nuevoServicio.Descripcion,
                Precio = nuevoServicio.Precio
            };
        }

        public async Task<bool> PatchAsync(int id, UpdateServicioDto patchData)
        {
            var existente = await _context.Servicio!.FindAsync(id);
            if (existente == null) return false;
            if (patchData.Nombre != null) existente.Nombre = patchData.Nombre;
            if (patchData.Descripcion != null) existente.Descripcion = patchData.Descripcion;
            if (patchData.Precio != 0) existente.Precio = patchData.Precio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServicioAsync(int id)
        {
            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio == null) return false;

            _context.Servicio.Remove(servicio);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}