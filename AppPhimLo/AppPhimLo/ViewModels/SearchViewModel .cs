using AppPhimLo.Models;
using AppPhimLo.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Controls;
namespace AppPhimLo.ViewModels;

public class SearchViewModel : BindableObject
{
    private string searchQuery;
    private int currentPage = 1;
    private int totalPages = 1;
    private bool isLoading = false;
    private const int PageSize = 10;

    private readonly MovieService _movieService;
    private readonly GenreService _genreService;

    public string SearchQuery
    {
        get => searchQuery;
        set { searchQuery = value; OnPropertyChanged(); }
    }

    public int CurrentPage
    {
        get => currentPage;
        set { currentPage = value; OnPropertyChanged(); UpdatePaginationProperties(); }
    }

    public int TotalPages
    {
        get => totalPages;
        set { totalPages = value; OnPropertyChanged(); UpdatePaginationProperties(); }
    }

    public ObservableCollection<Movie> Movies { get; set; } = new();

    public bool HasMovies => Movies.Count > 0;
    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;

    public ICommand SearchCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public ICommand ShowGenresCommand { get; }
    public ICommand LoadGenresCommand { get; }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged();
                ((Command)ShowGenresCommand).ChangeCanExecute();
            }
        }
    }

    public ObservableCollection<Genre> Genres { get; set; } = new();

    private Genre selectedGenre;
    public Genre SelectedGenre
    {
        get => selectedGenre;
        set
        {
            if (selectedGenre != value)
            {
                selectedGenre = value;
                OnPropertyChanged();
                if (value != null)
                {
                    _ = PerformGenreSearchAsync(value.Slug, 1);
                }
            }
        }
    }

    // Thêm vào trong SearchViewModel của bạn

    // Phương thức để tìm kiếm phim dựa trên slug của thể loại
    private async Task PerformGenreSearchAsync(string genreSlug, int page)
    {
        if (isLoading)
            return;

        isLoading = true; // Bắt đầu trạng thái tải
        CurrentPage = page; // Cập nhật trang hiện tại

        Movies.Clear(); // Xóa danh sách phim cũ

        try
        {
            // Gọi API để lấy phim theo thể loại đã chọn
            var (results, totalItems, totalPages) = await _genreService.GetMoviesByGenreAsync(
                genreSlug,
                CurrentPage,
                limit: PageSize // Sử dụng PageSize đã định nghĩa
            );

            foreach (var movie in results)
                Movies.Add(movie); // Thêm phim vào ObservableCollection

            TotalPages = totalPages; // Cập nhật tổng số trang

            OnPropertyChanged(nameof(Movies));
            OnPropertyChanged(nameof(HasMovies));
            UpdatePaginationProperties(); 
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error searching movies by genre: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể tải phim theo thể loại này. Vui lòng thử lại sau.", "OK");
        }
        finally
        {
            isLoading = false; 
        }
    }

    public ICommand SelectGenreCommand { get; }


    public SearchViewModel()
    {
        _movieService = new MovieService();

        SearchCommand = new Command(async () => await SearchMoviesAsync(1));

        NextPageCommand = new Command(async () =>
        {
            if (CanGoNext)
                await SearchMoviesAsync(CurrentPage + 1);
        });

        PreviousPageCommand = new Command(async () =>
        {
            if (CanGoPrevious)
                await SearchMoviesAsync(CurrentPage - 1);
        });

        // Tai
        _genreService = new GenreService();
        ShowGenresCommand = new Command(async () => await ShowGenresActionSheetAsync(), () => !IsBusy);
        LoadGenresCommand = new Command(async () => await LoadGenresAsync(), () => !IsBusy);

        Genres = new ObservableCollection<Genre>();
        _ = LoadGenresAsync();
        SelectGenreCommand = new Command<Genre>(async (genre) => await OnGenreSelected(genre));


    }

    // Sửa đổi phương thức OnGenreSelected hiện có trong SearchViewModel

    private async Task OnGenreSelected(Genre selectedGenre)
    {
        if (selectedGenre == null) return;

        SelectedGenre = selectedGenre; // Lưu thể loại đã chọn

        await PerformGenreSearchAsync(selectedGenre.Slug, 1);
    }


    private async Task LoadGenresAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var genreList = await _genreService.GetGenresAsync();
            Genres.Clear(); 
            foreach (var genre in genreList)
            {
                    Genres.Add(genre);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading genres: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể tải danh sách thể loại.", "OK");
        }
        finally
        {
            IsBusy = false; 
        }
    }

    private async Task ShowGenresActionSheetAsync()
    {
        if (IsBusy) return;

        if (!Genres.Any())
        {
            await LoadGenresAsync();
            if (!Genres.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Thông báo", "Không có thể loại nào để chọn.", "OK");
                return;
            }
        }

        string title = "Chọn Thể loại";
        string cancelButton = "Hủy";

        string[] genreButtons = Genres.Select(g => g.Name).ToArray();

        string selectedGenreName = await Application.Current.MainPage.DisplayActionSheet(
            title,
            cancelButton,
            null, 
            genreButtons);

        if (selectedGenreName != null && selectedGenreName != cancelButton)
        {
            SelectedGenre = Genres.FirstOrDefault(g => g.Name == selectedGenreName);
        }
    }

    private async Task SearchMoviesAsync(int page)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery) || isLoading)
            return;

        isLoading = true;
        CurrentPage = page;

        Movies.Clear();

        var (results, totalItems, totalPages) = await _movieService.SearchMoviesAsync(SearchQuery, CurrentPage, PageSize);

        foreach (var movie in results)
            Movies.Add(movie);

        TotalPages = totalPages; 

        OnPropertyChanged(nameof(Movies));
        OnPropertyChanged(nameof(HasMovies));
        UpdatePaginationProperties();

        isLoading = false;
    }

    private void UpdatePaginationProperties()
    {
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }
}
