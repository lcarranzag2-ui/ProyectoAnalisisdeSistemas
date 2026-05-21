using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data;
using HiddenValley.API.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Services;

public class DashboardService(ApplicationDbContext context) : IDashboardService
{
    public async Task<DashboardResumenDto> GetResumenAsync()
    {
        var hoy = DateTime.Today;
        var manana = hoy.AddDays(1);

        // cabanas que tienen reserva activa hoy (fecha actual entre entrada y salida, estados activos)
        var idsCabanasOcupadasHoy = await context.RegistroReservacion
            .Where(r => r.FechaEntrada < manana && r.FechaSalida > hoy
                     && r.EstadoReserva != "Cancelada")
            .Select(r => r.IdCabana)
            .Distinct()
            .ToListAsync();

        var cabanasOcupadasHoy = idsCabanasOcupadasHoy.Count;

        // cabanas con estado "Disponible" que no estan ocupadas hoy
        var cabanasDisponibles = await context.Cabanas
            .Include(c => c.EstadoCabana)
            .Where(c => c.EstadoCabana != null
                     && c.EstadoCabana.Nombre == "Disponible"
                     && !idsCabanasOcupadasHoy.Contains(c.IdCabana))
            .CountAsync();

        // suma de personas de las reservas cuyo check-in es hoy exactamente
        var personasEsperadasHoy = await context.RegistroReservacion
            .Where(r => r.FechaEntrada >= hoy && r.FechaEntrada < manana
                     && r.EstadoReserva != "Cancelada")
            .SumAsync(r => (int?)r.CantidadPersonas) ?? 0;

        // proximas 5 reservaciones a partir de hoy, ordenadas cronologicamente
        var proximasReservas = await context.RegistroReservacion
            .Include(r => r.Cliente)!.ThenInclude(c => c!.Persona)
            .Include(r => r.Cabana)!.ThenInclude(c => c!.TipoCabana)
            .Where(r => r.FechaEntrada >= hoy && r.EstadoReserva != "Cancelada")
            .OrderBy(r => r.FechaEntrada)
            .Take(5)
            .Select(r => new DashboardReservaDto
            {
                Id = r.Id,
                NombreCliente = r.Cliente != null && r.Cliente.Persona != null
                    ? r.Cliente.Persona.Nombres + " " + r.Cliente.Persona.Apellidos
                    : "Desconocido",
                TipoCabana = r.Cabana != null && r.Cabana.TipoCabana != null
                    ? r.Cabana.TipoCabana.Nombre
                    : "Desconocido",
                FechaEntrada = r.FechaEntrada,
                FechaSalida = r.FechaSalida,
                CantidadPersonas = r.CantidadPersonas,
                EstadoReserva = r.EstadoReserva
            })
            .ToListAsync();

        return new DashboardResumenDto
        {
            CabanasOcupadasHoy = cabanasOcupadasHoy,
            CabanasDisponibles = cabanasDisponibles,
            PersonasEsperadasHoy = personasEsperadasHoy,
            ProximasReservas = proximasReservas
        };
    }
}
