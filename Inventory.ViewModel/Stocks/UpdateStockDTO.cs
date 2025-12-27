using Inventory.ViewModel.StockBatchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class UpdateStockDTO
    {
        public int StockId { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public double Quantity { get; set; }

        public List<UpdateBatchDTO> Batches { get; set; } = new List<UpdateBatchDTO>();
    }
}
