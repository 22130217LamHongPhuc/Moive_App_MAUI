using AppPhimLo.Models;
using System.Text.Json;

public class GenreService
{
    private readonly HttpClient _httpClient;

    public GenreService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Genre>> GetGenresAsync()
    {
        var url = "https://phimapi.com/the-loai";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var genreList = JsonSerializer.Deserialize<List<Genre>>(response, options);

            return genreList ?? new List<Genre>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Genre API Error: {ex.Message}");
            return new List<Genre>();
        }
    }

    public async Task<(List<Movie> Movies, int TotalItems, int TotalPages)> GetMoviesByGenreAsync(
    string typeList,
    int page = 1,
    string sortField = "modified.time",
    string sortType = "desc",
    string sortLang = "vietsub",
    string country = "",
    int year = 0,
    int limit = 10)
    {
        var queryParams = new List<string>
    {
        $"page={page}",
        $"sort_field={sortField}",
        $"sort_type={sortType}",
        $"sort_lang={sortLang}",
        $"limit={limit}"
    };

        if (!string.IsNullOrWhiteSpace(country))
            queryParams.Add($"country={country}");

        if (year > 0)
            queryParams.Add($"year={year}");

        var queryString = string.Join("&", queryParams);
        var url = $"https://phimapi.com/v1/api/the-loai/{typeList}?{queryString}";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response, options);

            return (
                apiResponse?.Data?.Items ?? new List<Movie>(),
                apiResponse?.Data?.Params?.Pagination?.TotalItems ?? 0,
                apiResponse?.Data?.Params?.Pagination?.TotalPages ?? 1
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetMoviesByGenre API Error: {ex.Message}");
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