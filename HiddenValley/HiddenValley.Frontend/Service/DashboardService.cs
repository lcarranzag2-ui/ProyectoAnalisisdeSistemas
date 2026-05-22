using System.Net.Http.Json;
using System.Text.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Service;

// cliente HTTP que consume el endpoint GET /api/dashboard del backend
public class DashboardService(HttpClient http) : IDashboardService
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public async Task<DashboardResumenDto?> GetResumenAsync()
    {
        try
        {
            return await http.GetFromJsonAsync<DashboardResumenDto>("api/dashboard", _json);
        }
        catch
        {
            // si el backend no esta disponible se devuelve null y la vista lo maneja
            return null;
        }
    }
}
