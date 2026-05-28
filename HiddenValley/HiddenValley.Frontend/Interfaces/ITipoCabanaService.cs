using System.Collections.Generic;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Interfaces
{
    public interface ITipoCabanaService
    {
        Task<IEnumerable<TipoCabanaDTO>> GetAllAsync();
        Task<TipoCabanaDTO?> GetByIdAsync(int id);
        Task<TipoCabanaDTO> CreateAsync(TipoCabanaCreateDTO dto);
        Task<(bool Success, string Message)> UpdateAsync(int id, TipoCabanaCreateDTO dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}