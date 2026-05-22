using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces;

public interface IClienteClient
{
    Task<PagedResponse<ClienteDetalleDTO>> GetClientesAsync(string? search, int page, int pageSize);
    Task<ClienteDetalleDTO?> BuscarClienteAsync(string filtro);
    Task<IEnumerable<HistorialReservaDTO>> GetHistorialAsync(int idCliente);
    Task<(bool Success, string Message, int? Id)> CrearClienteAsync(ClienteCreateDTO dto);
    Task<(bool Success, string Message)> ActualizarClienteAsync(int idCliente, ClientePatchDTO dto);
    Task<(bool Success, string Message)> EliminarClienteAsync(int idCliente);
}