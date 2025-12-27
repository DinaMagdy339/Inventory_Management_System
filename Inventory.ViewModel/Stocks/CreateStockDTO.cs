using Inventory.ViewModel.StockBatchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class CreateStockDTO
    {
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public double Quantity { get; set; }

        public List<StockBatchDTO> Batches { get; set; } = new List<StockBatchDTO>();
    }
}
