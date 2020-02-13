using System;

namespace AriaPM.Models
{
    public class PalletItem
    {
        public int Id { get; set; }
        public string Quantity { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string Outer { get; set; }
        public string Inner { get; set; }
        public string WrapperName { get; set; }
        public int Palletid { get; set; }
        public int NewId { get; set; }
    }
}