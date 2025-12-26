using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Stock
    {
        public int StockId { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public double Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<StockBatch> StockBatches { get; set; } = new HashSet<StockBatch>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new HashSet<InventoryTransaction>();

    }
}
