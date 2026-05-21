using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces;

public interface IReservacionService
{
    Task<PagedResponse<ReservacionDetalleDto>> GetPagedAsync(string? search, int page, int pageSize);
    Task<ReservacionDetalleDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int? Id, decimal? Total, int? Noches)> CreateAsync(ReservacionCreateDto dto);
    Task<(bool Success, string Message, ReservacionDetalleDto? Data)> PatchAsync(int id, ReservacionPatchDto dto);
}
