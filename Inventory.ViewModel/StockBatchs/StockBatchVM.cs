using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.StockBatchs
{
    public class StockBatchVM
    {
        public int? StockBatchId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double Quantity { get; set; }
        public string ExpiryStatus { get; set; }
        public bool IsDeleted { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int StockId { get; set; }

    }
}
