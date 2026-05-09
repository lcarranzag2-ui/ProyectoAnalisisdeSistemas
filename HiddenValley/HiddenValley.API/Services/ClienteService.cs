using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.Shared.DTOs;
using HiddenValley.API.Interfaces;

namespace HiddenValley.API.Services;

public class ClienteService(ApplicationDbContext context) : IClienteService
{
    public async Task<PagedResponse<ClienteDetalleDTO>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = context.Clientes.Include(c => c.Persona).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(c => 
                c.Persona!.Nombres.ToLower().Contains(search) || 
                c.Persona.Apellidos.ToLower().Contains(search) ||
                c.Persona.DPI!.Contains(search));
        }

        var totalRecords = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Persona!.Nombres)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClienteDetalleDTO
            {
                IdCliente = c.IdCliente,
                NombreCompleto = $"{c.Persona!.Nombres} {c.Persona.Apellidos}",
                DPI = c.Persona.DPI ?? "N/A",
                Telefono = c.Persona.Telefono,
                Email = c.Persona.Gmail ?? "N/A"
            }).ToListAsync();

        return new PagedResponse<ClienteDetalleDTO>
        {
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<ClienteDetalleDTO?> GetByIdOrFiltroAsync(string filtro)
    {
        return await context.Clientes
            .Include(c => c.Persona)
            .Where(c => c.Persona!.DPI == filtro || c.Persona.Telefono == filtro || c.IdCliente.ToString() == filtro)
            .Select(c => new ClienteDetalleDTO
            {
                IdCliente = c.IdCliente,
                NombreCompleto = $"{c.Persona!.Nombres} {c.Persona.Apellidos}",
                DPI = c.Persona.DPI ?? "N/A",
                Telefono = c.Persona.Telefono,
                Email = c.Persona.Gmail ?? "N/A"
            }).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<HistorialReservaDTO>> GetHistorialAsync(int idCliente)
    {
        return await context.RegistroReservacion
            .Where(r => r.IdCliente == idCliente)
            .OrderByDescending(r => r.FechaEntrada)
            .Select(r => new HistorialReservaDTO
            {
                Id = r.Id,
                FechaEntrada = r.FechaEntrada,
                FechaSalida = r.FechaSalida,
                Estado = r.EstadoReserva,
                Total = r.TotalPagar
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(ClienteCreateDTO dto)
    {
        if (!await context.Personas.AnyAsync(p => p.IdPersona == dto.IdPersona))
            return (false, "La persona no existe.", null);

        if (await context.Clientes.AnyAsync(c => c.IdPersona == dto.IdPersona))
            return (false, "Esta persona ya es cliente.", null);

        var nuevoCliente = new Cliente { IdPersona = dto.IdPersona };
        context.Clientes.Add(nuevoCliente);
        await context.SaveChangesAsync();

        return (true, "Cliente creado", nuevoCliente.IdCliente);
    }

    public async Task<(bool Success, string Message)> PatchAsync(int idCliente, ClientePatchDTO dto)
    {
        var cliente = await context.Clientes.Include(c => c.Persona)
            .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

        if (cliente?.Persona == null) return (false, "Cliente no encontrado");

        if (!string.IsNullOrWhiteSpace(dto.Gmail)) cliente.Persona.Gmail = dto.Gmail;
        if (!string.IsNullOrWhiteSpace(dto.Telefono)) cliente.Persona.Telefono = dto.Telefono;

        await context.SaveChangesAsync();
        return (true, "Cliente actualizado");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int idCliente)
    {
        var cliente = await context.Clientes.FindAsync(idCliente);
        if (cliente == null) return (false, "Cliente no encontrado");

        if (await context.RegistroReservacion.AnyAsync(r => r.IdCliente == idCliente))
            return (false, "No se puede eliminar un cliente con historial.");

        context.Clientes.Remove(cliente);
        await context.SaveChangesAsync();
        return (true, "Cliente eliminado");
    }
}