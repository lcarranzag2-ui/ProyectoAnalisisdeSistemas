using System.Net.Http.Json;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Services
{
    public class TipoServicioClient : ITipoServicioClient
    {
        private readonly HttpClient _http;

        public TipoServicioClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TipoServicioReadDto>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<TipoServicioReadDto>>("api/tiposervicio");
            return result ?? new List<TipoServicioReadDto>();
        }

        public async Task<bool> CrearAsync(TipoServicioCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/tiposervicio", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarAsync(int id, TipoServicioCreateDto dto)
        {
            var response = await _http.PatchAsJsonAsync($"api/tiposervicio/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/tiposervicio/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
