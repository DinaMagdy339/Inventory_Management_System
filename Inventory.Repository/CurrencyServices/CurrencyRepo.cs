using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Currencies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.CurrencyServices
{
    public class CurrencyRepo : ICurrencyRepo
    {
        private readonly ApplicationDbContext _context;
        public CurrencyRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(CurrencyViewModel model)
        {
            bool code = _context.currencies.Any(c=> c.Code.ToLower() == model.Code.ToLower());
            if (code)
            {
                throw new Exception("This Currency code already exists.");
            }
            var currency = new Currency
            {
                Code = model.Code,
                Name = model.Name,
                Description = model.Description
            };
            _context.currencies.Add(currency);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var currency = _context.currencies.Find(id);

            if (currency == null)
                return;

            try
            {
                _context.currencies.Remove(currency);
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

        public async Task<PaginatedList<CurrencyViewModel>> GetAllCurrencies(int pageNum, int pageSize, string? searchTerm)
        {
            var currencies = _context.currencies.AsNoTracking()
                .Select(c => new CurrencyViewModel
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Description = c.Description
                });
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();
                currencies = currencies.Where(S => S.Name.ToLower().Contains(lowerTerm) || S.Code.ToLower().Contains(lowerTerm));
            }
            return await PaginatedList<CurrencyViewModel>.CreateAsync(currencies, pageNum, pageSize);

        }

        public async Task<CurrencyViewModel> GetCurrencyById(int id)
        {
            var Currency = await _context.currencies.FirstOrDefaultAsync(c => c.Id == id);
            var VM = new CurrencyViewModel
            {
                Id = Currency.Id,
                Code = Currency.Code,
                Name = Currency.Name,
                Description = Currency.Description
            };
            return VM;
        }

       
        public void Update(CurrencyViewModel model)
        {
            var currency = _context.currencies.FirstOrDefault(c => c.Id == model.Id);
            if (currency != null)
            {
                currency.Code = model.Code;
                currency.Name = model.Name;
                currency.Description = model.Description;
                _context.SaveChanges();
            }
        }

        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.currencies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync() ?? new List<string>();
        }

    }
}
