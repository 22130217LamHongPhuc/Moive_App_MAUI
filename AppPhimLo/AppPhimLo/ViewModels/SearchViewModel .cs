using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

using AppPhimLo.Models;
using AppPhimLo.Services;
using AppPhimLo.Views;

namespace AppPhimLo.ViewModels
{
    public class SearchViewModel : BindableObject
    {
        private const int PageSize = 10;

        private string _searchQuery;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _isLoading = false;   // chặn double trigger
        private bool _isBusy = false;      // trạng thái UI

        private readonly MovieService _movieService;
        private readonly GenreService _genreService;

        public SearchViewModel()
        {
            _movieService = new MovieService();
            _genreService = new GenreService();

            SearchCommand = new Command(async () => await SearchMoviesAsync(1), () => !IsBusy);
            NextPageCommand = new Command(async () =>
            {
                if (CanGoNext) await SearchMoviesAsync(CurrentPage + 1);
            }, () => !IsBusy && CanGoNext);

            PreviousPageCommand = new Command(async () =>
            {
                if (CanGoPrevious) await SearchMoviesAsync(CurrentPage - 1);
            }, () => !IsBusy && CanGoPrevious);

            ShowGenresCommand = new Command(async () => await ShowGenresActionSheetAsync(), () => !IsBusy);
            LoadGenresCommand = new Command(async () => await LoadGenresAsync(), () => !IsBusy);
            SelectGenreCommand = new Command<Genre>(async g => await OnGenreSelected(g));
            OpenDetailCommand = new Command<string?>(async slug => await OpenMovieAsync(slug));

            // tải genres lần đầu
            _ = LoadGenresAsync();
        }

        #region Properties

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); UpdatePaginationProperties(); }
        }

        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value; OnPropertyChanged(); UpdatePaginationProperties(); }
        }

        public ObservableCollection<Movie> Movies { get; } = new();
        public bool HasMovies => Movies.Count > 0;
        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public ObservableCollection<Genre> Genres { get; } = new();

        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        _ = PerformGenreSearchAsync(value.Slug, 1);
                    }
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    (SearchCommand as Command)?.ChangeCanExecute();
                    (NextPageCommand as Command)?.ChangeCanExecute();
                    (PreviousPageCommand as Command)?.ChangeCanExecute();
                    (ShowGenresCommand as Command)?.ChangeCanExecute();
                    (LoadGenresCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ShowGenresCommand { get; }
        public ICommand LoadGenresCommand { get; }
        public ICommand SelectGenreCommand { get; }
        public ICommand OpenDetailCommand { get; }

        #endregion

        #region Methods

        private async Task SearchMoviesAsync(int page)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery) || _isLoading) return;

            _isLoading = true;
            IsBusy = true;
            try
            {
                CurrentPage = page;
                Movies.Clear();

                var (results, _, totalPages) =
                    await _movieService.SearchMoviesAsync(SearchQuery, CurrentPage, PageSize);

                foreach (var movie in results)
                    Movies.Add(movie);

                TotalPages = totalPages;
                OnPropertyChanged(nameof(HasMovies));
                UpdatePaginationProperties();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SearchMoviesAsync] {ex}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể tải danh sách phim.", "OK");
            }
            finally
            {
                _isLoading = false;
                IsBusy = false;
            }
        }

        private async Task PerformGenreSearchAsync(string genreSlug, int page)
        {
            if (_isLoading) return;

            _isLoading = true;
            IsBusy = true;
            try
            {
                CurrentPage = page;
                Movies.Clear();

                var (results, _, totalPages) =
                    await _genreService.GetMoviesByGenreAsync(genreSlug, CurrentPage, limit: PageSize);

                foreach (var movie in results)
                    Movies.Add(movie);

                TotalPages = totalPages;
                OnPropertyChanged(nameof(HasMovies));
                UpdatePaginationProperties();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PerformGenreSearchAsync] {ex}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể tải phim theo thể loại.", "OK");
            }
            finally
            {
                _isLoading = false;
                IsBusy = false;
            }
        }

        private async Task LoadGenresAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var genreList = await _genreService.GetGenresAsync();
                Genres.Clear();

                if (genreList != null)
                    foreach (var g in genreList)
                        Genres.Add(g);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadGenresAsync] {ex}");
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

            var names = Genres.Select(g => g.Name).ToArray();

            string selectedName = await Application.Current.MainPage.DisplayActionSheet(
                "Chọn Thể loại",
                "Hủy",
                null,
                names
            );

            if (!string.IsNullOrEmpty(selectedName) && selectedName != "Hủy")
            {
                var g = Genres.FirstOrDefault(x => x.Name == selectedName);
                if (g != null) SelectedGenre = g;
            }
        }

        private async Task OnGenreSelected(Genre genre)
        {
            if (genre == null) return;
            SelectedGenre = genre;
            await PerformGenreSearchAsync(genre.Slug, 1);
        }

        private void UpdatePaginationProperties()
        {
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }

        private async Task OpenMovieAsync(string? slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return;

            await Shell.Current.GoToAsync(nameof(Movie_Detail), new Dictionary<string, object>
            {
                ["slug"] = slug
            });
        }

        #endregion
    }
}
