using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [Required]
        public string InvoiceName { get; set; }

        [Display(Name = "Shipment")]
        public int ShipmentId { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTimeOffset InvoiceDate { get; set; }
        public DateTimeOffset InvoiceDueDate { get; set; }

        [Display(Name = "Invoice Type")]
        public int InvoiceTypeId { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        
        public double TotalAmount => InvoiceItems.Sum(item => item.Total);
    }
}
