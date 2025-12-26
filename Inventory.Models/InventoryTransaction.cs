using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class InventoryTransaction
    {
        public int InventoryTransactionId { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        [Display(Name = "Invoice")]
        public int? InvoiceId { get; set; }
        public Invoice invoice { get; set; }

        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Display(Name = "Batch")]
        public int? StockBatchId { get; set; }
        public StockBatch StockBatch { get; set; }

        [Required]
        public double Quantity { get; set; }
        public DateTime CreatedAt{ get; set; }
        public DateTime UpdatedAt{ get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;
        [StringLength(500)]
        public string Remarks { get; set; }
    }
    public enum TransactionType
    {
        Receive,
        Issue,
        Transfer,
        WriteOff
    }

}

