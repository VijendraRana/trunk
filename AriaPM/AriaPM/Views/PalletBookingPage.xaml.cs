using AriaPM.Models;
using AriaPM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AriaPM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletBookingPage : ContentPage
    {
        PalletDispatchBookingViewModel viewModel;
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public List<int> ItemIds { get; set; }
        public int ChepCount { get; set; }
        public int LoscamCount { get; set; }
        public int PlainCount { get; set; }
        public int Weight { get; set; }

        public PalletBookingPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new PalletDispatchBookingViewModel();
            ItemIds = new List<int>();
            viewModel.Title = "Pallet Booking";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute("Wrapped");
            }
        }

        private void PalletListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Pallet;

            if (item == null)
                return;

            if (item.Id > 0)
            {
                if (ItemIds.Where(i => i == item.Id).Count() <= 0)
                {
                    ItemIds.Add(item.Id);

                    if (string.Equals(item.PalletType, "Chep", StringComparison.OrdinalIgnoreCase))
                    {
                        ChepCount++;
                    }
                    else if (string.Equals(item.PalletType, "Loscam", StringComparison.OrdinalIgnoreCase))
                    {
                        LoscamCount++;
                    }
                    else if (string.Equals(item.PalletType, "Plain", StringComparison.OrdinalIgnoreCase))
                    {
                        PlainCount++;
                    }

                    Weight = Weight + (string.IsNullOrWhiteSpace(item.Weight) ? 0 : Convert.ToInt32(item.Weight));
                }
                else
                {
                    ItemIds.Remove(item.Id);
                    if (string.Equals(item.PalletType, "Chep", StringComparison.OrdinalIgnoreCase))
                    {
                        ChepCount--;
                    }
                    else if (string.Equals(item.PalletType, "Loscam", StringComparison.OrdinalIgnoreCase))
                    {
                        LoscamCount--;
                    }
                    else if (string.Equals(item.PalletType, "Plain", StringComparison.OrdinalIgnoreCase))
                    {
                        PlainCount--;
                    }
                    Weight = Weight - (string.IsNullOrWhiteSpace(item.Weight) ? 0 : Convert.ToInt32(item.Weight));
                }

                SelectedPalletIds.Text = string.Join(", ", ItemIds);
                TotalChep.Text = Convert.ToString(ChepCount);
                TotalLoscam.Text = Convert.ToString(LoscamCount);
                TotalPlain.Text = Convert.ToString(PlainCount);
                Total.Text = Convert.ToString(ChepCount + LoscamCount + PlainCount);
                TotalWeight.Text = Convert.ToString(Weight);
            }
        }

        private async void btnBookPallet_ClickedAsync(object sender, EventArgs e)
        {
            if (ItemIds.Count > 0 && SelectedShipper?.SelectedItem != null && !string.IsNullOrWhiteSpace(ConsigmentNumber.Text))
            {
                var result = viewModel.UpdatePalletStatusWithShipper(ItemIds, "booked", ((PickList)SelectedShipper.SelectedItem).Name, ConsigmentNumber.Text).Result;
                if (result)
                {
                    SelectedPalletIds.Text = "";
                    TotalChep.Text = "";
                    TotalLoscam.Text = "";
                    TotalPlain.Text = "";
                    Total.Text = "";
                    TotalWeight.Text = "";
                    SelectedShipper.SelectedItem = null;
                    ItemIds.Clear();
                    ConsigmentNumber.Text = "";
                    await viewModel.GetPalletsByStatus("wrapped");
                    await DisplayAlert("Message", "Pallet has been booked successfully", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "Booking failed", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Warning", "Please enter all required inputs", "Ok");
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage { Detail = new NavigationPage(new PalletPage()) };
        }
    }
}