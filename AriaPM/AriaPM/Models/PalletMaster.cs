using System;
using System.Collections.Generic;
using System.Text;

namespace AriaPM.Models
{
    public class PalletMaster
    {
        public List<PickList> Stores { get; set; }
        public List<PickList> Status { get; set; }
        public List<PickList> Categories { get; set; }
        public List<PickList> Shippers { get; set; }
        public List<PickList> Wrappers { get; set; }
        public List<PickList> Builders { get; set; }
        public List<PickList> Suppliers { get; set; }
        public List<PickList> PalletTypes { get; set; }

    }
}
