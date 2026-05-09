using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Services;

public class PuestoTrabajoService(ApplicationDbContext context) : IPuestoTrabajoService
{
    public async Task<PagedResponse<PuestoTrabajoResponseDto>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = context.PuestosTrabajo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Buscar por nombre o por Id si el término es numérico
            if (int.TryParse(search, out var id))
                query = query.Where(p => p.IdPuestoTrabajo == id);
            else
                query = query.Where(p => p.Nombre.ToLower().Contains(search.ToLower()));
        }

        var totalRecords = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PuestoTrabajoResponseDto
            {
                IdPuestoTrabajo = p.IdPuestoTrabajo,
                Nombre          = p.Nombre,
                Descripcion     = p.Descripcion
            })
            .ToListAsync();

        return new PagedResponse<PuestoTrabajoResponseDto>
        {
            TotalRecords = totalRecords,
            TotalPages   = (int)Math.Ceiling(totalRecords / (double)pageSize),
            CurrentPage  = page,
            PageSize     = pageSize,
            Items        = items
        };
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(PuestoTrabajoCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return (false, "El nombre del puesto es obligatorio.", null);

        if (await context.PuestosTrabajo.AnyAsync(p => p.Nombre.ToLower() == dto.Nombre.ToLower()))
            return (false, "Ya existe un puesto con ese nombre.", null);

        var puesto = new PuestoTrabajo
        {
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        context.PuestosTrabajo.Add(puesto);
        await context.SaveChangesAsync();
        return (true, "Puesto de trabajo creado correctamente.", puesto.IdPuestoTrabajo);
    }

    public async Task<(bool Success, string Message)> PatchAsync(int id, PuestotrabajoPatchDto dto)
    {
        var puesto = await context.PuestosTrabajo.FindAsync(id);
        if (puesto == null) return (false, "Puesto de trabajo no encontrado.");

        if (dto.Nombre != null)
        {
            if (await context.PuestosTrabajo.AnyAsync(p => p.Nombre.ToLower() == dto.Nombre.ToLower() && p.IdPuestoTrabajo != id))
                return (false, "Ya existe otro puesto con ese nombre.");

            puesto.Nombre = dto.Nombre;
        }

        if (dto.Descripcion != null) puesto.Descripcion = dto.Descripcion;

        await context.SaveChangesAsync();
        return (true, "Puesto de trabajo actualizado correctamente.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var puesto = await context.PuestosTrabajo.FindAsync(id);
        if (puesto == null) return (false, "Puesto de trabajo no encontrado.");

        if (await context.Empleados.AnyAsync(e => e.IdPuestoTrabajo == id))
            return (false, "No se puede eliminar: hay empleados asignados a este puesto.");

        context.PuestosTrabajo.Remove(puesto);
        await context.SaveChangesAsync();
        return (true, "Puesto de trabajo eliminado correctamente.");
    }
}
