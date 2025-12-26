using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Banks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BankServices
{
    public class BankRepo : IBankRepo
    {
        private readonly ApplicationDbContext _context;
        public BankRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(BankVM model)
        {
            bool name = _context.Banks.Any(b => b.Name.ToLower() == model.Name.ToLower());
            if (name)
            {
                throw new Exception("This Bank name already exists.");
            }
            var bank = new Bank
            {
                Name = model.Name,
                Description = model.Description
            };
            _context.Banks.Add(bank);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var bank = _context.Banks.FirstOrDefault(b => b.Id == id);
            if (bank != null)
            {
                try
                {
                    _context.Banks.Remove(bank);
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

        public Task<PaginatedList<BankVM>> GetAllAsync(int pageSize, int pageNumber, string? SearchTerm)
        {
           
            var Banks = _context.Banks.AsNoTracking()
                .Select(B => new BankVM
                {
                    Id = B.Id,
                    Name = B.Name,
                    Description = B.Description,
                });
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Banks = Banks.Where(b => b.Name.ToLower().Contains(SearchTerm.ToLower()));
            }
                return PaginatedList<BankVM>.CreateAsync(Banks, pageNumber, pageSize);

        }

        public Task<BankVM> GetByIdAsync(int id)
        {
            var bank = _context.Banks.AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BankVM
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                }).FirstOrDefaultAsync();
            return bank;
        }

        public void Update(BankVM model)
        {
            var bank = _context.Banks.Find(model.Id);
            if (bank != null)
            {
                bank.Name = model.Name;
                bank.Description = model.Description;
                _context.SaveChanges();
            }

        }



        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.Banks
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(b => b.Name)
                .ToListAsync() ?? new List<string>();
        }

    }
}
