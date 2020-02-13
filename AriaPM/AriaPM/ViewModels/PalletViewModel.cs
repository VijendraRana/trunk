using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using AriaPM.Models;
using AriaPM.Views;

namespace AriaPM.ViewModels
{
    public class PalletViewModel : BaseViewModel
    {
        public ObservableCollection<Pallet> Items { get; set; }
        public ObservableCollection<PickList> Stores { get; set; }
        public ObservableCollection<Models.PickList> Status { get; set; }
        public ObservableCollection<PickList> Categories { get; set; }
        public PickList SelectedCategory { get; set; }
        public Models.PickList SelectedStatus { get; set; }
        public PickList SelectedStore { get; set; }
        public Command LoadItemsCommand { get; set; }
        public string TotalRecordsText { get; set; }
        public string PageNumberText { get; set; }
        public int PageNumber { get; set; }
        public int TotalRecords { get; set; }
        public bool IsAdmin { get; internal set; }
        public ObservableCollection<PalletItem> PalletItems { get; set; }
        public ObservableCollection<PickList> Wrappers { get; set; }


        public PalletViewModel()
        {
            Title = "Pallets";
            Items = new ObservableCollection<Pallet>();
            Stores = new ObservableCollection<PickList>();
            Status = new ObservableCollection<Models.PickList>();
            Categories = new ObservableCollection<PickList>();
            PalletItems = new ObservableCollection<PalletItem>();
            Wrappers = new ObservableCollection<PickList>();
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
                var palletMaster = await DataStore.GetPalletMaterDataAsync();
                Stores.Clear();

                if (!Convert.ToBoolean(Application.Current.Properties["IsAdmin"]))
                {
                    var userId = Convert.ToString(Application.Current.Properties["UserId"]);
                    Stores.Add( palletMaster.Stores.Find(s => string.Equals(s.Name, userId.Trim(), StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    foreach (var item in palletMaster.Stores)
                    {
                        Stores.Add(item);
                    }
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

        public async Task SearchPallets(Pallet palletItem)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(palletItem);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                SetPagination(palletItem.PageNumber);
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

        public async Task<bool> DeletePallet(int palletId)
        {
            bool result = false;
            try
            {
                result = await DataStore.DeletePalletAsync(palletId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        public async Task<bool> UpdatePalletStatus(string status, int palletId)
        {
            bool result = false;
            try
            {
                result = await DataStore.UpdatePalletStatusAsync(status, palletId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        private void SetPagination(int pageNumber = 1)
        {
            int total = 0;
            int totalPage = 0;
            if (Items != null && Items.Count > 0 && Items[0] != null)
            {
                total = Items[0].Total;
                totalPage = total / 20 + (total % 20 > 0 ? 1 : 0);
            }

            PageNumber = pageNumber;
            TotalRecordsText = "Total records found : " + total;
            PageNumberText = "Page " + pageNumber + " of " + totalPage;
            TotalRecords = total;
        }

        public async Task<Pallet> GetPalletById(int id)
        {
            Pallet result = new Pallet();
            try
            {
                result = await DataStore.GetPalletAsync(id);
                var items = new ObservableCollection<PalletItem>(result.PalletItem);
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

        public async Task<bool> SavePalletItem(PalletItem palletItem)
        {
            bool result = false;
            try
            {
                result = await DataStore.AddPalletItemAsync(palletItem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }
    }
}