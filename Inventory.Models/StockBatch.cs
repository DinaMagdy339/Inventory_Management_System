using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class StockBatch
    {
        public int StockBatchId { get; set; }

        [Display(Name = "Stock")]
        public int StockId { get; set; }
        [Required]
        public string BatchNumber { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public double Quantity { get; set; }
       
        // Navigation Properties
        public Stock Stock { get; set; }

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new HashSet<InventoryTransaction>();

    }
}
