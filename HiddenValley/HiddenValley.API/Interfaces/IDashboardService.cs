using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface IDashboardService
    {
        // retorna el resumen completo con las metricas del dia
        Task<DashboardResumenDto> GetResumenAsync();
    }
}
