using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class WithdrawItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int StockBatchId { get; set; }
        public string BatchNumber { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double UnitDiscount { get; set; }
    }
}
