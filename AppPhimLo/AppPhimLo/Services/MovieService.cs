using AppPhimLo.Models;
using AppPhimLo.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace AppPhimLo.Services;

public interface IMovieApiService
{
    Task<MovieResponse?> GetMovieAsync(string slug, CancellationToken ct = default);
}

public class MovieService
{
    private readonly HttpClient _httpClient;
    JsonSerializerOptions _json = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<Movie>> GetMoviesAsync(CancellationToken ct = default)
    {
        using var resp = await _httpClient.GetAsync("movies/action", ct);
        resp.EnsureSuccessStatusCode();

        var stream = await resp.Content.ReadAsStreamAsync(ct);
        var movies = await JsonSerializer.DeserializeAsync<List<Movie>>(stream, JsonOpts, ct)
                     ?? new List<Movie>();

        return movies;
    }

    public async Task<MovieResponse?> GetMovieAsync(string slug = "ngoi-truong-xac-song", CancellationToken ct = default)
    {
        var url = $"https://phimapi.com/phim/{slug}";
        var resp = await _httpClient.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();

        var stream = await resp.Content.ReadAsStreamAsync(ct);
        var result = await JsonSerializer.DeserializeAsync<MovieResponse>(stream, JsonOpts, ct);
        return result;
    }

    public async Task<List<CommentItem>> GetCommentsAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return new();

        var url = $"http://152.42.244.190:8080/api/comments?slug={Uri.EscapeDataString(slug)}";

        JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            var list = await _httpClient.GetFromJsonAsync<List<CommentResponse>>(url, _json);
            return list?.Select(MapToItem).ToList() ?? new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetCommentsAsync] {ex.Message}");
            return new();
        }
    }

    public async Task<bool> PostCommentAsync(CreateCommentRequest req, CancellationToken ct = default)
    {
        var url = "http://152.42.244.190:8080/api/comments";

        var resp = await _httpClient.PostAsJsonAsync(url, req, _json, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync(ct);
            System.Diagnostics.Debug.WriteLine($"[PostComment] {resp.StatusCode} - {err}");
            return false;
        }

        return true;
    }

    public async Task<List<FavoriteRecord>> GetFavoritesBySlugAsync(string slug)
    {
        var url = $"http://152.42.244.190:8080/api/favorites?slug={Uri.EscapeDataString(slug)}";
        try
        {
            var list = await _httpClient.GetFromJsonAsync<List<FavoriteRecord>>(url, _json);
            return list ?? new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetFavoritesBySlugAsync] {ex.Message}");
            return new();
        }
    }

    public async Task<bool> AddFavoriteAsync(FavoriteRecord record)
    {
        var resp = await _httpClient.PostAsJsonAsync("http://152.42.244.190:8080/api/favorites", record, _json);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveFavoriteAsync(int userId, string slug)
    {
        var url = $"http://152.42.244.190:8080/api/favorites?userId={userId}&slug={Uri.EscapeDataString(slug)}";
        var resp = await _httpClient.DeleteAsync(url);
        return resp.IsSuccessStatusCode;
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

    private class CommentResponse
    {
        public int? UserId { get; set; }
        public string? Slug { get; set; }
        public string? DisplayName { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }

    private CommentItem MapToItem(CommentResponse dto)
    {
        return new CommentItem
        {
            UserId = dto.UserId,
            Slug = dto.Slug,
            DisplayName = dto.DisplayName,
            Content = dto.Content,
            TimeAgo = GetTimeAgo(dto.CreatedAt)
        };
    }

    private string GetTimeAgo(DateTimeOffset? createdAt)
    {
        if (createdAt == null) return "vừa xong";
        var diff = DateTimeOffset.Now - createdAt.Value;
        if (diff.TotalMinutes < 1) return $"{(int)diff.TotalSeconds} giây trước";
        if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} phút trước";
        if (diff.TotalDays < 1) return $"{(int)diff.TotalHours} giờ trước";
        return createdAt.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }
}

public class CreateCommentRequest
{
    public int? userId { get; set; }
    public string? slug { get; set; }
    public string? displayName { get; set; }
    public string? content { get; set; }
    public string? timeAgo { get; set; } = "vừa xong";
}
