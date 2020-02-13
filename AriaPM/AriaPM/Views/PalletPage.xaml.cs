using AriaPM.Models;
using AriaPM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AriaPM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PalletPage : ContentPage
    {
        PalletViewModel viewModel;
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        private int _itemId = 0;
        private string _status = string.Empty;
        public PalletItem Item { get; set; }

        public PalletPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new PalletViewModel();
            lblTotalRecords.Text = viewModel.TotalRecordsText;
            lblPageNumber.Text = viewModel.PageNumberText;
            btnNext.IsVisible = false;
            btnPrev.IsVisible = false;
            viewModel.IsAdmin = Convert.ToBoolean(Application.Current.Properties["IsAdmin"]);
            if (!viewModel.IsAdmin)
            {
                PalletContent.ToolbarItems.Clear();
            }

            BtnAddItem.IsEnabled = false;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
        }

        void AddPallet_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage { Detail = new NavigationPage(new AddUpdatePallet()) };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
                lblTotalRecords.Text = viewModel.TotalRecordsText;
                lblPageNumber.Text = viewModel.PageNumberText;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            GetPalletDetailsAsync();
        }

        private void Prev_Clicked(object sender, EventArgs e)
        {
            if (lblPageNumber != null && lblPageNumber.Text != null && lblPageNumber.Text.Split(' ')[1] != null)
            {
                int pageNumber = Convert.ToInt32((lblPageNumber.Text.Split(' '))[1]);
                GetPalletDetailsAsync(pageNumber - 1);
            }
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            if (lblPageNumber != null && lblPageNumber.Text != null && lblPageNumber.Text.Split(' ')[1] != null)
            {
                int pageNumber = Convert.ToInt32((lblPageNumber.Text.Split(' '))[1]);
                GetPalletDetailsAsync(pageNumber + 1);
            }
        }

        private async void GetPalletDetailsAsync(int PageNumber = 1)
        {
            Pallet palletItem = new Pallet(); ;

            if (SearchStore?.SelectedItem != null || SearchStatus?.SelectedItem != null || SearchCategory?.SelectedItem != null ||
                !string.IsNullOrWhiteSpace(SearchBarcode?.Text?.Trim()) || !string.IsNullOrWhiteSpace(SearchDescription?.Text?.Trim()) ||
                !string.IsNullOrWhiteSpace(SearchPalletId?.Text?.Trim()))
            {
                palletItem.StoreId = ((PickList)SearchStore.SelectedItem)?.Id == 0 ? null : ((PickList)SearchStore.SelectedItem)?.Id;
                palletItem.Status = ((PickList)SearchStatus.SelectedItem)?.Name;
                palletItem.Category = ((PickList)SearchCategory.SelectedItem)?.Name;
                palletItem.Barcode = SearchBarcode.Text?.Trim();
                palletItem.Description = SearchDescription?.Text?.Trim();
                palletItem.Id = string.IsNullOrWhiteSpace(SearchPalletId.Text?.Trim()) ? 0 : Convert.ToInt32(SearchPalletId.Text?.Trim());
            }

            palletItem.PageNumber = PageNumber;
            await viewModel.SearchPallets(palletItem);
            lblTotalRecords.Text = viewModel.TotalRecordsText;
            lblPageNumber.Text = viewModel.PageNumberText;

            btnNext.IsVisible = true;
            btnPrev.IsVisible = true;

            if (PageNumber <= 1)
            {
                btnPrev.IsEnabled = false;
            }
            else
            {
                btnPrev.IsEnabled = true;
            }

            if (PageNumber < (viewModel.TotalRecords / 20) || viewModel.TotalRecords % 20 > 0 && (PageNumber - 1 < (viewModel.TotalRecords / 20)))
            {
                btnNext.IsEnabled = true;
            }
            else
            {
                btnNext.IsEnabled = false;
            }
        }

        private void OnEdit_Tapped(object sender, EventArgs e)
        {
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var id = Convert.ToInt32(item.CommandParameter);
            App.Current.MainPage = new MainPage { Detail = new NavigationPage(new AddUpdatePallet(id)) };
        }

        private void OnStatus_Tapped(object sender, EventArgs e)
        {
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            _itemId = Convert.ToInt32(item.CommandParameter);
            _status = item.ClassId;
            ChangedStatus.SelectedItem = viewModel.Status.FirstOrDefault(s => s.Name == _status);
            popupLoadingView.IsVisible = true;
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            popupLoadingView.IsVisible = false;
        }

        private async void Done_Clicked(object sender, EventArgs e)
        {
            string changedStatus = ((PickList)ChangedStatus.SelectedItem)?.Name;

            if (_itemId > 0 && !string.IsNullOrWhiteSpace(changedStatus))
            {
                if (changedStatus != _status)
                {
                    var result = await viewModel.UpdatePalletStatus(changedStatus, _itemId);
                    if (result)
                    {
                        viewModel.Items.FirstOrDefault(i => i.Id == _itemId).Status = changedStatus;
                        var updatedList = new List<Pallet>(viewModel.Items);
                        viewModel.Items.Clear();

                        foreach (var item in updatedList)
                        {
                            viewModel.Items.Add(item);
                        }

                        _itemId = 0;
                        _status = string.Empty;
                        popupLoadingView.IsVisible = false;
                        await DisplayAlert("Message", "Status has been updated successfully.", "Ok");
                        return;
                    }
                }
            }

            await DisplayAlert("Message", "Status updation failed.", "Ok");
        }

        private async void OnDelete_TappedAsync(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm", "Are you sure to delete this Pallet?", "Yes", "No"))
            {
                Image lblClicked = (Image)sender;
                var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
                var id = Convert.ToInt32(item.CommandParameter);
                if (id > 0)
                {
                    var result = await viewModel.DeletePallet(id);
                    if (result)
                    {
                        GetPalletDetailsAsync(viewModel.PageNumber);
                        await DisplayAlert("Message", "Pallet has been deleted successfully.", "Ok");
                        return;
                    }
                }
                await DisplayAlert("Message", "Pallet deletion failed.", "Ok");
            }
        }

        private void PalletListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Pallet;
            if (item == null)
                return;

            if (item.Id > 0)
            {
                var pallet = viewModel.GetPalletById(item.Id).Result;
                Status.SelectedItem = viewModel.Status.FirstOrDefault(s => s.Name == pallet.Status);
                Category.SelectedItem = viewModel.Categories.FirstOrDefault(s => s.Name == pallet.Category);
                PalletId.Text = Convert.ToString(pallet.Id);
                WrappedBy.SelectedItem = viewModel.Wrappers.FirstOrDefault(s => s.Name == pallet.WrappedBy);
                Weight.Text = pallet.Weight;
                PalletType.Text = pallet.PalletType;
                Contents.Text = pallet.Contents;
                BtnAddItem.IsEnabled = true;
            }
        }

        private void AddItem_Clicked(object sender, EventArgs e)
        {
            Item = new PalletItem();
            Description.Text = "";
            Barcode.Text = "";
            Inner.Text = "";
            Outer.Text = "";
            Quatity.Text = "";
            WrapperName.Text = "";
            popupAddItemView.IsVisible = true;
        }

        private void ItemClose_Clicked(object sender, EventArgs e)
        {
            popupAddItemView.IsVisible = false;
        }

        private async void ItmDone_Clicked(object sender, EventArgs e)
        {
            Item = new PalletItem();
            Item.Palletid = Convert.ToInt32(PalletId.Text);
            Item.Description = Description.Text;
            Item.Barcode = Barcode.Text;
            Item.Inner = Inner.Text;
            Item.Outer = Outer.Text;
            Item.Quantity = Quatity.Text;
            Item.WrapperName = WrapperName.Text;
            var result = await viewModel.SavePalletItem(Item);

            if (result)
            {
                await viewModel.GetPalletById(Item.Palletid);
                popupAddItemView.IsVisible = false;
                await DisplayAlert("Message", "Item has been saved", "Ok");
            }
            else
            {
                await DisplayAlert("Message", "Item not saved", "Ok");
            }
        }

        private void ToolbarBooking_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage { Detail = new NavigationPage(new PalletBookingPage()) };
        }

        private void ToolbarDispatch_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage { Detail = new NavigationPage(new PalletDispatchPage()) };
        }
    }
}