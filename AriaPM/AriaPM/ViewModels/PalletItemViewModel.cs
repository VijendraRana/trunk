using AriaPM.Models;
using AriaPM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AriaPM.ViewModels
{
    public class PalletItemViewModel
    {
        public ObservableCollection<PalletItem> PalletItems { get; set; }
        public int Palletid { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ObservableCollection<PickList> Stores { get; set; }
        public ObservableCollection<PickList> Status { get; set; }
        public ObservableCollection<PickList> Categories { get; set; }
        public ObservableCollection<PickList> Shippers { get; set; }
        public ObservableCollection<PickList> Wrappers { get; set; }
        public ObservableCollection<PickList> Builders { get; set; }
        public ObservableCollection<PickList> Suppliers { get; set; }
        public ObservableCollection<PickList> PalletTypes { get; set; }

        public IPalletService<Pallet> DataStore => DependencyService.Get<IPalletService<Pallet>>() ?? new PalletService();
        public PalletItemViewModel()
        {
            Stores = new ObservableCollection<PickList>();
            Status = new ObservableCollection<PickList>();
            Categories = new ObservableCollection<PickList>();
            Suppliers = new ObservableCollection<PickList>();
            Builders = new ObservableCollection<PickList>();
            Wrappers = new ObservableCollection<PickList>();
            Shippers = new ObservableCollection<PickList>();
            PalletTypes = new ObservableCollection<PickList>();
            PalletItems = new ObservableCollection<PalletItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
                Stores.Clear();
                var palletMaster = await DataStore.GetPalletMaterDataAsync();
                foreach (var item in palletMaster.Stores)
                {
                    Stores.Add(item);
                }

                Status.Clear();
                foreach (var item in palletMaster.Status)
                {
                    Status.Add(item);
                }

                Categories.Clear();
                foreach (var item in palletMaster.Categories)
                {
                    Categories.Add(item);
                }

                Wrappers.Clear();
                foreach (var item in palletMaster.Wrappers)
                {
                    Wrappers.Add(item);
                }

                Shippers.Clear();
                foreach (var item in palletMaster.Shippers)
                {
                    Shippers.Add(item);
                }

                Suppliers.Clear();
                foreach (var item in palletMaster.Suppliers)
                {
                    Suppliers.Add(item);
                }

                Builders.Clear();
                foreach (var item in palletMaster.Builders)
                {
                    Builders.Add(item);
                }

                PalletTypes.Clear();
                foreach (var item in palletMaster.PalletTypes)
                {
                    PalletTypes.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task<bool> SavePallet(Pallet pallet)
        {
            bool result = false;
            try
            {
                result = await DataStore.AddUpdatePalletAsync(pallet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public async Task<Pallet> GetPalletById(int id)
        {
            Pallet result = new Pallet();
            try
            {
                result = await DataStore.GetPalletAsync(id);
                var items =  new ObservableCollection<PalletItem>(result.PalletItem);
                PalletItems.Clear();
                foreach (var item in items)
                {
                    PalletItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public async Task<bool> DeletePalletItem(int id)
        {
            bool result = false;
            try
            {
                result = await DataStore.DeletePalletItemAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public async Task<bool> UpdatePalletItem(PalletItem palletItem)
        {
            bool result = false;
            try
            {
                result = await DataStore.UpdatePalletItemAsync(palletItem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }
    }
}
