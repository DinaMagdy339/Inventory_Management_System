using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Bills
{
    public class BillFilterationViewModel
    {
        public string? VendorName { get; set; } 
        public string? BillTypeName { get; set; }
        public string? BillName { get; set; } 
        public  string? GoodsReceivedNote { get; set; }
        public string? PaymentStatus { get; set; } 
        public bool SortByDueDateAsc { get; set; } 
    }

}
