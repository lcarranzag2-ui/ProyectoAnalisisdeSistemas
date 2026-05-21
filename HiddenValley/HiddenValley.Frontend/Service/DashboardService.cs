using System.Net.Http.Json;
using System.Text.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Service;

public class DashboardService(HttpClient http) : IDashboardService
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    // hace la peticion GET al backend y retorna el dto con las metricas del dia
    public async Task<DashboardResumenDto?> GetResumenAsync()
    {
        try
        {
            return await http.GetFromJsonAsync<DashboardResumenDto>("api/dashboard", _json);
        }
        catch
        {
            return null;
        }
    }
}
