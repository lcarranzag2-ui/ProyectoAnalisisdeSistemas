using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Models;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Services;

public class ReservacionService(ApplicationDbContext context) : IReservacionService
{
    private const string EstadoRecibida   = "Recibida";
    private const string EstadoConfirmada = "Confirmada";
    private const string EstadoCancelada  = "Cancelada";
    private const string EstadoPagada     = "Pagada";
    private const int    EmpleadoDefault  = 1;

    private static readonly string[] EstadosValidos = [EstadoRecibida, EstadoConfirmada, EstadoCancelada, EstadoPagada];

    public async Task<PagedResponse<ReservacionDetalleDto>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            if (int.TryParse(search, out var id))
            {
                query = query.Where(r => r.Id == id || r.IdCliente == id);
            }
            else
            {
                var term = search.Trim().ToLower();
                query = query.Where(r =>
                    r.Cliente != null && r.Cliente.Persona != null &&
                    (r.Cliente.Persona.Nombres + " " + r.Cliente.Persona.Apellidos).ToLower().Contains(term));
            }
        }

        var totalRecords = await query.CountAsync();
        var entidades = await query
            .OrderByDescending(r => r.FechaEntrada)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<ReservacionDetalleDto>
        {
            TotalRecords = totalRecords,
            TotalPages   = (int)Math.Ceiling(totalRecords / (double)pageSize),
            CurrentPage  = page,
            PageSize     = pageSize,
            Items        = entidades.Select(MapearADetalle).ToList()
        };
    }

    public async Task<ReservacionDetalleDto?> GetByIdAsync(int id)
    {
        var reserva = await context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .FirstOrDefaultAsync(r => r.Id == id);

        return reserva is null ? null : MapearADetalle(reserva);
    }

    public async Task<(bool Success, string Message, int? Id, decimal? Total, int? Noches)> CreateAsync(ReservacionCreateDto dto)
    {
        var errorFechas = ValidarFechas(dto.FechaEntrada, dto.FechaSalida, esCreacion: true);
        if (errorFechas != null) return (false, errorFechas, null, null, null);

        var cliente = await context.Clientes
            .Include(c => c.Persona)
            .FirstOrDefaultAsync(c => c.IdCliente == dto.IdCliente);

        if (cliente is null)
            return (false, $"El cliente con id {dto.IdCliente} no existe.", null, null, null);

        if (cliente.Persona?.Telefono != dto.Telefono)
            return (false, "El teléfono no coincide con el cliente proporcionado.", null, null, null);

        var clienteTieneReservaActiva = await context.RegistroReservacion
            .AnyAsync(r => r.IdCliente == dto.IdCliente && r.EstadoReserva != EstadoCancelada);
        if (clienteTieneReservaActiva)
            return (false, "El cliente ya tiene otra reserva activa.", null, null, null);

        var cabana = await context.Cabanas
            .Include(c => c.TipoCabana)
            .FirstOrDefaultAsync(c => c.IdCabana == dto.IdCabana);

        if (cabana is null)
            return (false, $"La cabaña con id {dto.IdCabana} no existe.", null, null, null);

        if (cabana.TipoCabana is null)
            return (false, "La cabaña no tiene un tipo asignado.", null, null, null);

        if (dto.CantidadPersonas > cabana.TipoCabana.Capacidad)
            return (false, $"La capacidad de la cabaña es {cabana.TipoCabana.Capacidad} personas y se solicitaron {dto.CantidadPersonas}.", null, null, null);

        var hayTraslape = await context.RegistroReservacion.AnyAsync(r =>
            r.IdCabana == dto.IdCabana &&
            r.EstadoReserva != EstadoCancelada &&
            r.FechaEntrada < dto.FechaSalida &&
            r.FechaSalida > dto.FechaEntrada);
        if (hayTraslape)
            return (false, "La cabaña ya tiene una reserva en esas fechas.", null, null, null);

        var noches = Math.Max(1, (dto.FechaSalida.Date - dto.FechaEntrada.Date).Days);
        var total  = noches * cabana.TipoCabana.Precio;

        var nueva = new RegistroReservacion
        {
            FechaEntrada     = dto.FechaEntrada.Date,
            FechaSalida      = dto.FechaSalida.Date,
            CantidadPersonas = dto.CantidadPersonas,
            EstadoReserva    = EstadoRecibida,
            TotalPagar       = total,
            IdCliente        = dto.IdCliente,
            IdCabana         = dto.IdCabana,
            IdEmpleado       = dto.IdEmpleado ?? EmpleadoDefault
        };

        context.RegistroReservacion.Add(nueva);
        await context.SaveChangesAsync();

        return (true, "Reservación creada con éxito.", nueva.Id, nueva.TotalPagar, noches);
    }

    public async Task<(bool Success, string Message, ReservacionDetalleDto? Data)> PatchAsync(int id, ReservacionPatchDto dto)
    {
        var reserva = await context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva is null)
            return (false, $"No existe la reservación con id {id}.", null);

        if (reserva.EstadoReserva == EstadoCancelada)
            return (false, "No se puede actualizar una reservación cancelada.", null);

        // Cambio de estado
        if (dto.EstadoReserva != null)
        {
            if (!EstadosValidos.Contains(dto.EstadoReserva))
                return (false, $"Estado inválido. Los estados permitidos son: {string.Join(", ", EstadosValidos)}.", null);

            if (reserva.EstadoReserva == EstadoPagada)
                return (false, "No se puede cambiar el estado de una reservación ya pagada.", null);

            reserva.EstadoReserva = dto.EstadoReserva;
        }

        // Actualización de campos de datos
        var nuevaFechaEntrada = dto.FechaEntrada ?? reserva.FechaEntrada;
        var nuevaFechaSalida  = dto.FechaSalida  ?? reserva.FechaSalida;
        var nuevaCantidad     = dto.CantidadPersonas ?? reserva.CantidadPersonas;
        var nuevaIdCabana     = dto.IdCabana ?? reserva.IdCabana;

        var errorFechas = ValidarFechas(nuevaFechaEntrada, nuevaFechaSalida, esCreacion: false);
        if (errorFechas != null) return (false, errorFechas, null);

        Cabana? cabanaDestino = reserva.Cabana;
        if (dto.IdCabana.HasValue && dto.IdCabana.Value != reserva.IdCabana)
        {
            cabanaDestino = await context.Cabanas
                .Include(c => c.TipoCabana)
                .FirstOrDefaultAsync(c => c.IdCabana == dto.IdCabana.Value);

            if (cabanaDestino is null)
                return (false, $"La cabaña con id {dto.IdCabana.Value} no existe.", null);
        }

        if (cabanaDestino?.TipoCabana is null)
            return (false, "La cabaña no tiene un tipo asignado.", null);

        if (nuevaCantidad > cabanaDestino.TipoCabana.Capacidad)
            return (false, $"La capacidad de la cabaña es {cabanaDestino.TipoCabana.Capacidad} personas y se solicitaron {nuevaCantidad}.", null);

        var hayTraslape = await context.RegistroReservacion.AnyAsync(r =>
            r.Id != id &&
            r.IdCabana == nuevaIdCabana &&
            r.EstadoReserva != EstadoCancelada &&
            r.FechaEntrada < nuevaFechaSalida &&
            r.FechaSalida > nuevaFechaEntrada);
        if (hayTraslape)
            return (false, "La cabaña ya tiene una reserva en esas fechas.", null);

        if (dto.Telefono != null && reserva.Cliente?.Persona?.Telefono != dto.Telefono)
            return (false, "El teléfono no coincide con el cliente de la reservación.", null);

        reserva.FechaEntrada     = nuevaFechaEntrada.Date;
        reserva.FechaSalida      = nuevaFechaSalida.Date;
        reserva.CantidadPersonas = nuevaCantidad;
        reserva.IdCabana         = nuevaIdCabana;

        var noches = Math.Max(1, (reserva.FechaSalida.Date - reserva.FechaEntrada.Date).Days);
        reserva.TotalPagar = noches * cabanaDestino.TipoCabana.Precio;

        await context.SaveChangesAsync();

        return (true, "Reservación actualizada con éxito.", MapearADetalle(reserva));
    }

    private static string? ValidarFechas(DateTime entrada, DateTime salida, bool esCreacion)
    {
        if (entrada == default || salida == default)
            return "Las fechas son obligatorias.";

        if (salida <= entrada)
            return "La fecha de salida debe ser mayor a la fecha de entrada.";

        if (esCreacion && entrada.Date < DateTime.UtcNow.Date)
            return "La fecha de entrada no puede ser anterior a hoy.";

        return null;
    }

    private static ReservacionDetalleDto MapearADetalle(RegistroReservacion r) => new()
    {
        Id               = r.Id,
        FechaEntrada     = r.FechaEntrada,
        FechaSalida      = r.FechaSalida,
        CantidadPersonas = r.CantidadPersonas,
        EstadoReserva    = r.EstadoReserva,
        TotalPagar       = r.TotalPagar,
        IdCliente        = r.IdCliente,
        NombreCliente    = r.Cliente?.Persona != null
            ? r.Cliente.Persona.Nombres + " " + r.Cliente.Persona.Apellidos
            : "Desconocido",
        TelefonoCliente  = r.Cliente?.Persona?.Telefono ?? string.Empty,
        IdCabana         = r.IdCabana,
        TipoCabana       = r.Cabana?.TipoCabana?.Nombre ?? "Desconocido",
        CapacidadCabana  = r.Cabana?.TipoCabana?.Capacidad ?? 0,
        IdEmpleado       = r.IdEmpleado
    };
}
