using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AriaPM.Models;
using AriaPM.ViewModels;

namespace AriaPM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Pallet
            {
                Id= 2,
                Status = "Status",
                SentDate="12/12/2020",
                StoreId=123,
                WrappedDate = "12/12/2020"
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}