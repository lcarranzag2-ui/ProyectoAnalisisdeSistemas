using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces;

public interface IPersonaClient
{
    Task<PagedResponsePersona<PersonaResponseDto>> GetPersonasAsync(string? search = null, int page = 1, int pageSize = 10);
    Task<bool> CrearPersonaAsync(PersonaCreateDto dto);
    Task<bool> EliminarPersonaAsync(int id);
}