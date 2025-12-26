using Inventory.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Products
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public string ProductDescription { get; set; }
        public string? ProductImageUrl { get; set; }
        [Required(ErrorMessage = "Please select an image")]

        public IFormFile ProductImageFile { get; set; } 

        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string MeasurementUnit { get; set; }
        public string Currency { get; set; }
        public string ProductType { get; set; }
        public string Brand { get; set; }
        public double TotalQuantity { get; set; }




    }
}
