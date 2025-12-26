using Inventory.Models;
using Inventory.ViewModel.StockBatchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class StockDetailVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public List<StockPerWarehouseVM> StocksPerWarehouse { get; set; } = new();
    }

    public class StockPerWarehouseVM
    {
        public int StockId { get; set; }
        public string WarehouseName { get; set; } = default!;
        public double Quantity { get; set; }
        public List<StockBatchVM> Batches { get; set; } = new();
    }

   

}

