using System.Net.Http.Json;
using System.Text.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Service;

public class PuestoTrabajoService(HttpClient http) : IPuestoTrabajoService
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public async Task<PagedResponse<PuestoTrabajoResponseDto>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var url = $"api/puestotrabajo?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<PagedResponse<PuestoTrabajoResponseDto>>(url, _json)
               ?? new PagedResponse<PuestoTrabajoResponseDto>();
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(PuestoTrabajoCreateDto dto)
    {
        var response = await http.PostAsJsonAsync("api/puestotrabajo", dto);
        var content  = await response.Content.ReadAsStringAsync();
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root      = doc.RootElement;
            var mensaje   = root.TryGetProperty("mensaje", out var m) ? m.GetString() ?? "" : content;
            int? id       = root.TryGetProperty("id", out var i) && i.ValueKind != JsonValueKind.Null ? i.GetInt32() : null;
            return (response.IsSuccessStatusCode, mensaje, id);
        }
        catch { return (false, content, null); }
    }

    public async Task<(bool Success, string Message)> PatchAsync(int id, PuestotrabajoPatchDto dto)
    {
        var response = await http.PatchAsJsonAsync($"api/puestotrabajo/{id}", dto);
        var content  = await response.Content.ReadAsStringAsync();
        try
        {
            using var doc = JsonDocument.Parse(content);
            var mensaje   = doc.RootElement.TryGetProperty("mensaje", out var m) ? m.GetString() ?? "" : content;
            return (response.IsSuccessStatusCode, mensaje);
        }
        catch { return (false, content); }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/puestotrabajo/{id}");
        var content  = await response.Content.ReadAsStringAsync();
        try
        {
            using var doc = JsonDocument.Parse(content);
            var mensaje   = doc.RootElement.TryGetProperty("mensaje", out var m) ? m.GetString() ?? "" : content;
            return (response.IsSuccessStatusCode, mensaje);
        }
        catch { return (false, content); }
    }
}
