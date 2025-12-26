using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? Barcode { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImageUrl { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }

        [Display(Name = "Measurement Unit")]
        public int MeasurementUnitId { get; set; }
        public MeasureUnit? MeasurementUnit { get; set; }

  
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();


    }
}
