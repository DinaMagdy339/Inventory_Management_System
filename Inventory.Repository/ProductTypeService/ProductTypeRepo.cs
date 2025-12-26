using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.ProductTypeService
{
    public class ProductTypeRepo : IProductTypeRepo
    {
        private readonly ApplicationDbContext _context;
        public ProductTypeRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(ProductTypeViewModel model)
        {
            bool name = _context.ProductTypes.Any(b => b.ProductTypeName.ToLower() == model.ProductTypeName.ToLower());

            if (name)
            {
                throw new Exception("This Product Type name already exists.");
            }

            var productType = new ProductType()
            {
                ProductTypeName = model.ProductTypeName,
                Description = model.Description
            };
            _context.ProductTypes.Add(productType);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var productType = _context.ProductTypes.Find(id);
            if (productType != null)
            {
                try
                {

                    _context.ProductTypes.Remove(productType);
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

        public Task<PaginatedList<ProductTypeViewModel>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _context.ProductTypes.AsNoTracking()
                .Select(pt => new ProductTypeViewModel
                {
                    ProductTypeId = pt.ProductTypeId,
                    ProductTypeName = pt.ProductTypeName,
                    Description = pt.Description
                });
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(pt => pt.ProductTypeName.ToLower().Contains(searchTerm.ToLower()));
            }
            return PaginatedList<ProductTypeViewModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public Task<ProductTypeViewModel> GetById(int id)
        {
            var productType = _context.ProductTypes.AsNoTracking()
                .Where(pt => pt.ProductTypeId == id)
                .Select(pt => new ProductTypeViewModel
                {
                    ProductTypeId = pt.ProductTypeId,
                    ProductTypeName = pt.ProductTypeName,
                    Description = pt.Description
                })
                .FirstOrDefaultAsync();
            return productType;
        }

     
        public void Update(ProductTypeViewModel model)
        {
            var productType = _context.ProductTypes.Find(model.ProductTypeId);
            if (productType != null)
            {
                bool name = _context.ProductTypes.Any(b => b.ProductTypeName.ToLower() == model.ProductTypeName.ToLower()
                                                        && b.ProductTypeId != model.ProductTypeId);
                if (name)
                {
                    throw new Exception("This Product Type name already exists.");
                }
                productType.ProductTypeName = model.ProductTypeName;
                productType.Description = model.Description;
                _context.ProductTypes.Update(productType);
                _context.SaveChanges();
            }
        }
        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.ProductTypes
                .AsNoTracking()
                .OrderBy(pt => pt.ProductTypeName)
                .Select(pt => pt.ProductTypeName)
                .ToListAsync() ?? new List<string>();
        }

    }
}
