using System.Net.Http.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Services;

public class PersonaClient : IPersonaClient
{
    private readonly HttpClient _http;

    public PersonaClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResponsePersona<PersonaResponseDto>> GetPersonasAsync(string? search, int page, int pageSize)
    {
        var url = $"api/personas?search={search}&page={page}&pageSize={pageSize}";
        return await _http.GetFromJsonAsync<PagedResponsePersona<PersonaResponseDto>>(url) 
               ?? new PagedResponsePersona<PersonaResponseDto>();
    }

    public async Task<bool> CrearPersonaAsync(PersonaCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/personas", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarPersonaAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/personas/{id}");
        return response.IsSuccessStatusCode;
    }
}