
using AppPhimLo.Models;
using AppPhimLo.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
namespace AppPhimLo
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private readonly MovieItemService _movieService = new MovieItemService();
        private readonly MovieItemTQ _movieItemsTQ = new MovieItemTQ();
        private readonly MovieItemHanQuoc _movieTeamHQ = new MovieItemHanQuoc();
        private readonly MovieItemVN _movieVN = new MovieItemVN();
        private readonly TheLoaiService _theLoai = new TheLoaiService();
        private int _currentPage = 1;
        private int _totalPages = 1;
        public ObservableCollection<MovieItem> Movies { get; set; } = new ObservableCollection<MovieItem>();
        public ObservableCollection<MovieItem> MoviesTQ { get; set; } = new ObservableCollection<MovieItem>();

        public ObservableCollection<MovieItem> MoviesHQ { get; set; } = new ObservableCollection<MovieItem>();

        public ObservableCollection<MovieItem> MoviesVN { get; set; } = new ObservableCollection<MovieItem>();

        public ObservableCollection<MovieItem> MoviesVN2 { get; set; } = new ObservableCollection<MovieItem>();

        public ObservableCollection<TheLoaiItem> TheLoais { get; set; } = new ObservableCollection<TheLoaiItem>();
        int _carouselIndex = 0;
        public object PrevButton { get; private set; }
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(); 
                    OnPropertyChanged(nameof(IsNotLoading)); 
                }
            }
        }
        public bool IsNotLoading => !IsLoading;
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
            LoadMovies(_currentPage);

            
            
        }
        protected override async void OnAppearing()
        {
            
            base.OnAppearing();

            IsLoading = true;
            var (movies, pagination) = await _movieService.GetNewMoviesAsync(_currentPage);
            Movies.Clear();
            foreach (var movie in movies)
                Movies.Add(movie);
            //MoviesCollection.ItemsSource = Movies;


            
            if (MoviesTQ.Count == 0)   
            {
                var moviesTQ = await _movieItemsTQ.GetMoviesByYearAsync();
                foreach (var movie in moviesTQ)
                    MoviesTQ.Add(movie);
                
            }
            await Task.Delay(300);

            _carouselIndex = 0;
            carousel.ScrollTo(_carouselIndex, animate: false);

            IsLoading = false;
            //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            //{
            //    if (MoviesTQ.Count == 0)
            //        return true; 


            //    _carouselIndex++;
            //    if (_carouselIndex >= MoviesTQ.Count)
            //        _carouselIndex = 0;


            //    carousel.ScrollTo(_carouselIndex, animate: false);

            //    return true; 
            //});


            if (MoviesHQ.Count == 0)
            {
                var moviesHQ = await _movieTeamHQ.GetMoviesByYearAndQuocGiaAsync();
                foreach (var movie in moviesHQ)
                    MoviesHQ.Add(movie);

            }
            if (MoviesVN.Count == 0)
            {
                var moviesVN = await _movieVN.GetMoviesByYearAndQuocGiaAsync();
                foreach (var movie in moviesVN)
                {
                    MoviesVN.Add(movie);
                    MoviesVN2.Add(movie);
                }


            }

            if (TheLoais.Count == 0)
            {
                var theLoai = await _theLoai.GetTheLoai();

                var colors = new List<Color>
                    {
                     Color.FromArgb("#FF9AA2"),
                     Color.FromArgb("#FFB7B2"),
                     Color.FromArgb("#FFDAC1"),
                     Color.FromArgb("#E2F0CB"),
                     Color.FromArgb("#B5EAD7"),
                     Color.FromArgb("#C7CEEA"),
                    };
                int i = 0;

                foreach (var theloai in theLoai)
                {
                    theloai.Color = colors[i % colors.Count]; 
                    i++;
                    TheLoais.Add(theloai); 
                }
            }

            IsLoading = false;
        }
        private async void LoadMovies(int page)
        {
            var (movies, pagination) = await _movieService.GetNewMoviesAsync(page);

            Movies.Clear();
            foreach (var movie in movies)
                Movies.Add(movie);

            
            _currentPage = pagination.currentPage;
            _totalPages = pagination.totalPages;

            CurrentPageLabel.Text = _currentPage.ToString();
            TotalPagesLabel.Text = _totalPages.ToString();

            
            PrevButton1.IsEnabled = _currentPage > 1;
            NextButton.IsEnabled = _currentPage < _totalPages;
        }
        private void PrevButton_Clicked(object sender, EventArgs e)
        {
            if (_currentPage > 1)
                LoadMovies(_currentPage - 1);
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
                LoadMovies(_currentPage + 1);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Nút trái
        private void OnLeftTapped(object sender, EventArgs e)
        {
            if (MoviesTQ.Count == 0) return;

            _carouselIndex--;
            if (_carouselIndex < 0)
                _carouselIndex = MoviesTQ.Count - 1;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var movie = MoviesTQ[_carouselIndex];
                carousel.ScrollTo(movie, position: ScrollToPosition.Center, animate: false);
            });
        }

        private void OnRightTapped(object sender, EventArgs e)
        {
            if (MoviesTQ.Count == 0) return;

            _carouselIndex++;
            if (_carouselIndex >= MoviesTQ.Count)
                _carouselIndex = 0;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var movie = MoviesTQ[_carouselIndex];
                carousel.ScrollTo(movie, position: ScrollToPosition.Center, animate: false);
            });
        }




    }

}
