using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Interfaces;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;

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
            var query = _context.Servicio.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(s => s.IdServicio == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(s => s.Nombre != null && s.Nombre.ToLower().Contains(nombre.ToLower()));
            }

            var itemsDb = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return itemsDb.Select(s => new ServicioReadDto
            {
                IdServicio = s.IdServicio,
                Nombre = s.Nombre ?? string.Empty,
                Descripcion = s.Descripcion ?? string.Empty,
                Precio = s.Precio
            }).ToList();
        }

        public async Task<ServicioCreateDto> CreateServicioAsync(ServicioCreateDto servicioDto)
        {
            var nuevoServicio = new Servicio
            {
                Nombre = servicioDto.Nombre,
                Descripcion = servicioDto.Descripcion,
                Precio = servicioDto.Precio
            };
            _context.Servicio.Add(nuevoServicio);
            await _context.SaveChangesAsync();
            return servicioDto;
        }

        public async Task<bool> PatchAsync(int id, UpdateServicioDto patchData)
        {
            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio == null) return false;

            if (patchData.Nombre != null) servicio.Nombre = patchData.Nombre;
            if (patchData.Descripcion != null) servicio.Descripcion = patchData.Descripcion;
            if (patchData.Precio > 0) servicio.Precio = patchData.Precio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServicioAsync(int id)
        {
            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio == null) return false;

            _context.Servicio.Remove(servicio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}