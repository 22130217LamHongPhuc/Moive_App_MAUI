#nullable enable
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using AppPhimLo.Models;
using AppPhimLo.Services;
using Microsoft.Maui;
using System;
using AppPhimLo.Views;



namespace AppPhimLo.ViewModels
{
    [QueryProperty(nameof(Slug), "slug")]

    public class MovieViewModel : BindableObject
    {
        // ===== Fields (giống SearchViewModel) =====
        private string? slug;
        private bool isLoading = false; // đồng bộ cách đặt tên
        private string errorMessage = string.Empty;
        public ICommand OpenTrailerCommand { get; }



        // Data
        private MovieDetail? movie;
        public ObservableCollection<EpisodeServer> Episodes { get; } = new();

        // Service
        private readonly MovieService _movieService;
        // ... các property khác: Movie, Episodes, IsLoading, ErrorMessage ...
        public ICommand OpenEpisodeCommand { get; }
        public string? CurrentMovieSlug { get; private set; }



        public MovieViewModel() : this(new MovieService()) { }

        public MovieViewModel(MovieService movieService)
        {
            _movieService = movieService;

            LoadCommand = new Command<string?>(async s =>
            {
                if (!string.IsNullOrWhiteSpace(s))
                    Slug = s;

                await LoadMovieAsync();
            },
            s => !IsLoading && !string.IsNullOrWhiteSpace(s ?? Slug));

            OpenTrailerCommand = new Command<string?>(async url =>
            {
                if (string.IsNullOrWhiteSpace(url)) return;
                try { await Launcher.Default.OpenAsync(url); } catch { }
            });

            OpenEpisodeCommand = new Command<EpisodeData>(async ep =>
            {
                try
                {
                    if (ep == null) return;

                    var routeParams = new Dictionary<string, object?>
                    {
                        ["movieSlug"] = Slug,
                        ["episodeSlug"] = ep.Slug,
                        ["episodeName"] = ep.Name,
                        ["linkEmbed"] = ep.LinkEmbed,
                        ["linkM3u8"] = ep.LinkM3u8,
                        ["fileName"] = ep.FileName
                    };

                    await Shell.Current.GoToAsync(nameof(PlayerPage), routeParams);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi điều hướng", ex.Message, "OK");
                }
            });


        }

        // ===== Bindable Properties =====
        public string? Slug
        {
            get => slug;
            set
            {
                if (slug != value)
                {
                    slug = value;
                    OnPropertyChanged();
                    // Cho phép nút Load cập nhật CanExecute khi Slug thay đổi
                    (LoadCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            private set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged();
                    (LoadCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            private set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public MovieDetail? Movie
        {
            get => movie;
            private set
            {
                if (!Equals(movie, value))
                {
                    movie = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasData));
                }
            }
        }

        public bool HasData => Movie is not null;

        // ===== Commands =====
        public ICommand LoadCommand { get; }
     


public async Task LoadMovieAsync()
        {
         

            IsLoading = true;
            ErrorMessage = string.Empty;

            // Xoá dữ liệu cũ trước khi nạp, giống Movies.Clear() ở SearchViewModel
            Movie = null;
            Episodes.Clear();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            try
            {
                var result = await _movieService.GetMovieAsync(Slug!, cts.Token);
                if (result is null)
                {
                    ErrorMessage = "Không tìm thấy dữ liệu phim.";
                    return;
                }

                // Không dùng ConfigureAwait(false) => tiếp tục trên UI thread, set bindables an toàn
                Movie = result.Movie;
                foreach (var ep in result.Episodes ?? new List<EpisodeServer>())
                    Episodes.Add(ep);

                // Thông báo các thuộc tính phụ thuộc (giống OnPropertyChanged(nameof(HasMovies)) của bạn)
                OnPropertyChanged(nameof(HasData));
            }
            catch (OperationCanceledException)
            {
                ErrorMessage = "Hết thời gian chờ. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải phim: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Gọi ở OnAppearing để tự tải khi chưa có dữ liệu (giống EnsureLoadedAsync của bạn).
        /// </summary>
        public async Task EnsureLoadedAsync(string slug)
        {
                Slug = slug;
            

            if (!HasData && !IsLoading)
                await LoadMovieAsync();
        }

        // BindableObject đã có OnPropertyChanged([CallerMemberName])
        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => base.OnPropertyChanged(propertyName);

     
    }


}
