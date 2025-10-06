using AppPhimLo.Models;
using System.Text.Json;

namespace AppPhimLo.Services;

public class MovieService
{
    private readonly HttpClient _httpClient;

    public MovieService()
    {
        _httpClient = new HttpClient();
    }

    // Trả về: danh sách phim, tổng số phim, tổng số trang
    public async Task<(List<Movie> Movies, int TotalItems, int TotalPages)> SearchMoviesAsync(string keyword, int page = 1, int limit = 10)
    {
        var url = $"https://phimapi.com/v1/api/tim-kiem?keyword={keyword}&page={page}&limit={limit}";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response, options);

            // Trả về danh sách phim + totalItems + totalPages từ API
            return (
                apiResponse?.Data?.Items ?? new List<Movie>(),
                apiResponse?.Data?.Params?.Pagination?.TotalItems ?? 0,
                apiResponse?.Data?.Params?.Pagination?.TotalPages ?? 1
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            return (new List<Movie>(), 0, 1);
        }
    }
    

    private class ApiResponse
    {
        public ApiData Data { get; set; }
    }

    private class ApiData
    {
        public List<Movie> Items { get; set; }
        public ApiParams Params { get; set; }
    }

    private class ApiParams
    {
        public Pagination Pagination { get; set; }
    }

    private class Pagination
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
