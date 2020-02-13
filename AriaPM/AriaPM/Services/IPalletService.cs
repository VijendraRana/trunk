using AriaPM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AriaPM.Services
{
    public interface IPalletService<T>
    {
        Task<bool> AddUpdatePalletAsync(T item);
        Task<bool> UpdatePalletStatusAsync(string status, int palletId);
        Task<bool> DeletePalletAsync(int palletId);
        Task<T> GetPalletAsync(int id);
        Task<IEnumerable<T>> GetItemsAsync(Pallet palletItem = null);
        Task<IEnumerable<PickList>> GetStoresAsync();
        Task<IEnumerable<PickList>> GetStatusAsync();
        Task<IEnumerable<PickList>> GetCategoriesAsync();
        Task<PalletMaster> GetPalletMaterDataAsync();
        Task<bool> AddPalletItemAsync(PalletItem palletItem);
        Task<IEnumerable<Pallet>> GetPalletsByStatusAsync(string status);
        Task<List<PickList>> GetShipperDataAsync();
        Task<bool> UpdatePalletStatusWithShipperAsync(List<int> palletIds, string status, string shipper, string conNumber);
        Task<bool> DeletePalletItemAsync(int id);
        Task<bool> UpdatePalletItemAsync(PalletItem item);
    }
}
