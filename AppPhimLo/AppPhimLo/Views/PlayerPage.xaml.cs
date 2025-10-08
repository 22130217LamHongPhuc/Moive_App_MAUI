using AppPhimLo.Services;
using AppPhimLo.ViewModels;

namespace AppPhimLo.Views;

[QueryProperty(nameof(EpisodeName), "episodeName")]
[QueryProperty(nameof(LinkEmbed), "linkEmbed")]
[QueryProperty(nameof(MovieSlug), "movieSlug")]


public partial class PlayerPage : ContentPage
{
    private readonly PlayerViewModel vm;

    private string? episodeName;
    public string? EpisodeName
    {
        get => episodeName;
        set
        {
            episodeName = value;
            TryLoad();
        }
    }

    private string? movieSlug;
    public string? MovieSlug
    {
        get => movieSlug;
        set
        {
            movieSlug = value;
            TryLoad();

        }
    }

    private string? linkEmbed;
    public string? LinkEmbed
    {
        get => linkEmbed;
        set
        {
            linkEmbed = value;
        }
    }

    public PlayerPage()
    {
        InitializeComponent();
        vm = new PlayerViewModel(new MovieService());
        BindingContext = vm;
    }

    private async void TryLoad()
    {
        if (vm == null || string.IsNullOrWhiteSpace(LinkEmbed)) return;
        var a = movieSlug;
        await vm.LoadFromEmbedAsync(EpisodeName, LinkEmbed,MovieSlug);
    }
}
