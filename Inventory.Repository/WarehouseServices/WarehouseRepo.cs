using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.WarehouseServices
{
    public class WarehouseRepo : IWarehouseRepo
    {
        private readonly ApplicationDbContext _context;
        public WarehouseRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(CreatedWarehouseVM warehouseVM)
        {
            bool isExist = _context.Warehouses.Any(w => w.WarehouseName == warehouseVM.WarehouseName && w.BranchId == warehouseVM.BranchId);
            if (!isExist)
            {
                var branchId = _context.Branches.Where(b=>b.BranchName.ToLower() == warehouseVM.BranchName.ToLower()).Select(b => b.BranchId).FirstOrDefault();

                var warehouse = new Warehouse
                {
                    WarehouseName = warehouseVM.WarehouseName,
                    Description = warehouseVM.Description,
                    BranchId = branchId
                };
                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
            }
        }

       
        public void Delete(int warehouseId)
        {
            var warehouse = _context.Warehouses.Find(warehouseId);
            if (warehouse != null)
            {
                try 
                {
                    _context.Warehouses.Remove(warehouse);
                    _context.SaveChanges();
                }
                catch (Exception ex)
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

        public Task<PaginatedList<AllWarehouseVM>> GetAllWarehousesAsync(int pageNum, int oageSize, string? SearchTerm)
        {
            var warehouses = _context.Warehouses
                .Include(w => w.Branch)
                .Select(w => new AllWarehouseVM
                {
                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.WarehouseName,
                    BranchName = w.Branch.BranchName,
                }).AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                warehouses = warehouses.Where(w => w.WarehouseName.Contains(SearchTerm) || w.BranchName.Contains(SearchTerm));
            }
            return PaginatedList<AllWarehouseVM>.CreateAsync(warehouses, pageNum, oageSize);
        }

     
        public Task<WarehouseVM> GetWarehouseByIdAsync(int warehouseId)
        {
            var warehouse = _context.Warehouses
                .Where(w => w.WarehouseId == warehouseId)
                .Select(w => new WarehouseVM
                {
                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.WarehouseName,
                    Description = w.Description,
                    BranchName=w.Branch.BranchName,
                    Stocks = w.Stocks.Select(s => new StocksWarehouseVM
                    {
                        StockId = s.StockId,
                        ProductName = s.Product.ProductName,
                        Quantity = s.Quantity
                    }).ToList()
                }).FirstOrDefaultAsync();
            return warehouse;
        }

        public void Update(CreatedWarehouseVM warehouseVM)
        {
            var warehouse = _context.Warehouses.Find(warehouseVM.WarehouseId);
            var branchId = _context.Branches.Where(b => b.BranchName.ToLower() == warehouseVM.BranchName.ToLower()).Select(b => b.BranchId).FirstOrDefault();

            if (warehouse != null)
            {
                warehouse.WarehouseName = warehouseVM.WarehouseName;
                warehouse.Description = warehouseVM.Description;
                warehouse.BranchId = branchId;
                
                _context.SaveChanges();
            }
        }

        public Task<List<string>> GetForDropdownAsync()
        {
            return _context.Warehouses
                .Select(w => w.WarehouseName)
                .OrderBy(name => name)
                .ToListAsync();
        }

    }
}
