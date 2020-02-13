using AriaPM.Models;
using AriaPM.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AriaPM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        LoginViewModel viewModel;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new LoginViewModel();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private int? IsAuthenticated()
        {
            return viewModel.IsAuthenticated(UserId.Text.Trim(), Password.Text.Trim()).Result;
        }

        private async void Password_Completed(object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            if (!string.IsNullOrEmpty(UserId.Text) && !string.IsNullOrEmpty(Password.Text))
            {
                var result = IsAuthenticated();
                if (result != null)
                {
                    Application.Current.Properties["IsAdmin"] = result == 1;
                    Application.Current.Properties["UserId"] = UserId.Text;
                    Application.Current.MainPage = new MainPage();
                    await Navigation.PushModalAsync(new MainPage());
                    return;
                }
            }
            await DisplayAlert("Message", "Enter valid credentials", "Ok");
        }
    }
}