using System.ComponentModel.DataAnnotations;
using Inventory.ViewModel.PaymentVouchers;

namespace Inventory.ViewModel.Bills
{
    public class AllBillViewModel
    {
        public int BillId { get; set; }

        [Required]
        [Display(Name = "Bill Name")]
        public string BillName { get; set; }

        [Display(Name = "Vendor Delivery Order")]
        public string VendorDeliveryNumber { get; set; }

        [Display(Name = "Vendor Invoice Number")]
        public string VendorInvoiceNumber { get; set; }

        [Display(Name = "Bill Date")]
        public DateTimeOffset BillDate { get; set; }

        [Display(Name = "Bill Due Date")]
        public DateTimeOffset BillDueDate { get; set; }

        // BillType
        [Display(Name = "Bill Type")]
        [Required]
        public int BillTypeId { get; set; }
        public string BillTypeName { get; set; } 

        // Vendor
        [Display(Name = "Vendor")]
        [Required]
        public int VendorId { get; set; }
        public string VendorName { get; set; } 

        // GRN / ReceivedNote
        [Display(Name = "GRN")]
        [Required]
        public int GoodsReceivedNoteId { get; set; }
        public string GRNNumber { get; set; } 
        public DateTimeOffset GRNDate { get; set; } 

        public ICollection<PaymentVoucherViewModel> PaymentVouchers { get; set; } = new List<PaymentVoucherViewModel>();

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } 
    }

}
