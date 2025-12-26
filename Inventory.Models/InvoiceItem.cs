using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }

        // الربط بالفاتورة
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        // المنتج
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // الباتش المختار
        public int StockBatchId { get; set; }
        public StockBatch StockBatch { get; set; }

        // الكمية المسحوبة
        [Range(1, double.MaxValue, ErrorMessage = "الكمية لازم تكون أكبر من صفر")]
        public double Quantity { get; set; }

        // السعر لكل وحدة
        [Range(0, double.MaxValue)]
        public double UnitPrice { get; set; }

        // الخصم لكل وحدة
        [Range(0, double.MaxValue)]
        public double UnitDiscount { get; set; }
        public bool IsPaid { get; set; } = false;

        // snapshot تاريخ الصلاحية وقت البيع
        public DateTime ExpiryDate { get; set; }

        // السعر النهائي للعنصر = (السعر - الخصم) * الكمية
        public double Total => (UnitPrice - UnitDiscount) * (double)Quantity;
    }
}
