using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Bill
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
        public int BillTypeId { get; set; }
        public BillType BillType { get; set; }

        // Vendor
        [Required]
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }

        // GRN Relation
        [Display(Name = "GRN")]
        public int GoodsReceivedNoteId { get; set; }
        public ReceivedNote GoodsReceivedNote { get; set; }

        // Payment Vouchers
        public ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();
    }
}
