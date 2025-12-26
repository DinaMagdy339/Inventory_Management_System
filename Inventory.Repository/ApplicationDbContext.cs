using Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.BuyingPrice)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Currency)
                .WithMany()
                .HasForeignKey(p => p.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.MeasurementUnit)
                .WithMany()
                .HasForeignKey(p => p.MeasurementUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey("ProductTypeId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey("BrandId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
               .HasKey(s => s.StockId);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(s => s.WarehouseId);


            modelBuilder.Entity<StockBatch>()
                .HasOne(sb => sb.Stock)
                .WithMany(s => s.StockBatches)
                .HasForeignKey(sb => sb.StockId);


            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.StockBatch)
                .WithMany(sb => sb.InventoryTransactions)
                .HasForeignKey(it => it.StockBatchId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Currency)
                .WithMany()
                .HasForeignKey(p => p.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.BillType)
                .WithMany()
                .HasForeignKey(b => b.BillTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.GoodsReceivedNote)
                .WithMany()
                .HasForeignKey(b => b.GoodsReceivedNoteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
               .HasOne(c => c.CustomerType)
               .WithMany()
               .HasForeignKey(c => c.CustomerTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vendor>()
               .HasOne(v => v.VendorType)
               .WithMany()
               .HasForeignKey(v => v.VendorTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
               .HasOne(p => p.Branch)
               .WithMany()
               .HasForeignKey(p => p.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Vendor)
                .WithMany()
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.PurchaseType)
                .WithMany()
                .HasForeignKey(p => p.PurchaseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.Branch)
                .WithMany()
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.Currency)
                .WithMany()
                .HasForeignKey(s => s.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.SalesType)
                .WithMany()
                .HasForeignKey(s => s.SalesTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesOrderLine>()
                .HasOne(l => l.SalesOrder)
                .WithMany()
                .HasForeignKey(l => l.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesOrderLine>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.SalesOrder)
                .WithMany()
                .HasForeignKey(s => s.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.ShipmentType)
                .WithMany()
                .HasForeignKey(s => s.ShipmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
               .HasOne(s => s.Warehouse)
               .WithMany()
               .HasForeignKey(s => s.WarehouseId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentReceive>()
               .HasOne(p => p.Invoice)
               .WithMany()
               .HasForeignKey(p => p.InvoiceId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentReceive>()
                .HasOne(p => p.PaymentType)
                .WithMany()
                .HasForeignKey(p => p.PaymentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentVoucher>()
               .HasOne(p => p.Bill)
               .WithMany(b => b.PaymentVouchers)
               .HasForeignKey(p => p.BillId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentVoucher>()
                .HasOne(p => p.PaymentType)
                .WithMany()
                .HasForeignKey(p => p.PaymentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentVoucher>()
                .HasOne(p => p.CashBank)
                .WithMany()
                .HasForeignKey(p => p.CashBankId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.StockBatch)
                .WithMany()
                .HasForeignKey(ii => ii.StockBatchId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillType> BillTypes { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Currency> currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }
        public DbSet<MeasureUnit> MeasureUnits { get; set; }
        public DbSet<NumberSequence> NumberSequences { get; set; }
        public DbSet<PaymentReceive> PaymentReceives { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<PaymentVoucher> paymentVouchers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<PurchaseType> PurchaseTypes { get; set; }
        public DbSet<ReceivedNote> ReceivedNotes { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderLine> SalesOrderLines { get; set; }
        public DbSet<SalesType> SalesTypes { get; set; }
        public DbSet<Shipment> Shipment { get; set; }
        public DbSet<ShipmentType> ShipmentTypes { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorType> VendorTypes { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockBatch> StockBatchs { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
    }
}