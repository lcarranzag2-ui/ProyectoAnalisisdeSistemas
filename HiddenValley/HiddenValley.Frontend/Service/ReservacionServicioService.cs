using System.Net.Http.Json;
using System.Web;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Services;

public class ReservacionServicioService : IReservacionServicioService
{
    private readonly HttpClient _http;

    public ReservacionServicioService(HttpClient http) => _http = http;

    public async Task<PagedResultReservacionServicio<ReservacionServicioReadDto>?> GetPagedAsync(int page, int size, string? buscar, int? idServicio, DateTime? fecha)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["page"] = page.ToString();
        query["size"] = size.ToString();
        
        if (!string.IsNullOrWhiteSpace(buscar)) query["buscar"] = buscar;
        if (idServicio.HasValue) query["idServicio"] = idServicio.Value.ToString();
        if (fecha.HasValue) query["fecha"] = fecha.Value.ToString("yyyy-MM-dd");

        return await _http.GetFromJsonAsync<PagedResultReservacionServicio<ReservacionServicioReadDto>>($"api/ReservacionServicio?{query}");
    }

    public async Task<bool> CreateAsync(ReservacionServicioCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/ReservacionServicio", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(int idRes, int idSer, ReservacionServicioUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/ReservacionServicio/{idRes}/{idSer}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int idRes, int idSer)
    {
        var response = await _http.DeleteAsync($"api/ReservacionServicio/{idRes}/{idSer}");
        return response.IsSuccessStatusCode;
    }
}