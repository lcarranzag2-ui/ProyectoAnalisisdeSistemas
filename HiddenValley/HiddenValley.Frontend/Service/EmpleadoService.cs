using System.Net.Http.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly HttpClient _http;

    public EmpleadoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResponse<EmpleadoResponseDTO>> GetPagedAsync(string? search, int page, int pageSize)
    {
        // Llama al controlador de la API pasando los parámetros de paginación y búsqueda
        var url = $"api/empleado?search={search}&page={page}&pageSize={pageSize}";
        return await _http.GetFromJsonAsync<PagedResponse<EmpleadoResponseDTO>>(url) 
               ?? new PagedResponse<EmpleadoResponseDTO>();
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(EmpleadoCreateDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/empleado", dto);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            int? id = result != null && result.ContainsKey("id") ? Convert.ToInt32(result["id"]?.ToString()) : null;
            return (true, "Empleado creado exitosamente.", id);
        }
        
        var errorMsg = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al crear el empleado." : errorMsg, null);
    }

    public async Task<(bool Success, string Message)> PatchAsync(int idEmpleado, EmpleadoPatchDTO dto)
    {
        var response = await _http.PatchAsJsonAsync($"api/empleado/{idEmpleado}", dto);
        if (response.IsSuccessStatusCode)
        {
            return (true, "Empleado actualizado con éxito.");
        }

        var errorMsg = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al actualizar el empleado." : errorMsg);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int idEmpleado)
    {
        var response = await _http.DeleteAsync($"api/empleado/{idEmpleado}");
        if (response.IsSuccessStatusCode)
        {
            return (true, "Empleado eliminado con éxito.");
        }

        var errorMsg = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al eliminar el empleado." : errorMsg);
    }
}