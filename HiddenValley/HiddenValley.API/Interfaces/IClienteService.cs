using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface IClienteService
    {
    Task<PagedResponse<ClienteDetalleDTO>> GetPagedAsync(string? search, int page, int pageSize);
    Task<(bool Success, string Message, int? Id)> CreateAsync(ClienteCreateDTO dto);
    Task<ClienteDetalleDTO?> GetByIdOrFiltroAsync(string filtro);
    Task<IEnumerable<HistorialReservaDTO>> GetHistorialAsync(int idCliente);
    Task<(bool Success, string Message)> PatchAsync(int idCliente, ClientePatchDTO dto);
    Task<(bool Success, string Message)> DeleteAsync(int idCliente);
    }
}