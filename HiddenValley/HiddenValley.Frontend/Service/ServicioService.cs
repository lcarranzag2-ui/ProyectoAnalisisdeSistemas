using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;

namespace HiddenValley.Frontend.Services
{
    public class ServicioService : IServicioService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        public ServicioService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<ServicioReadDto>> GetPagedAsync(int pageNumber, int pageSize, string? nombre, int? id)
        {
            try
            {
                var url = $"api/servicios?pageNumber={pageNumber}&pageSize={pageSize}";
                if (id.HasValue) url += $"&id={id.Value}";
                else if (!string.IsNullOrWhiteSpace(nombre)) url += $"&nombre={Uri.EscapeDataString(nombre)}";

                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<ServicioReadDto>();

                var items = await response.Content.ReadFromJsonAsync<IEnumerable<ServicioReadDto>>(_options);
                return items ?? new List<ServicioReadDto>();
            }
            catch (Exception)
            {
                return new List<ServicioReadDto>();
            }
        }

        public async Task<ServicioCreateDto> CreateServicioAsync(ServicioCreateDto servicioDto)
        {
            var response = await _http.PostAsJsonAsync("api/servicios", servicioDto);
            if (response.IsSuccessStatusCode)
            {
                var creado = await response.Content.ReadFromJsonAsync<ServicioCreateDto>(_options);
                return creado ?? servicioDto;
            }
            return null!;
        }

        public async Task<bool> PatchAsync(int id, UpdateServicioDto patchData)
        {
            var json = JsonSerializer.Serialize(patchData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/servicios/{id}") { Content = content };
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServicioAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/servicios/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}