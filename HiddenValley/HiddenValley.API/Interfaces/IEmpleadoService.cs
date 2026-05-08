using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface IEmpleadoService
    {
    Task<PagedResponse<EmpleadoResponseDTO>> GetPagedAsync(string? search, int page, int pageSize);
    Task<(bool Success, string Message, int? Id)> CreateAsync(EmpleadoCreateDTO dto);
    Task<(bool Success, string Message)> PatchAsync(int idEmpleado, EmpleadoPatchDTO dto);
    Task<(bool Success, string Message)> DeleteAsync(int idEmpleado);
    }
}