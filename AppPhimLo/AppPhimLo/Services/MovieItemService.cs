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
    class MovieItemService
    {
        private readonly HttpClient _httpClient;

        public MovieItemService()
        {
            _httpClient= new HttpClient();
        }
        public async Task<(ObservableCollection<MovieItem> Movies, Pagination Pagination)> GetNewMoviesAsync(int page = 1)
        {
            var url = $"https://phimapi.com/danh-sach/phim-moi-cap-nhat?page={page}";
            try
            {
                var response = await _httpClient.GetStringAsync(url);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response, options);

                return (
                    apiResponse?.items ?? new ObservableCollection<MovieItem>(),
                    apiResponse?.pagination ?? new Pagination()
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return (new ObservableCollection<MovieItem>(), new Pagination());
            }
        }

    }
}
