using Microsoft.Maui.ApplicationModel.Communication;
using System.Net.Http.Json;
using System.Xml;

namespace AppPhimLo.Views;

public partial class ProfilePage : ContentPage
{
    private readonly HttpClient _httpClient;

    // Truyền email từ trang LoginPage khi đăng nhập thành công
    public ProfilePage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        string email = Preferences.Get("UserEmail", null);
        LoadProfile(email);
    }

    private async void LoadProfile(string email)
    {
        try
        {

            string url = $"http://152.42.244.190:8080/api/user/profile?email={email}";
            var profile = await _httpClient.GetFromJsonAsync<ProfileResponse>(url);

            if (profile != null)
            {
                NameLabel.Text = profile.Name;
                EmailLabel.Text = profile.Email;
                PhoneLabel.Text = profile.PhoneNumber;
                AddressLabel.Text = profile.Address;

                // Lưu sau khi load profile:
                Preferences.Set("UserId", profile.UserId );
                Preferences.Set("UserName", profile.Name);
                Preferences.Set("UserEmail", profile.Email);
                Preferences.Set("UserPhone", profile.PhoneNumber);
                Preferences.Set("UserAddress", profile.Address);

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", "Không thể load thông tin: " + ex.Message, "OK");
        }
    }

    private async void OnProfileButtonClicked(object sender, EventArgs e)
    {
        //Application.Current.MainPage = new AppShell();
        await Shell.Current.GoToAsync($"{nameof(MainPage)}");
    }
}

// Class để deserialize JSON từ backend
public class ProfileResponse
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
}
