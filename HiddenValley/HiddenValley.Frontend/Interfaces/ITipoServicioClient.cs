using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces
{
    public interface ITipoServicioClient
    {
        Task<List<TipoServicioReadDto>> GetAllAsync();
        Task<bool> CrearAsync(TipoServicioCreateDto dto);
        Task<bool> ActualizarAsync(int id, TipoServicioCreateDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
