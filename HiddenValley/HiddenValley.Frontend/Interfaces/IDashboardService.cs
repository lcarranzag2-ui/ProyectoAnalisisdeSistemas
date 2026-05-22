using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces;

// contrato que deben implementar los servicios que consuman el endpoint del dashboard
public interface IDashboardService
{
    // obtiene el resumen operativo del dia: cabanas, personas y proximas reservas
    Task<DashboardResumenDto?> GetResumenAsync();
}
