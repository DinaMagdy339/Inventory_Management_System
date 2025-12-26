using Inventory.Models;
using Inventory.ViewModel.Products;
using Inventory.ViewModel.StockBatchs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Stocks
{
    public class WithdrawForInvoiceVM
    {
        public string? ProductSearch { get; set; }
        public int? ProductId { get; set; }
        public int? StockBatchId { get; set; }
        public double Quantity { get; set; }

        public double? UnitPrice { get; set; }     
        public double? UnitDiscount { get; set; }

        public string? CustomerName { get; set; }
        public int? InvoiceId { get; set; }


        public List<WithdrawItemVM> SelectedProducts { get; set; } = new List<WithdrawItemVM>();

        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<StockBatchVM> Batches { get; set; } = new List<StockBatchVM>();
        public List<Invoice> OpenInvoices { get; set; } = new List<Invoice>();
        public string? Remarks { get; set; }
    }

}

