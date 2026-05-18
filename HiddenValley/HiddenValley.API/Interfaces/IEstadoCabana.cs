using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces;

public interface IEstadoCabanaService
{
    Task<IEnumerable<EstadoCabanaDto>> GetAllAsync();
    Task<EstadoCabanaDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int? Id)> CreateAsync(EstadoCabanaDto dto);
    Task<(bool Success, string Message)> PatchAsync(int id, EstadoCabanaDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}