using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces;

// contrato del cliente http para obtener el resumen del dashboard
public interface IDashboardService
{
    Task<DashboardResumenDto?> GetResumenAsync();
}
