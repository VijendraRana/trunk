using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using AriaPM.Models;
using AriaPM.Views;
using System.Collections.Generic;

namespace AriaPM.ViewModels
{
    public class PalletDispatchBookingViewModel : BaseViewModel
    {
        public ObservableCollection<Pallet> Items { get; set; }
        public ObservableCollection<PickList> Shippers { get; set; }
        public Command LoadItemsCommand { get; set; }

        public PalletDispatchBookingViewModel()
        {
            Title = "Booking/Dispatch";
            Items = new ObservableCollection<Pallet>();
            Shippers = new ObservableCollection<PickList>();
            LoadItemsCommand = new Command(async (status) => await ExecuteLoadItemsCommand(status));
        }

        async Task ExecuteLoadItemsCommand(object status)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var data = await DataStore.GetShipperDataAsync();
                Shippers.Clear();
                foreach (var item in data)
                {
                    Shippers.Add(item);
                }

                await GetPalletsByStatus(Convert.ToString(status));
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

        public async Task<bool> UpdatePalletStatusWithShipper(List<int> palletIds, string status, string shipper, string conNumber)
        {
            bool result = false;
            try
            {
                result = await DataStore.UpdatePalletStatusWithShipperAsync(palletIds, status, shipper, conNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public async Task GetPalletsByStatus(string status)
        {
            Items.Clear();
            var items = await DataStore.GetPalletsByStatusAsync(status);
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
    }
}