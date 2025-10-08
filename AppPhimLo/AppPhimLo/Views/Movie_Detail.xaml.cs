using AppPhimLo.Services;
using AppPhimLo.ViewModels;

namespace AppPhimLo.Views;

[QueryProperty(nameof(Slug), "slug")]
public partial class Movie_Detail : ContentPage
{
    private readonly MovieViewModel vm;
    private string? slug;

    public string? Slug
    {
        get => slug;
        set
        {
            slug = value;
           if (!string.IsNullOrWhiteSpace(value))
            {
                _ = vm.EnsureLoadedAsync(value); // gọi API đúng lúc Shell đã set
            }
        }
    }

    public Movie_Detail()
    {
        InitializeComponent();
        vm = new MovieViewModel(new MovieService());
        BindingContext = vm;

       
    }
}
