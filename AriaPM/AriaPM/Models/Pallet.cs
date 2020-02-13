using System;
using System.Collections.Generic;

namespace AriaPM.Models
{
    public class Pallet
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string WrappedDate { get; set; }
        public string SentDate { get; set; }
        public string Supplier { get; set; }
        public int? StoreId { get; set; }
        public string ReceivedDate { get; set; }
        public string Category { get; set; }
        public string ConNumber { get; set; }
        public string StoreRequest { get; set; }
        public string PalletType { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }
        public int Total { get; set; }
        public int PageNumber { get; set; }
        public string FreightCompany { get; set; }
        public string PackedBy { get; set; }
        public string OtherNotes { get; set; }
        public string ReceivedBy { get; set; }
        public string Contents { get; set; }
        public string BuiltBy { get; set; }
        public string WrappedBy { get; set; }

        public List<PalletItem> PalletItem { get; set; }
    }
}