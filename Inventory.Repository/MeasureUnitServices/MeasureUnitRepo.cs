using Inventory.Helper.Paging;
using Inventory.ViewModel.MeasureUnits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.MeasureUnitServices
{
    public class MeasureUnitRepo : IMeasureUnitRepo
    {
        private readonly ApplicationDbContext _context;
        public MeasureUnitRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(MeasureUnitVM model)
        {
            bool name = _context.MeasureUnits.Any(m => m.Name.ToLower() == model.Name.ToLower());
            if (name)
            {
                throw new Exception("This Measure Unit name already exists.");
            }
            var measureUnit = new Models.MeasureUnit
            {
                Name = model.Name,
                Description = model.Description,
            };
            _context.MeasureUnits.Add(measureUnit);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var measureUnit = _context.MeasureUnits.FirstOrDefault(m => m.Id == id);
            if (measureUnit != null)
            {
                try
                {
                    _context.MeasureUnits.Remove(measureUnit);
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

        public Task<PaginatedList<MeasureUnitVM>> GetAllAsync(int pageSize, int pageNumber, string? SearchTerm)
        {
            var query = _context.MeasureUnits.AsNoTracking()
                .Select(m => new MeasureUnitVM
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                });
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(m => m.Name.ToLower().Contains(SearchTerm.ToLower()));
            }
            return PaginatedList<MeasureUnitVM>.CreateAsync(query, pageNumber, pageSize);
        }

        public Task<MeasureUnitVM> GetByIdAsync(int id)
        {
            var measureUnit = _context.MeasureUnits.AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MeasureUnitVM
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                })
                .FirstOrDefaultAsync();
            return measureUnit;
        }

        public void Update(MeasureUnitVM model)
        {
            var measureUnit = _context.MeasureUnits.FirstOrDefault(m => m.Id == model.Id);
            if (measureUnit != null)
            {
                measureUnit.Name = model.Name;
                measureUnit.Description = model.Description;
                _context.MeasureUnits.Update(measureUnit);
                _context.SaveChanges();
            }
        }
        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.MeasureUnits
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => m.Name)
                .ToListAsync() ?? new List<string>();
        }

    }
}
