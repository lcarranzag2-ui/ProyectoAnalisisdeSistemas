using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Services;

public class TipoCabanaService(ApplicationDbContext context) : ITipoCabanaService
{
    public async Task<IEnumerable<TipoCabanaDTO>> GetAllAsync()
    {
        // Cambiado context.TipoCabanas por context.TiposCabana
        return await context.TiposCabana
            .Select(t => new TipoCabanaDTO
            {
                IdTipoCabana = t.IdTipoCabana,
                Nombre = t.Nombre,
                Precio = t.Precio
            })
            .ToListAsync();
    }

    public async Task<TipoCabanaDTO?> GetByIdAsync(int id)
    {
        var tipo = await context.TiposCabana.FindAsync(id);
        if (tipo == null) return null;

        return new TipoCabanaDTO
        {
            IdTipoCabana = tipo.IdTipoCabana,
            Nombre = tipo.Nombre,
            Precio = tipo.Precio
        };
    }

    public async Task<TipoCabanaDTO> CreateAsync(TipoCabanaCreateDTO dto)
    {
        var nuevoTipo = new TipoCabana
        {
            Nombre = dto.Nombre,
            Precio = dto.Precio
        };

        context.TiposCabana.Add(nuevoTipo);
        await context.SaveChangesAsync();

        return new TipoCabanaDTO
        {
            IdTipoCabana = nuevoTipo.IdTipoCabana,
            Nombre = nuevoTipo.Nombre,
            Precio = nuevoTipo.Precio
        };
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, TipoCabanaCreateDTO dto)
    {
        var tipo = await context.TiposCabana.FindAsync(id);
        if (tipo == null) return (false, "Tipo de cabaña no encontrado.");

        if (!string.IsNullOrWhiteSpace(dto.Nombre)) tipo.Nombre = dto.Nombre;
        if (dto.Precio > 0) tipo.Precio = dto.Precio;

        await context.SaveChangesAsync();
        return (true, "Actualizado correctamente.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var tipo = await context.TiposCabana.FindAsync(id);
        if (tipo == null) return (false, "Tipo de cabaña no encontrado.");

        var tieneCabanas = await context.Cabanas.AnyAsync(c => c.IdTipoCabana == id);
        if (tieneCabanas) return (false, "No se puede eliminar: tiene cabañas asociadas.");

        context.TiposCabana.Remove(tipo);
        await context.SaveChangesAsync();
        return (true, "Eliminado correctamente.");
    }
}