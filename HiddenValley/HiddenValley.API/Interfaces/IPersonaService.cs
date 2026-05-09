using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface IPersonaService
    {
    Task<PagedResponse<PersonaResponseDto>> GetPagedAsync(string? search, int page, int pageSize);
    Task<(bool Success, string Message, int? Id)> CreateAsync(PersonaCreateDto dto);
    Task<(bool Success, string Message)> PatchAsync(int id, PersonaPatchDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<bool> ExisteDpiAsync(string dpi);
    }
}