using AriaPM.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AriaPM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Pallets, Title="Pallets" },
                new HomeMenuItem {Id = MenuItemType.Warehouse, Title="Warehouse" },
                new HomeMenuItem {Id = MenuItemType.Distribution, Title="Distribution" },
                new HomeMenuItem {Id = MenuItemType.Logout, Title="Logout" }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                if (id == 3)
                {
                    await Navigation.PushModalAsync(new LoginPage());
                    return;
                }
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}