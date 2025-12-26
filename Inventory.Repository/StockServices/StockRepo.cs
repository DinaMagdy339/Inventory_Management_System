using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Customers;
using Inventory.ViewModel.Products;
using Inventory.ViewModel.StockBatchs;
using Inventory.ViewModel.Stocks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Inventory.Repository.StockServices
{
    public class StockRepo : IStockRepo
    {
        private readonly ApplicationDbContext _context;
        public StockRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<AllStockVM>> GetAllStocksAsync(int pageNum, int pageSize, string? searchTerm)
        {
            var query = _context.Stocks
                .Include(s => s.Product)
                .Where(s => s.IsActive)
                .GroupBy(s => new { s.ProductId, s.Product.ProductName })
                .Select(g => new AllStockVM
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(x => x.Quantity)
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.ProductName.ToLower().Contains(searchTerm.ToLower()));
            }

            return await PaginatedList<AllStockVM>.CreateAsync(query, pageNum, pageSize);
        }
        public async Task<StockDetailVM> GetStockByIdAsync(int productId)
        {
            var stocks = await _context.Stocks
                .Include(s => s.Warehouse)
                .Include(s => s.Product)
                .Include(s => s.StockBatches)
                .Where(s => s.ProductId == productId && s.IsActive)
                .ToListAsync();

            if (!stocks.Any())
                return null;

            var vm = new StockDetailVM
            {
                ProductId = productId,
                ProductName = stocks.First().Product.ProductName,
                StocksPerWarehouse = new List<StockPerWarehouseVM>()
            };

            foreach (var stock in stocks)
            {
                var stockVM = new StockPerWarehouseVM
                {
                    StockId = stock.StockId,
                    WarehouseName = stock.Warehouse.WarehouseName,
                    Quantity = stock.Quantity,
                    Batches = stock.StockBatches
                        .Select(b => new StockBatchVM
                        {
                            StockBatchId = b.StockBatchId,
                            BatchNumber = b.BatchNumber,
                            Quantity = b.Quantity,
                            ExpiryDate = b.ExpiryDate,
                            ExpiryStatus = GetExpiryStatus(b.ExpiryDate)
                        }).ToList()
                };

                vm.StocksPerWarehouse.Add(stockVM);
            }

            return vm;
        }
        public async Task<CreatedStockVM> GetStockByStockIdAsync(int stockId)
        {
            var stock = await _context.Stocks
                .Include(s => s.StockBatches)
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.StockId == stockId && s.IsActive);

            if (stock == null)
                return null;

            var vm = new CreatedStockVM
            {
                StockId = stock.StockId,
                ProductName = stock.Product.ProductName,
                WarehouseName = stock.Warehouse.WarehouseName,
                Quantity = stock.Quantity,
                Batches = stock.StockBatches.Select(b => new StockBatchVM
                {
                    StockBatchId = b.StockBatchId,
                    BatchNumber = b.BatchNumber,
                    Quantity = b.Quantity,
                    ExpiryDate = b.ExpiryDate
                }).ToList() ?? new List<StockBatchVM>()
            };

            return vm;
        }

        public void AddStock(CreatedStockVM stock)
        {
            
            var warehouseId = _context.Warehouses
                .Where(w => w.WarehouseName == stock.WarehouseName)
                .Select(w => w.WarehouseId)
                .FirstOrDefault();

            var productId = _context.Products
                .Where(p => p.ProductName == stock.ProductName)
                .Select(p => p.ProductId)
                .FirstOrDefault();

            var newStock = new Stock
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = stock.Quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Stocks.Add(newStock);
            _context.SaveChanges(); 

            if (stock.Batches != null && stock.Batches.Any())
            {
                foreach (var batch in stock.Batches)
                {
                    var newBatch = new StockBatch
                    {
                        StockId = newStock.StockId, 
                        BatchNumber = batch.BatchNumber,
                        Quantity = batch.Quantity,
                        ExpiryDate = batch.ExpiryDate
                    };
                    _context.StockBatchs.Add(newBatch);
                }
                _context.SaveChanges();
            }
        }
        public void Delete(int id)
        {
            var stock = _context.Stocks.Find(id);
            if (stock != null)
            {
                try
                {
                    stock.IsActive = false;
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                    {
                        throw new InvalidOperationException(
                            "لا يمكن الحذف لأنه مرتبط ببيانات أخرى"
                        );
                    }

                    throw;
                }

            }
        }
        public void Update(CreatedStockVM stock)
        {
            var existingStock = _context.Stocks
                .Include(s => s.StockBatches)
                .FirstOrDefault(s => s.StockId == stock.StockId && s.IsActive);
            var ProductId = _context.Products.Where(s => s.ProductName == stock.ProductName)
                .Select(s => s.ProductId)
                .FirstOrDefault();

            var WarehouseId = _context.Warehouses.Where(s => s.WarehouseName == stock.WarehouseName)
                .Select(s => s.WarehouseId)
                .FirstOrDefault();

            existingStock.Quantity = stock.Quantity;
            existingStock.UpdatedAt = DateTime.Now;
            existingStock.ProductId = ProductId;
            existingStock.WarehouseId = WarehouseId;
            foreach (var batch in stock.Batches)
            {
                if (batch.StockBatchId.HasValue)
                {
                    var existingBatch = existingStock.StockBatches
                        .FirstOrDefault(b => b.StockBatchId == batch.StockBatchId.Value);

                    if (existingBatch != null)
                    {
                        if (batch.IsDeleted)
                        {
                            _context.StockBatchs.Remove(existingBatch);
                        }
                        else
                        {
                            existingBatch.BatchNumber = batch.BatchNumber;
                            existingBatch.Quantity = batch.Quantity;
                            existingBatch.ExpiryDate = batch.ExpiryDate;
                        }
                    }
                }
                else
                {
                    var newBatch = new StockBatch
                    {
                        StockId = existingStock.StockId,
                        BatchNumber = batch.BatchNumber,
                        Quantity = batch.Quantity,
                        ExpiryDate = batch.ExpiryDate
                    };
                    _context.StockBatchs.Add(newBatch);
                }
            }

            _context.SaveChanges();
        }


       
        #region Withdraw
        public async Task<List<ProductViewModel>> GetProductsAsync(string search)
        {
            return await _context.Products
                .Where(p => p.ProductName.ToLower().Contains(search.ToLower()))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    TotalQuantity = p.Stocks
                        .Where(s => s.IsActive)
                        .SelectMany(s => s.StockBatches)
                        .Where(b => b.Quantity > 0 && b.ExpiryDate >= DateTime.Today)
                        .Sum(b => (double?)b.Quantity) ?? 0
                })
                .Where(p => p.TotalQuantity > 0)
                .ToListAsync();
        }

        public async Task<List<StockBatchVM>> GetBatchesAsync(int productId)
        {
            return await _context.StockBatchs
                .Include(b => b.Stock)
                .ThenInclude(s => s.Warehouse)
                .Where(b =>
                        b.Stock != null &&
                        b.Stock.Warehouse != null &&
                        b.Stock.ProductId == productId &&
                        b.Stock.IsActive &&
                        b.Quantity > 0 &&
                        b.ExpiryDate >= DateTime.Today)
                .OrderBy(b => b.ExpiryDate)
                .Select(b => new StockBatchVM
                {
                    StockBatchId = b.StockBatchId,
                    BatchNumber = b.BatchNumber,
                    Quantity = b.Quantity,
                    ExpiryDate = b.ExpiryDate,
                    WarehouseName = b.Stock.Warehouse.WarehouseName,
                    StockId = b.Stock.StockId
                })
                .ToListAsync();
        }

        public async Task<bool> WithdrawMultipleAsync(List<WithdrawItemVM> items, string customerName, int? invoiceId, string remarks = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!items.Any())
                    throw new Exception("لا توجد منتجات للسحب.");

                Invoice invoice = null;
                Customer customer = null;

                if (!string.IsNullOrWhiteSpace(customerName))
                {
                    customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerName == customerName);
                    if (customer == null) throw new Exception("العميل غير موجود.");

                    if (invoiceId.HasValue)
                    {
                        invoice = await _context.Invoices
                            .Include(i => i.InvoiceItems)
                            .Include(i => i.Customer)
                            .Where(i => i.CustomerId == customer.CustomerId)
                            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId.Value);
                    }

                    if (invoice == null)
                    {
                        invoice = new Invoice
                        {
                            CustomerId = customer.CustomerId,
                            InvoiceDate = DateTimeOffset.Now,
                            InvoiceDueDate = DateTimeOffset.Now.AddDays(30),
                            InvoiceName = $"فاتورة {customer.CustomerName} - {DateTime.Today:dd/MM/yyyy}",
                            InvoiceItems = new List<InvoiceItem>()
                        };
                        _context.Invoices.Add(invoice);
                        await _context.SaveChangesAsync();
                    }
                }

                foreach (var item in items)
                {
                    var batch = await _context.StockBatchs
                        .Include(b => b.Stock)
                        .FirstOrDefaultAsync(b => b.StockBatchId == item.StockBatchId && b.Stock.IsActive);

                    if (batch == null)
                        throw new Exception($"الباتش {item.BatchNumber} غير موجود.");

                    if (item.Quantity <= 0 || item.Quantity > batch.Quantity)
                        throw new Exception($"كمية غير صحيحة للمنتج {batch.Stock.Product.ProductName}.");

                    batch.Quantity -= item.Quantity;
                    batch.Stock.Quantity -= item.Quantity;

                    if (invoice != null)
                    {
                        _context.InvoiceItems.Add(new InvoiceItem
                        {
                            InvoiceId = invoice.InvoiceId,
                            ProductId = batch.Stock.ProductId,
                            StockBatchId = batch.StockBatchId,
                            Quantity = item.Quantity,
                            ExpiryDate = batch.ExpiryDate,
                            UnitPrice = item.UnitPrice,
                            UnitDiscount = item.UnitDiscount
                        });
                    }

                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductId = batch.Stock.ProductId,
                        WarehouseId = batch.Stock.WarehouseId,
                        StockBatchId = batch.StockBatchId,
                        Quantity = item.Quantity,
                        TransactionType = TransactionType.Issue,
                        TransactionDate = DateTime.Now,
                        InvoiceId = invoice?.InvoiceId,
                        CustomerId = customer?.CustomerId,
                        Remarks = remarks ?? "سحب مخزون"
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion


        public string GetExpiryStatus(DateTime expiryDate)
        {
            if (expiryDate < DateTime.Now) return "Expired";
            if (expiryDate <= DateTime.Now.AddMonths(6)) return "NearExpiry";
            return "Valid";
        }

            

    }
}
