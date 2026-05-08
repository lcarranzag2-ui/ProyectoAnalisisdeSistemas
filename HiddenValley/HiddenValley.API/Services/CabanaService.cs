using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.API.Interfaces; 
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Services;

public class CabanaService(ApplicationDbContext context) : ICabanaService
{
    public async Task<PagedResponse<object>> GetPagedAsync(string? searchTerm, int page, int pageSize)
    {
        var query = context.Cabanas
            .Include(c => c.TipoCabana)
            .Include(c => c.EstadoCabana)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(c => 
                (c.TipoCabana != null && c.TipoCabana.Nombre.ToLower().Contains(searchTerm)) ||
                (c.EstadoCabana != null && c.EstadoCabana.Nombre.ToLower().Contains(searchTerm)));
        }

        var totalRecords = await query.CountAsync();
        
        var items = await query
            .OrderBy(c => c.IdCabana)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new {
                c.IdCabana,
                TipoCabana = c.TipoCabana != null ? c.TipoCabana.Nombre : "Desconocido",
                Capacidad = c.TipoCabana != null ? c.TipoCabana.Capacidad : 0,
                Precio = c.TipoCabana != null ? c.TipoCabana.Precio : 0,
                EstadoActual = c.EstadoCabana != null ? c.EstadoCabana.Nombre : "Desconocido"
            }).ToListAsync();

        return new PagedResponse<object>
        {
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<IEnumerable<object>> GetDisponibilidadAsync(DateTime inicio, DateTime fin)
    {
        var ocupadas = await context.RegistroReservacion
            .Where(r => r.FechaEntrada < fin && r.FechaSalida > inicio)
            .Select(r => r.IdCabana)
            .Distinct()
            .ToListAsync();

        return await context.Cabanas
            .Include(c => c.TipoCabana)
            .Include(c => c.EstadoCabana)
            .Select(c => new {
                c.IdCabana,
                TipoCabana = c.TipoCabana != null ? c.TipoCabana.Nombre : "Desconocido",
                Capacidad = c.TipoCabana != null ? c.TipoCabana.Capacidad : 0,
                Precio = c.TipoCabana != null ? c.TipoCabana.Precio : 0,
                EstadoActual = c.EstadoCabana != null ? c.EstadoCabana.Nombre : "Desconocido",
                EstaDisponible = !ocupadas.Contains(c.IdCabana) && 
                                 (c.EstadoCabana != null && c.EstadoCabana.Nombre == "Disponible")
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message, object? Data)> CambiarEstadoAsync(CambiarEstadoRequest request)
    {
        var cabana = await context.Cabanas
            .FirstOrDefaultAsync(c => c.IdCabana == request.IdCabana);

        if (cabana == null) return (false, "La cabaña no existe.", null);

        var nuevoEstado = await context.EstadosCabana
            .FirstOrDefaultAsync(e => e.Nombre == request.NuevoEstado);

        if (nuevoEstado == null) return (false, "Estado no válido.", null);

        // Actualizamos solo el estado de la cabaña
        cabana.IdEstadoCabana = nuevoEstado.IdEstadoCabana;
        
        await context.SaveChangesAsync();

        return (true, "Estado actualizado con éxito.", new { 
            IdCabana = cabana.IdCabana,
            NuevoEstado = request.NuevoEstado 
        });
    }

    public async Task<(bool Success, string Message, int? Id)> RegistrarCabanaAsync(RegistrarCabanaRequest request)
    {
        if (!await context.TiposCabana.AnyAsync(t => t.IdTipoCabana == request.IdTipoCabana))
            return (false, "El tipo de cabaña no existe.", null);

        var nuevaCabana = new Cabana
        {
            IdTipoCabana = request.IdTipoCabana,
            IdEstadoCabana = request.IdEstadoCabana
        };

        context.Cabanas.Add(nuevaCabana);
        await context.SaveChangesAsync();

        return (true, "Cabaña registrada correctamente.", nuevaCabana.IdCabana);
    }

    public async Task<(bool Success, string Message)> EliminarCabanaAsync(int id)
    {
        var cabana = await context.Cabanas.FindAsync(id);
        if (cabana == null) return (false, "La cabaña no existe.");

        if (await context.RegistroReservacion.AnyAsync(r => r.IdCabana == id))
            return (false, "No se puede eliminar: tiene reservaciones asociadas.");

        context.Cabanas.Remove(cabana);
        await context.SaveChangesAsync();

        return (true, "Cabaña eliminada correctamente.");
    }
}