using AppPhimLo.Views;

namespace AppPhimLo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(AppPhimLo.MainPage));
            Routing.RegisterRoute(nameof(Movie_Detail), typeof(AppPhimLo.Views.Movie_Detail));
            Routing.RegisterRoute(nameof(PlayerPage), typeof(AppPhimLo.Views.PlayerPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(AppPhimLo.Views.SearchPage));



        }
    }
}
