using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Branchs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BranchServices
{
    public class BranchRepo  : IBranchRepo
    {
        private ApplicationDbContext _context;
        public BranchRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<AllBranchViewModel>> GetAllAsync(int pageSize, int pageNumber, string? searchTerm)
        {
            var branchs = _context.Branches.AsNoTracking()
                .Include(b => b.Currency)
                .Select(b => new AllBranchViewModel
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName,
                    Description = b.Description,
                    Currency = b.Currency != null ? b.Currency.Name : "",
                    City = b.City,
                  
                });
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();
                branchs = branchs.Where(x => x.City.ToLower().Contains(lowerTerm));
            }
            return await PaginatedList<AllBranchViewModel>.CreateAsync(branchs, pageNumber, pageSize);
        }
        public void Add(CreateBranchViewModel model)
        {
            var currencyId = _context.currencies
                .Where(c => c.Name.ToLower() == model.Currency.ToLower())
                .Select(c => c.Id)
                .FirstOrDefault();

            if (currencyId == 0) throw new Exception($"BillType '{model.Currency}' not found.");
            bool name = _context.Branches.Any(b => b.BranchName.ToLower() == model.BranchName.ToLower());
            if (name)
            {
                throw new Exception("This Branch name already exists.");
            }
            var branch = new Branch
            {
                BranchName = model.BranchName,
                Description = model.Description,
                CurrencyId = currencyId,
                Address = model.Address,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                phone = model.phone,
                Email = model.Email,
                ContactPerson = model.ContactPerson
            };
            _context.Branches.Add(branch);
            _context.SaveChanges();
        }
        public async Task<BranchViewModel> GetByIdAsync(int? id)
        {
            var branch = await _context.Branches
                .Include(b => b.Currency)
                .Include(b=>b.Warehouses)
                .FirstOrDefaultAsync(b => b.BranchId == id.Value);
            var model = new BranchViewModel
            {
                BranchId= branch.BranchId,
                BranchName = branch.BranchName,
                Description = branch.Description,
                Currency = branch.Currency != null ? branch.Currency.Name : "",
                Address = branch.Address,
                City = branch.City,
                State = branch.State,
                ZipCode = branch.ZipCode,
                phone = branch.phone,
                Email = branch.Email,
                ContactPerson = branch.ContactPerson,
                Warehouses = branch.Warehouses.Select(w => new BranchWarehouseVM
                {
                    warehouseId = w.WarehouseId,
                    warehouseName = w.WarehouseName,
                }).ToList()
            };
            return model;
        }
        public void Update(BranchViewModel model)
        {
            var branch = _context.Branches.FirstOrDefault(b => b.BranchId == model.BranchId);

            var currencyId = _context.currencies
                  .Where(c => c.Name.ToLower() == model.Currency.ToLower())
                  .Select(c => c.Id)
                  .FirstOrDefault();

           
            if (branch != null)
            {
                branch.BranchName = model.BranchName;
                branch.Description = model.Description;
                branch.CurrencyId = currencyId;
                branch.Address = model.Address;
                branch.City = model.City;
                branch.State = model.State;
                branch.ZipCode = model.ZipCode;
                branch.phone = model.phone;
                branch.Email = model.Email;
                _context.SaveChanges();
            }
        }
        public void Delete(int branchId)
        {
            var branch = _context.Branches.FirstOrDefault(b => b.BranchId == branchId);
            if (branch != null)
            {
                try
                {
                    _context.Branches.Remove(branch);
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

        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.Branches
                .AsNoTracking()
                .OrderBy(b => b.BranchName)
                .Select(b => b.BranchName)
                .ToListAsync() ?? new List<string>();
        }
    }

}
