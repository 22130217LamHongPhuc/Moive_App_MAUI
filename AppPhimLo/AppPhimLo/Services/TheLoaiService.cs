using AppPhimLo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppPhimLo.Services
{
    class TheLoaiService
    {
        private readonly HttpClient _httpClient;

        public TheLoaiService()
        {
            _httpClient = new HttpClient();
        }
        public async Task<ObservableCollection<TheLoaiItem>> GetTheLoai()
        {
            var url = $"https://phimapi.com/the-loai";
            try
            {

                var response = await _httpClient.GetStringAsync(url);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                
                var list = JsonSerializer.Deserialize<List<TheLoaiItem>>(response, options);

                return new ObservableCollection<TheLoaiItem>(list ?? new List<TheLoaiItem>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new ObservableCollection<TheLoaiItem>();
            }
        }
    }
}
