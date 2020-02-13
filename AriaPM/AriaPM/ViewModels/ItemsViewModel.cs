using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using AriaPM.Models;
using AriaPM.Views;

namespace AriaPM.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Pallet> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Pallets";
            Items = new ObservableCollection<Pallet>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<AddUpdatePallet, Pallet>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Pallet;
                Items.Add(newItem);
                await DataStore.AddUpdatePalletAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}