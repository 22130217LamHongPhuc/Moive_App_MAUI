using System.Collections.ObjectModel;
using System.Windows.Input;
using AppPhimLo.Models;
using AppPhimLo.Services;
using AppPhimLo.Views;

namespace AppPhimLo.ViewModels;

public class SearchViewModel : BindableObject
{
    private string searchQuery;
    private int currentPage = 1;
    private int totalPages = 1;
    private bool isLoading = false;
    private const int PageSize = 10;

    private readonly MovieService _movieService;
    public ICommand OpenDetailCommand { get; }


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


        //OpenDetailCommand = new Command<string?>(async slug =>
        //{
        //    if (string.IsNullOrWhiteSpace(slug)) return;
        //    await Shell.Current.Navigation.PushAsync(new Movie_Detail(slug));
        //});

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


    private async Task OpenMovieAsync(string slug)
    {
        if (slug is null) return;

        // Cách 1: Query string
        // await Shell.Current.GoToAsync($"{nameof(MovieDetailPage)}?slug={item.Slug}");

        // Cách 2: Dictionary (an toàn hơn)
        await Shell.Current.GoToAsync(nameof(Movie_Detail), new Dictionary<string, object>
        {
            ["slug"] = slug
        });
    }



}
