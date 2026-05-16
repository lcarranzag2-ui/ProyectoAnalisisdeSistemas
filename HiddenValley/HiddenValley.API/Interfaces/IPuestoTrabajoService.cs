using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces;

public interface IPuestoTrabajoService
{
    Task<PagedResponse<PuestoTrabajoResponseDto>> GetPagedAsync(string? search, int page, int pageSize);
    Task<(bool Success, string Message, int? Id)> CreateAsync(PuestoTrabajoCreateDto dto);
    Task<(bool Success, string Message)> PatchAsync(int id, PuestotrabajoPatchDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}
