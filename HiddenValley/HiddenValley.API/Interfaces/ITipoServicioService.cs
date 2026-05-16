using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface ITipoServicioService
    {
        Task<IEnumerable<TipoServicioReadDto>> GetAllAsync();
        Task<TipoServicioReadDto?> GetByIdAsync(int id);
        Task<TipoServicioReadDto> CreateAsync(TipoServicioCreateDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int id, TipoServicioCreateDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
