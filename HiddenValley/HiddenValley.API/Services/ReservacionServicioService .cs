using Microsoft.EntityFrameworkCore;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;
using HiddenValley.API.Data;
using HiddenValley.API.Models;

namespace HiddenValley.API.Services
{
    public class ReservacionServicioService : IReservacionServicio
    {
        private readonly ApplicationDbContext _context;

        public ReservacionServicioService(ApplicationDbContext context) => _context = context;

        public async Task<PagedResultReservacionServicio<ReservacionServicioReadDto>> GetPagedAsync(
            int pagina, int registrosPorPagina, string? cliente, int? idServicio, DateTime? fecha)
        {
            var query = _context.ReservacionServicios
                .Include(x => x.Servicio)
                .Include(x => x.Reservacion)
                    .ThenInclude(r => r!.Cliente) // El ! evita el warning de nulabilidad
                        .ThenInclude(c => c!.Persona)
                .AsQueryable();

            // Filtro por Cliente (Persona)
            if (!string.IsNullOrWhiteSpace(cliente))
            {
                // Asumiendo que en Persona tienes Nombre1 y Apellido1 o similar
                // Ajusta 'Nombre' según lo que diga tu clase Persona.cs
                query = query.Where(x => x.Reservacion!.Cliente!.Persona!.Nombres.Contains(cliente));
            }

            if (idServicio.HasValue)
                query = query.Where(x => x.IdServicio == idServicio.Value);

            if (fecha.HasValue)
                query = query.Where(x => x.Reservacion!.FechaEntrada.Date == fecha.Value.Date);

            int totalRegistros = await query.CountAsync();
            
            var items = await query
                .OrderByDescending(x => x.IdReservacion)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(x => new ReservacionServicioReadDto {
                    IdReservacion = x.IdReservacion,
                    IdServicio = x.IdServicio,
                    Cantidad = x.Cantidad,
                    NombreServicio = x.Servicio != null ? x.Servicio.Nombre : "N/A",
                    // IMPORTANTE: Verifica si en Persona.cs es 'Nombre' o 'Nombres'
                    NombreCliente = x.Reservacion!.Cliente!.Persona!.Nombres, 
                    FechaEntrada = x.Reservacion.FechaEntrada,
                    EstadoReserva = x.Reservacion.EstadoReserva
                })
                .ToListAsync();

            return new PagedResultReservacionServicio<ReservacionServicioReadDto> {
                Items = items,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina)
            };
        }

        public async Task<bool> CreateAsync(ReservacionServicioCreateDto dto)
        {
            var ent = new ReservacionServicio { 
                IdReservacion = dto.IdReservacion, 
                IdServicio = dto.IdServicio, 
                Cantidad = dto.Cantidad 
            };
            _context.ReservacionServicios.Add(ent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int idRes, int idSer, ReservacionServicioUpdateDto dto)
        {
            var ent = await _context.ReservacionServicios.FindAsync(idRes, idSer);
            if (ent == null) return false;
            ent.Cantidad = dto.Cantidad;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int idRes, int idSer)
        {
            var ent = await _context.ReservacionServicios.FindAsync(idRes, idSer);
            if (ent == null) return false;
            _context.ReservacionServicios.Remove(ent);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}