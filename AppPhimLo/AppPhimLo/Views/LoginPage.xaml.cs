using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AppPhimLo.Views
{
    public partial class LoginPage : ContentPage
    {
        // HttpClient dùng để gọi API
        private readonly HttpClient _httpClient;

        public LoginPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập email và mật khẩu.", "OK");
                return;
            }

            try
            {
                // URL backend Spring Boot
                string url = "http://152.42.244.190:8080/api/auth/login";

                // Tạo nội dung x-www-form-urlencoded
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", email),
                    new KeyValuePair<string, string>("password", password)
                });

                // Gửi POST request
                HttpResponseMessage response = await _httpClient.PostAsync(url, formData);

                // Đọc kết quả trả về
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Thông báo", result, "OK");

                    //await Navigation.PushAsync(new ProfilePage(email));
                    // Lưu thông tin đăng nhập nếu cần
                    Preferences.Set("email", email);

                    // Chuyển sang AppShell (màn hình chính có TabBar)
                    Application.Current.MainPage = new AppShell();

                    // Và chuyển thẳng tới tab "Profile"
                    await Shell.Current.GoToAsync("//ProfilePage");
                }
                else
                {
                    await DisplayAlert("Lỗi", "Đăng nhập thất bại: " + result, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", "Không thể kết nối tới server: " + ex.Message, "OK");
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Thông báo", "Chức năng quên mật khẩu chưa được triển khai.", "OK");
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Thông báo", "Chức năng đăng ký chưa được triển khai.", "OK");
        }
    }
}
