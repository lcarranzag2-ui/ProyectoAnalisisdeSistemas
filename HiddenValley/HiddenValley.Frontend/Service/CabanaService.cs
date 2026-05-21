using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Service;

public class CabanaService : ICabanasService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CabanaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Configuración para ignorar mayúsculas/minúsculas del JSON del backend
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<PagedResponse<object>?> GetPagedAsync(string? searchTerm, int page, int pageSize)
    {
        var url = $"api/cabanas?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            url += $"&search={Uri.EscapeDataString(searchTerm)}";
        }
        return await _httpClient.GetFromJsonAsync<PagedResponse<object>>(url);
    }

    public async Task<IEnumerable<object>?> GetDisponibilidadAsync(DateTime inicio, DateTime fin)
    {
        var url = $"api/cabanas/disponibilidad?fechaInicio={inicio:yyyy-MM-ddTHH:mm:ss}&fechaFin={fin:yyyy-MM-ddTHH:mm:ss}";
        return await _httpClient.GetFromJsonAsync<IEnumerable<object>>(url);
    }

    public async Task<(bool Success, string Message, object? Data)> CambiarEstadoAsync(CambiarEstadoRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/cabanas/cambiar-estado", request);
        var contentString = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(contentString);
            var root = doc.RootElement;
            
            bool success = root.GetProperty("success").GetBoolean();
            string message = root.GetProperty("message").GetString() ?? "";
            object? data = root.TryGetProperty("data", out var dataProp) ? dataProp : null;
            
            return (success, message, data);
        }
        catch
        {
            return (response.IsSuccessStatusCode, contentString, null);
        }
    }

    public async Task<(bool Success, string Message, int? Id)> RegistrarCabanaAsync(RegistrarCabanaRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/cabanas/registrar", request);
        var contentString = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(contentString);
            var root = doc.RootElement;
            
            // Tu API devuelve 'mensaje' e 'id' en minúsculas en el BadRequest/Ok
            string message = root.TryGetProperty("mensaje", out var m) ? m.GetString()! : contentString;
            int? id = root.TryGetProperty("id", out var i) && i.ValueKind != JsonValueKind.Null ? i.GetInt32() : null;
            
            return (response.IsSuccessStatusCode, message, id);
        }
        catch
        {
            return (response.IsSuccessStatusCode, contentString, null);
        }
    }

    public async Task<(bool Success, string Message)> EliminarCabanaAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/cabanas/{id}");
        var contentString = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(contentString);
            var root = doc.RootElement;
            
            string message = root.TryGetProperty("mensaje", out var m) ? m.GetString()! : contentString;
            return (response.IsSuccessStatusCode, message);
        }
        catch
        {
            return (response.IsSuccessStatusCode, contentString);
        }
    }
}