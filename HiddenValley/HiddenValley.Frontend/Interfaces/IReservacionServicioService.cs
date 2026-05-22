using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces;

public interface IReservacionServicioService
{
    Task<PagedResultReservacionServicio<ReservacionServicioReadDto>?> GetPagedAsync(int page, int size, string? buscar, int? idServicio, DateTime? fecha);
    Task<bool> CreateAsync(ReservacionServicioCreateDto dto);
    Task<bool> UpdateAsync(int idRes, int idSer, ReservacionServicioUpdateDto dto);
    Task<bool> DeleteAsync(int idRes, int idSer);
}