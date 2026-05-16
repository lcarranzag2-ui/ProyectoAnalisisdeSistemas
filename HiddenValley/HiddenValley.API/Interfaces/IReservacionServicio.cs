using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface IReservacionServicio
    {
        Task<PagedResultReservacionServicio<ReservacionServicioReadDto>> GetPagedAsync(
            int pagina, int registrosPorPagina, string? cliente, int? idServicio, DateTime? fecha);
        Task<bool> CreateAsync(ReservacionServicioCreateDto dto);
        Task<bool> UpdateAsync(int idRes, int idSer, ReservacionServicioUpdateDto dto);
        Task<bool> DeleteAsync(int idRes, int idSer);
    }
}