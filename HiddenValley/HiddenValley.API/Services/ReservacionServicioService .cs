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
                    .ThenInclude(r => r!.Cliente)
                        .ThenInclude(c => c!.Persona)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(cliente))
            {
                query = query.Where(x => x.Reservacion!.Cliente!.Persona!.Nombres.ToLower().Contains(cliente.ToLower()));
            }

            if (idServicio.HasValue)
                query = query.Where(x => x.IdServicio == idServicio.Value);

            if (fecha.HasValue)
            {
                var inicioDia = fecha.Value.Date;
                var finDia = inicioDia.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.Reservacion!.FechaEntrada >= inicioDia && x.Reservacion!.FechaEntrada <= finDia);
            }

            var rawItems = await query
                .OrderByDescending(x => x.IdReservacion)
                .ToListAsync();

            var agrupado = rawItems
                .GroupBy(x => x.IdReservacion)
                .Select(g => new ReservacionServicioReadDto
                {
                    IdReservacion = g.Key,
                    NombreCliente = g.First().Reservacion!.Cliente!.Persona!.Nombres,
                    FechaEntrada = g.First().Reservacion!.FechaEntrada,
                    EstadoReserva = g.First().Reservacion!.EstadoReserva,
                    Servicios = g.Select(s => new DetalleServicioDto
                    {
                        IdServicio = s.IdServicio,
                        NombreServicio = s.Servicio != null ? s.Servicio.Nombre : "N/A",
                        Cantidad = s.Cantidad
                    }).ToList()
                }).ToList();

            int totalRegistros = agrupado.Count;

            var itemsPaginados = agrupado
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            return new PagedResultReservacionServicio<ReservacionServicioReadDto>
            {
                Items = itemsPaginados,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina)
            };
        }

        public async Task<bool> CreateAsync(ReservacionServicioCreateDto dto)
        {
            var existente = await _context.ReservacionServicios
                .FirstOrDefaultAsync(x => x.IdReservacion == dto.IdReservacion && x.IdServicio == dto.IdServicio);

            if (existente != null)
            {
                existente.Cantidad += dto.Cantidad;
            }
            else
            {
                var ent = new ReservacionServicio
                {
                    IdReservacion = dto.IdReservacion,
                    IdServicio = dto.IdServicio,
                    Cantidad = dto.Cantidad
                };
                _context.ReservacionServicios.Add(ent);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int idRes, int idSer, ReservacionServicioUpdateDto dto)
        {
            var ent = await _context.ReservacionServicios.FirstOrDefaultAsync(x => x.IdReservacion == idRes && x.IdServicio == idSer);
            if (ent == null) return false;
            ent.Cantidad = dto.Cantidad;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int idRes, int idSer)
        {
            var ent = await _context.ReservacionServicios.FirstOrDefaultAsync(x => x.IdReservacion == idRes && x.IdServicio == idSer);
            if (ent == null) return false;
            _context.ReservacionServicios.Remove(ent);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}