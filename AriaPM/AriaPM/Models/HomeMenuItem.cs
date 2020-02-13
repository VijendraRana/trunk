using System;
using System.Collections.Generic;
using System.Text;

namespace AriaPM.Models
{
    public enum MenuItemType
    {
        Pallets,
        Warehouse,
        Distribution,
        Logout,
        Login,
        AddUpdatePallet
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }
        public string Title { get; set; }
    }
}
