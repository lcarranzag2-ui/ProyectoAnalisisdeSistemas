using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Services;

public class EstadoCabanaService(ApplicationDbContext context) : IEstadoCabanaService
{
    public async Task<IEnumerable<EstadoCabanaDto>> GetAllAsync()
    {
        return await context.EstadosCabanas
            .Select(e => new EstadoCabanaDto
            {
                IdEstadoCabana = e.IdEstadoCabana,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion
            }).ToListAsync();
    }

    public async Task<EstadoCabanaDto?> GetByIdAsync(int id)
    {
        var estado = await context.EstadosCabanas.FindAsync(id);
        if (estado == null) return null;

        return new EstadoCabanaDto
        {
            IdEstadoCabana = estado.IdEstadoCabana,
            Nombre = estado.Nombre,
            Descripcion = estado.Descripcion
        };
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(EstadoCabanaDto dto)
    {
        if (await context.EstadosCabanas.AnyAsync(e => e.Nombre.ToLower() == dto.Nombre.ToLower()))
            return (false, "Ya existe un estado con este nombre.", null);

        var nuevo = new EstadoCabana
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        context.EstadosCabanas.Add(nuevo);
        await context.SaveChangesAsync();

        return (true, "Estado creado exitosamente.", nuevo.IdEstadoCabana);
    }

    public async Task<(bool Success, string Message)> PatchAsync(int id, EstadoCabanaDto dto)
    {
        var estado = await context.EstadosCabanas.FindAsync(id);
        if (estado == null) return (false, "Estado no encontrado.");

        if (!string.IsNullOrWhiteSpace(dto.Nombre)) estado.Nombre = dto.Nombre;
        if (dto.Descripcion != null) estado.Descripcion = dto.Descripcion;

        await context.SaveChangesAsync();
        return (true, "Estado actualizado correctamente.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var estado = await context.EstadosCabanas.FindAsync(id);
        if (estado == null) return (false, "Estado no encontrado.");

        context.EstadosCabanas.Remove(estado);
        await context.SaveChangesAsync();

        return (true, "Estado eliminado con éxito.");
    }
}