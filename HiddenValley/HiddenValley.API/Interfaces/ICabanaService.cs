using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface ICabanaService
    {
    Task<PagedResponse<object>> GetPagedAsync(string? searchTerm, int page, int pageSize);
    Task<IEnumerable<object>> GetDisponibilidadAsync(DateTime inicio, DateTime fin);
    Task<(bool Success, string Message, object? Data)> CambiarEstadoAsync(CambiarEstadoRequest request);
    Task<(bool Success, string Message, int? Id)> RegistrarCabanaAsync(RegistrarCabanaRequest request);
    Task<(bool Success, string Message)> EliminarCabanaAsync(int id);
         
    }
}