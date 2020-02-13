using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AriaPM.Models;
using AriaPM.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AriaPM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUpdatePallet : ContentPage
    {
        PalletItemViewModel viewModel;
        public PalletItem Item { get; set; }
        public int ItemId { get; set; }
        public int ItemNewId { get; set; }

        public AddUpdatePallet()
        {
            InitializeComponent();
            BindingContext = viewModel = new PalletItemViewModel();
            Item = new PalletItem();
        }
        public AddUpdatePallet(int palletId)
        {
            InitializeComponent();
            BindingContext = viewModel = new PalletItemViewModel();
            Item = new PalletItem();
            Item.Palletid = palletId;
            viewModel.Palletid = palletId;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            Pallet pallet = new Pallet();
            pallet.Barcode = Barcode.Text;
            pallet.StoreId = ((PickList)Store.SelectedItem)?.Id == 0 ? null : ((PickList)Store.SelectedItem)?.Id;
            pallet.Status = ((PickList)Status.SelectedItem)?.Name;
            pallet.Category = ((PickList)Category.SelectedItem)?.Name;
            pallet.Barcode = Barcode.Text?.Trim();
            pallet.Description = Description?.Text?.Trim();
            pallet.Id = viewModel.Palletid;
            pallet.WrappedDate = WrappedDate?.Text?.Trim();
            pallet.SentDate = SentDate?.Text?.Trim();
            pallet.FreightCompany = ((PickList)FreightCompany.SelectedItem)?.Name;
            pallet.ConNumber = ConNumber?.Text?.Trim();
            pallet.PackedBy = PackedBy?.Text?.Trim();
            pallet.OtherNotes = OtherNotes?.Text?.Trim();
            pallet.ReceivedDate = ReceivedDate?.Text?.Trim();
            pallet.ReceivedBy = ReceivedBy?.Text?.Trim();
            pallet.Contents = Contents?.Text?.Trim();
            pallet.WrappedBy = ((PickList)WrappedBy.SelectedItem)?.Name;
            pallet.BuiltBy = ((PickList)BuiltBy.SelectedItem)?.Name;
            pallet.Supplier = ((PickList)Supplier.SelectedItem)?.Name;
            pallet.Weight = Weight?.Text?.Trim();
            pallet.PalletType = ((PickList)PalletType.SelectedItem)?.Name;
            pallet.PalletItem = (new List<PalletItem>(viewModel.PalletItems)).Where(p => p.Id <= 0).ToList();

            var result = await viewModel.SavePallet(pallet);
            if (result)
            {
                App.Current.MainPage = new MainPage { Detail = new NavigationPage(new PalletPage()) };
                await DisplayAlert("Message", "Data has been saved", "Ok");
            }
            else
            {
                await DisplayAlert("Message", "Data not saved", "Ok");
            }
        }

        void Cancel_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage { Detail = new NavigationPage(new PalletPage()) };
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
            ItemId = 0;
            ItemNewId = 0;
            popupLoadingView.IsVisible = true;
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            popupLoadingView.IsVisible = false;
        }

        private async void Done_Clicked(object sender, EventArgs e)
        {
            Item = new PalletItem();
            Item.Description = Description.Text;
            Item.Barcode = Barcode.Text;
            Item.Inner = Inner.Text;
            Item.Outer = Outer.Text;
            Item.Quantity = Quatity.Text;
            Item.WrapperName = WrapperName.Text;
            Item.NewId = ItemNewId;
            Item.Id = ItemId;
            if (ItemId != 0 || ItemNewId!= 0)
            {
                foreach (var newItem in viewModel.PalletItems)
                {
                    if (ItemNewId < 0 && newItem.NewId == ItemNewId || ItemId > 0 && newItem.Id == ItemId)
                    {
                        viewModel.PalletItems.Remove(newItem);
                        break;
                    }
                }

                if (ItemId > 0)
                {
                    var result = await viewModel.UpdatePalletItem(Item);
                    if (result)
                    {
                        await DisplayAlert("Message", "Item has been updated successfully.", "Ok");
                    }
                }
            }
            else
            {
                Item.NewId = -1 - viewModel.PalletItems.Count();
            }
            viewModel.PalletItems.Add(Item);
            popupLoadingView.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Categories.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }

            if (Item.Palletid > 0)
            {
                var pallet = viewModel.GetPalletById(Item.Palletid).Result;
                Barcode.Text = pallet.Barcode;
                Store.SelectedItem = viewModel.Stores.FirstOrDefault(s => s.Id == pallet.StoreId);
                Status.SelectedItem = viewModel.Status.FirstOrDefault(s => s.Name == pallet.Status);
                Category.SelectedItem = viewModel.Categories.FirstOrDefault(s => s.Name == pallet.Category);
                Barcode.Text = pallet.Barcode;
                Description.Text = pallet.Description;
                Item.Palletid = pallet.Id;
                WrappedDate.Text = pallet.WrappedDate;
                SentDate.Text = pallet.SentDate;
                FreightCompany.SelectedItem = viewModel.Shippers.FirstOrDefault(s => s.Name == pallet.FreightCompany);
                ConNumber.Text = pallet.ConNumber;
                PackedBy.Text = pallet.PackedBy;
                OtherNotes.Text = pallet.OtherNotes;
                ReceivedDate.Text = pallet.ReceivedDate;
                ReceivedBy.Text = pallet.ReceivedBy;
                Contents.Text = pallet.Contents;
                WrappedBy.SelectedItem = viewModel.Wrappers.FirstOrDefault(s => s.Name == pallet.WrappedBy);
                BuiltBy.SelectedItem = viewModel.Builders.FirstOrDefault(s => s.Name == pallet.BuiltBy);
                Supplier.SelectedItem = viewModel.Suppliers.FirstOrDefault(s => s.Name == pallet.Supplier);
                Weight.Text = pallet.Weight;
                PalletType.SelectedItem = viewModel.PalletTypes.FirstOrDefault(s => s.Name == pallet.PalletType);
            }
        }

        private void OnEdit_Tapped(object sender, EventArgs e)
        {
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var id = Convert.ToInt32(item.CommandParameter);
            var editItem = viewModel.PalletItems.FirstOrDefault(p => p.Id == id);

            if (id == 0)
            {
                id = ((PalletItem)lblClicked.BindingContext).NewId;
                editItem = viewModel.PalletItems.FirstOrDefault(p => p.NewId == id);
            }

            Item = new PalletItem();
            Description.Text = editItem.Description;
            Barcode.Text = editItem.Barcode;
            Inner.Text = editItem.Inner;
            Outer.Text = editItem.Outer;
            Quatity.Text = editItem.Quantity;
            WrapperName.Text = editItem.WrapperName;
            ItemId = editItem.Id;
            ItemNewId = editItem.NewId;
            popupLoadingView.IsVisible = true;
        }

        private async void OnDelete_TappedAsync(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm", "Are you sure to delete this item?", "Yes", "No"))
            {
                Image lblClicked = (Image)sender;
                var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
                var id = Convert.ToInt32(item.CommandParameter);
                if (id > 0)
                {
                    var result = await viewModel.DeletePalletItem(id);
                    if (result)
                    {
                        foreach (var newItem in viewModel.PalletItems)
                        {
                            if (newItem.Id == id)
                            {
                                viewModel.PalletItems.Remove(newItem);
                                break;
                            }
                        }
                        await DisplayAlert("Message", "Item has been deleted successfully.", "Ok");
                        return;
                    }
                }
                else if (((Image)sender).BindingContext != null)
                {
                    viewModel.PalletItems.Remove((PalletItem)((Image)sender).BindingContext);
                    return;
                }
                await DisplayAlert("Message", "Item deletion failed.", "Ok");
            }
        }

    }
}