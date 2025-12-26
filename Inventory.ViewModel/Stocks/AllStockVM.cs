using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class AllStockVM
    {
        public int StockId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public double TotalQuantity { get; set; } 

    }
}
