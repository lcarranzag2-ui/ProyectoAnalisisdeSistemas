using System.Net.Http.Json;
using System.Text.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Service;

public class ReservacionService(HttpClient http) : IReservacionService
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public async Task<PagedResponse<ReservacionDetalleDto>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var url = $"api/reservaciones?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<PagedResponse<ReservacionDetalleDto>>(url, _json)
               ?? new PagedResponse<ReservacionDetalleDto>();
    }

    public async Task<ReservacionDetalleDto?> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<ReservacionDetalleDto>($"api/reservaciones/{id}", _json);

    public async Task<(bool Success, string Message, int? Id, decimal? Total, int? Noches)> CreateAsync(ReservacionCreateDto dto)
    {
        var response = await http.PostAsJsonAsync("api/reservaciones", dto);
        var content  = await response.Content.ReadAsStringAsync();
        try
        {
            using var doc  = JsonDocument.Parse(content);
            var root       = doc.RootElement;
            var mensaje    = root.TryGetProperty("mensaje",    out var m) ? m.GetString() ?? "" : content;
            int?    id     = root.TryGetProperty("id",         out var i) && i.ValueKind != JsonValueKind.Null ? i.GetInt32()     : null;
            decimal? total = root.TryGetProperty("totalPagar", out var t) && t.ValueKind != JsonValueKind.Null ? t.GetDecimal()   : null;
            int? noches    = root.TryGetProperty("noches",     out var n) && n.ValueKind != JsonValueKind.Null ? n.GetInt32()     : null;
            return (response.IsSuccessStatusCode, mensaje, id, total, noches);
        }
        catch { return (false, content, null, null, null); }
    }

    public async Task<(bool Success, string Message, ReservacionDetalleDto? Data)> PatchAsync(int id, ReservacionPatchDto dto)
    {
        var response = await http.PatchAsJsonAsync($"api/reservaciones/{id}", dto);
        var content  = await response.Content.ReadAsStringAsync();
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root      = doc.RootElement;
            var mensaje   = root.TryGetProperty("mensaje", out var m) ? m.GetString() ?? "" : content;
            ReservacionDetalleDto? data = null;
            if (root.TryGetProperty("reservacion", out var r) && r.ValueKind == JsonValueKind.Object)
                data = JsonSerializer.Deserialize<ReservacionDetalleDto>(r.GetRawText(), _json);
            return (response.IsSuccessStatusCode, mensaje, data);
        }
        catch { return (false, content, null); }
    }
}
