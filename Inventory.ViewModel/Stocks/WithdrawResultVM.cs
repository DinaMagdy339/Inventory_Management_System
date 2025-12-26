using Inventory.Models;
using Inventory.ViewModel.Products;
using Inventory.ViewModel.StockBatchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class WithdrawResultVM
    {
        public bool Success { get; set; } = false;
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

        public List<StockBatchVM> Batches { get; set; } = new List<StockBatchVM>();
        public double RemainingInExpiry { get; set; }
        public double RemainingInBatch { get; set; }
        public double TotalRemaining { get; set; }

    }
}
