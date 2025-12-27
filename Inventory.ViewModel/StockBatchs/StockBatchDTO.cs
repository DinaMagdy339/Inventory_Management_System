using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.StockBatchs
{
    public class StockBatchDTO
    {
        public int StockID { get; set; }
        public double Quantity { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
}
