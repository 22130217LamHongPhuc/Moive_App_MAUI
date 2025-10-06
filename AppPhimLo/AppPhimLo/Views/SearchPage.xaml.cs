namespace AppPhimLo.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void Move(object sender, TappedEventArgs e)
    {
        var frame = (Frame)sender;
        string Slug = frame.ClassId;
        await Shell.Current.GoToAsync($"Movie_Detail?slug={Uri.EscapeDataString(Slug)}");
    }
}
