using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.API.Interfaces
{
    public interface IServicioService
    {
        Task<IEnumerable<ServicioReadDto>> GetPagedAsync(int pageNumber, int pageSize, string? nombre, int? id);
        Task<ServicioCreateDto> CreateServicioAsync(ServicioCreateDto servicioDto);
        Task<bool> PatchAsync(int id, UpdateServicioDto patchData);
        Task<bool> DeleteServicioAsync(int id);
    }
}