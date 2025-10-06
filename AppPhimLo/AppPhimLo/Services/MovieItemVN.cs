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
    class MovieItemVN
    {
        private readonly HttpClient _httpClient;

        public MovieItemVN()
        {
            _httpClient = new HttpClient();
        }
        public async Task<ObservableCollection<MovieItem>> GetMoviesByYearAndQuocGiaAsync()
        {
            var url = $"https://phimapi.com/v1/api/quoc-gia/viet-nam?page=1&year=2025";
            try
            {

                var response = await _httpClient.GetStringAsync(url);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    //ReadCommentHandling = JsonCommentHandling.Skip,
                    //AllowTrailingCommas = true
                };

                var apiResponse = JsonSerializer.Deserialize<ApiResponse2>(response, options);

                return apiResponse?.data?.items ?? new ObservableCollection<MovieItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new ObservableCollection<MovieItem>();
            }
        }
    }
}
