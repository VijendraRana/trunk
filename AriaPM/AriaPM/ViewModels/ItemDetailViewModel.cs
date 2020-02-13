using System;

using AriaPM.Models;

namespace AriaPM.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Pallet Item { get; set; }
        public ItemDetailViewModel(Pallet item = null)
        {
            Title = item?.Status;
            Item = item;
        }
    }
}
