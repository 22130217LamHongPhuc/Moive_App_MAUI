using AppPhimLo.Views;

namespace AppPhimLo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Movie_Detail), typeof(AppPhimLo.Views.Movie_Detail));
            Routing.RegisterRoute(nameof(PlayerPage), typeof(AppPhimLo.Views.PlayerPage));


        }
    }
}
