using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Shared.DTOs;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace HiddenValley.Frontend.Services
{
    public class TipoCabanaService : ITipoCabanaService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/tipocabana"; 

        public TipoCabanaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<TipoCabanaDTO>> GetAllAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<IEnumerable<TipoCabanaDTO>>(BaseUrl);
                return response ?? new List<TipoCabanaDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllAsync: {ex.Message}");
                return new List<TipoCabanaDTO>();
            }
        }

        public async Task<TipoCabanaDTO?> GetByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<TipoCabanaDTO>($"{BaseUrl}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<TipoCabanaDTO> CreateAsync(TipoCabanaCreateDTO dto)
        {
            var response = await _http.PostAsJsonAsync(BaseUrl, dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TipoCabanaDTO>() ?? new TipoCabanaDTO();
            }
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new Exception(!string.IsNullOrEmpty(errorMsg) ? errorMsg : "Error al crear el registro en el servidor.");
        }
        public async Task<(bool Success, string Message)> UpdateAsync(int id, TipoCabanaCreateDTO dto)
    {
        try
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{BaseUrl}/{id}");

            var jsonContent = JsonSerializer.Serialize(dto);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return (true, "Actualizado correctamente.");
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrEmpty(errorContent) ? "Error al actualizar." : errorContent);
        }
        catch (Exception ex)
        {
            return (false, $"Error de comunicación: {ex.Message}");
        }
    }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "Eliminado correctamente.");
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrEmpty(errorContent) ? "No se pudo eliminar el registro." : errorContent);
        }
    }
}