using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Services;

public class EmpleadoService(ApplicationDbContext context) : IEmpleadoService
{
    public async Task<PagedResponse<EmpleadoResponseDTO>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = context.Empleados
            .Include(e => e.Persona)
            .Include(e => e.PuestoTrabajo)
            .AsQueryable();

        // Filtro de búsqueda
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(e => 
                e.Persona!.Nombres.ToLower().Contains(search) || 
                e.Persona.Apellidos.ToLower().Contains(search) ||
                e.PuestoTrabajo!.Nombre.ToLower().Contains(search));
        }

        var totalRecords = await query.CountAsync();
        
        // Mapeo manual de Entidad a DTO usando inicializadores { }
        var items = await query
            .OrderBy(e => e.IdEmpleado)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EmpleadoResponseDTO
            {
                IdEmpleado = e.IdEmpleado,
                IdPersona = e.IdPersona,
                NombreCompleto = $"{e.Persona!.Nombres} {e.Persona.Apellidos}",
                IdPuestoTrabajo = e.IdPuestoTrabajo,
                NombrePuesto = e.PuestoTrabajo!.Nombre,
                Telefono = e.Persona.Telefono,
                Email = e.Persona.Gmail ?? "N/A"
            }).ToListAsync();

        return new PagedResponse<EmpleadoResponseDTO>
        {
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(EmpleadoCreateDTO dto)
    {
        // Validaciones de negocio
        if (!await context.PuestosTrabajo.AnyAsync(p => p.IdPuestoTrabajo == dto.IdPuestoTrabajo))
            return (false, "El puesto de trabajo no existe.", null);

        if (!await context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona))
            return (false, "La persona no existe.", null);

        if (await context.Empleados.AnyAsync(e => e.IdPersona == dto.IdPersona))
            return (false, "Esta persona ya es un empleado registrado.", null);

        var nuevoEmpleado = new Empleado
        {
            IdPersona = dto.IdPersona,
            IdPuestoTrabajo = dto.IdPuestoTrabajo
        };

        context.Empleados.Add(nuevoEmpleado);
        await context.SaveChangesAsync();

        return (true, "Empleado creado con éxito.", nuevoEmpleado.IdEmpleado);
    }

    public async Task<(bool Success, string Message)> PatchAsync(int idEmpleado, EmpleadoPatchDTO dto)
    {
        var empleado = await context.Empleados
            .Include(e => e.Persona)
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);

        if (empleado == null) return (false, "Empleado no encontrado.");

        // Actualización parcial del puesto
        if (dto.IdPuestoTrabajo.HasValue)
        {
            if (!await context.PuestosTrabajo.AnyAsync(p => p.IdPuestoTrabajo == dto.IdPuestoTrabajo))
                return (false, "El nuevo puesto no es válido.");
            empleado.IdPuestoTrabajo = dto.IdPuestoTrabajo.Value;
        }

        // Actualización parcial de datos de contacto (Persona)
        if (empleado.Persona != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Gmail)) empleado.Persona.Gmail = dto.Gmail;
            if (!string.IsNullOrWhiteSpace(dto.Telefono)) empleado.Persona.Telefono = dto.Telefono;
        }

        await context.SaveChangesAsync();
        return (true, "Datos del empleado actualizados.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int idEmpleado)
    {
        var empleado = await context.Empleados.FindAsync(idEmpleado);
        if (empleado == null) return (false, "Empleado no existe.");

        context.Empleados.Remove(empleado);
        await context.SaveChangesAsync();
        return (true, "Empleado eliminado correctamente.");
    }
}