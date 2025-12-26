using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Brands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BrandServices
{
    public class BrandRepo : IBrandRepo
    {
        private readonly ApplicationDbContext _context;
        public BrandRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(BrandVM model)
        {
            bool name = _context.Brands.Any(b => b.Title.ToLower() == model.Title.ToLower());
            if (name)
            {
                throw new Exception("This Brand name already exists.");
            }
            var brand = new Brand
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Brands.Add(brand);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.Id == id);
            if (brand != null)
            {
                try
                {
                    _context.Brands.Remove(brand);
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

        public Task<PaginatedList<BrandVM>> GetAllAsync(int pageSize, int pageNumber, string? SearchTerm)
        {
            var query = _context.Brands.AsNoTracking()
                .Select(b => new BrandVM
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description
                });
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(b => b.Title.ToLower().Contains(SearchTerm.ToLower()));
            }
            return PaginatedList<BrandVM>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<BrandVM> GetByIdAsync(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null) return null;

            var brandVM = new BrandVM
            {
                Id = brand.Id,
                Title = brand.Title,
                Description = brand.Description,
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt,
                Products = brand.Products.Select(p => new BrandProductVM
                {
                    productId = p.ProductId,
                    productName = p.ProductName
                }).ToList()
            };

            return brandVM;
        }


        public void Update(BrandVM model)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.Id == model.Id);
            if (brand != null)
            {
                brand.Title = model.Title;
                brand.Description = model.Description;
                brand.CreatedAt = model.CreatedAt;
                brand.UpdatedAt = DateTime.Now;
                _context.Brands.Update(brand);
                _context.SaveChanges();
            }
        }


        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.Brands
                .AsNoTracking()
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToListAsync() ?? new List<string>();
        }

    }
}
