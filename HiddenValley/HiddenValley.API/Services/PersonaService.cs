using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Services;

public class PersonaService(ApplicationDbContext context) : IPersonaService
{
    public async Task<PagedResponse<PersonaResponseDto>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = context.Personas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p => 
                p.Nombres.ToLower().Contains(search) || 
                p.Apellidos.ToLower().Contains(search) ||
                p.DPI!.Contains(search));
        }

        var totalRecords = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Nombres)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PersonaResponseDto
            {
                IdPersona = p.IdPersona,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                DPI = p.DPI ?? "N/A",
                Telefono = p.Telefono,
                Gmail = p.Gmail ?? "N/A",
                Direccion = p.Direccion
            }).ToListAsync();

        return new PagedResponse<PersonaResponseDto>
        {
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(PersonaCreateDto dto)
    {
        if (await context.Personas.AnyAsync(p => p.DPI == dto.DPI))
            return (false, "Ya existe una persona con ese DPI.", null);

        var persona = new Persona
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            FechaNacimiento = dto.FechaNacimiento,
            DPI = dto.DPI,
            Telefono = dto.Telefono,
            Gmail = dto.Gmail,
            Direccion = dto.Direccion
        };

        context.Personas.Add(persona);
        await context.SaveChangesAsync();
        return (true, "Persona creada", persona.IdPersona);
    }

    public async Task<bool> ExisteDpiAsync(string dpi) => await context.Personas.AnyAsync(p => p.DPI == dpi);

    public async Task<(bool Success, string Message)> PatchAsync(int id, PersonaPatchDto dto)
    {
        var persona = await context.Personas.FindAsync(id);
        if (persona == null) return (false, "Persona no encontrada");

        if (dto.Telefono != null) persona.Telefono = dto.Telefono;
        if (dto.Gmail != null) persona.Gmail = dto.Gmail;
        if (dto.Direccion != null) persona.Direccion = dto.Direccion;

        await context.SaveChangesAsync();
        return (true, "Actualizado");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var persona = await context.Personas.FindAsync(id);
        if (persona == null) return (false, "No encontrada");

        if (await context.Empleados.AnyAsync(e => e.IdPersona == id) || await context.Clientes.AnyAsync(c => c.IdPersona == id))
            return (false, "Persona vinculada a otros registros.");

        context.Personas.Remove(persona);
        await context.SaveChangesAsync();
        return (true, "Eliminado");
    }
}