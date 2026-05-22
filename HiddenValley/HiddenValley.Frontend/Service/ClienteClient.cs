using System.Net.Http.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Services;

public class ClienteClient : IClienteClient
{
    private readonly HttpClient _http;

    public ClienteClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResponse<ClienteDetalleDTO>> GetClientesAsync(string? search, int page, int pageSize)
    {
        var url = $"api/clientes?search={search}&page={page}&pageSize={pageSize}";
        var response = await _http.GetFromJsonAsync<PagedResponse<ClienteDetalleDTO>>(url);
        return response ?? new PagedResponse<ClienteDetalleDTO>();
    }

    public async Task<ClienteDetalleDTO?> BuscarClienteAsync(string filtro)
    {
        try
        {
            return await _http.GetFromJsonAsync<ClienteDetalleDTO>($"api/clientes/buscar?filtro={filtro}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<HistorialReservaDTO>> GetHistorialAsync(int idCliente)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<IEnumerable<HistorialReservaDTO>>($"api/clientes/{idCliente}/historial");
            return response ?? new List<HistorialReservaDTO>();
        }
        catch
        {
            return new List<HistorialReservaDTO>();
        }
    }

    public async Task<(bool Success, string Message, int? Id)> CrearClienteAsync(ClienteCreateDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/clientes", dto);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            return (true, "Cliente creado exitosamente", (int?)result?.Id);
        }
        var error = await response.Content.ReadAsStringAsync();
        return (false, error, null);
    }

    public async Task<(bool Success, string Message)> ActualizarClienteAsync(int idCliente, ClientePatchDTO dto)
    {
        var response = await _http.PatchAsJsonAsync($"api/clientes/{idCliente}", dto);
        if (response.IsSuccessStatusCode)
        {
            return (true, "Cliente actualizado exitosamente");
        }
        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<(bool Success, string Message)> EliminarClienteAsync(int idCliente)
    {
        var response = await _http.DeleteAsync($"api/clientes/{idCliente}");
        if (response.IsSuccessStatusCode)
        {
            return (true, "Cliente eliminado exitosamente");
        }
        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }
}