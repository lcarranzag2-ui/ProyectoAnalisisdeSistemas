using HiddenValley.Shared.DTOs;
using HiddenValley.API.Models;

namespace HiddenValley.API.Interfaces;

public interface ITipoCabanaService
{
    Task<IEnumerable<TipoCabanaDTO>> GetAllAsync();
    Task<TipoCabanaDTO?> GetByIdAsync(int id);
    Task<TipoCabanaDTO> CreateAsync(TipoCabanaCreateDTO dto);
    Task<(bool Success, string Message)> UpdateAsync(int id, TipoCabanaCreateDTO dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}