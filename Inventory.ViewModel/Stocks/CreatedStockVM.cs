using Inventory.ViewModel.StockBatchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class CreatedStockVM
    {
        public int StockId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public double Quantity { get; set; }


        public List<StockBatchVM> Batches { get; set; } = new List<StockBatchVM>();

    }
}
