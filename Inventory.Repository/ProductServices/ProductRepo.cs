using Inventory.Helper.Paging;
using Inventory.ViewModel.Products;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Inventory.ViewModel.Brands;
using Inventory.Repository.AttachementServices;
namespace Inventory.Repository.ProductServices
{
    public class ProductRepo : IProductRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IAttachementServices _attachementServices;

        public ProductRepo(ApplicationDbContext context, IAttachementServices attachementServices)
        {
            _context = context;

            _attachementServices = attachementServices;
        }
        public void Add(ProductViewModel model)
        {
            bool name = _context.Products.Any(b => b.ProductName.ToLower() == model.ProductName.ToLower());
            if (name)
            {
                throw new Exception("This Product name already exists.");
            }
            var MeasurementUnitId = _context.MeasureUnits
              .Where(bt => bt.Name.ToLower() == model.MeasurementUnit.ToLower())
              .Select(bt => bt.Id)
              .FirstOrDefault();
            if (MeasurementUnitId == 0) throw new Exception($"MeasurementUnit '{model.MeasurementUnit}' not found.");

        
            var CurrencyId = _context.currencies
             .Where(bt => bt.Name.ToLower() == model.Currency.ToLower())
             .Select(bt => bt.Id)
             .FirstOrDefault();
            if (CurrencyId == 0) throw new Exception($"Currency '{model.Currency}' not found.");

            var ProductTypeId = _context.ProductTypes
            .Where(bt => bt.ProductTypeName.ToLower() == model.ProductType.ToLower())
            .Select(bt => bt.ProductTypeId)
            .FirstOrDefault();
            if (ProductTypeId == 0) throw new Exception($"ProductType '{model.ProductType}' not found.");

            var BrandId = _context.Brands
           .Where(bt => bt.Title.ToLower() == model.Brand.ToLower())
           .Select(bt => bt.Id)
           .FirstOrDefault();
            if (BrandId == 0) throw new Exception($"Brand '{model.Brand}' not found.");

            string imageName = null;

            if (model.ProductImageFile != null)
            {
                var uploadResult = _attachementServices
                    .UploadAttachement(model.ProductImageFile, "Images");

                if (!uploadResult.IsSuccess)
                {
                    throw new Exception(uploadResult.ErrorMessage);
                }

                imageName = uploadResult.FileName;
            }


            var product = new Product()
            {
                ProductName = model.ProductName,
                ProductCode = model.ProductCode,
                Barcode = model.Barcode,
                ProductDescription = model.ProductDescription,
                ProductImageUrl = imageName,
                BuyingPrice = model.BuyingPrice,
                SellingPrice = model.SellingPrice,
                MeasurementUnitId = MeasurementUnitId,
                CurrencyId = CurrencyId,
                ProductTypeId = ProductTypeId,
                BrandId = BrandId,

            };

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(b => b.ProductId == id);
            if (product != null)
            {
                try
                {
                    _context.Products.Remove(product);
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

        public Task<PaginatedList<AllProductsVM>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
        {

            var query = _context.Products.AsNoTracking()
                .Select(p => new AllProductsVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    Barcode = p.Barcode,

                })
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.ToLower().Contains(searchTerm.ToLower()));
            }
            return PaginatedList<AllProductsVM>.CreateAsync(query, pageNumber, pageSize);
        }

        public Task<ProductViewModel> GetByIdAsync(int id)
        {
            var product = _context.Products
                .AsNoTracking()
                .Include(p => p.MeasurementUnit)
                .Include(p => p.Currency)
                .Include(p => p.ProductType)
                .Include(p => p.Brand)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    Barcode = p.Barcode,
                    ProductDescription = p.ProductDescription,
                    ProductImageUrl = p.ProductImageUrl,
                    BuyingPrice = p.BuyingPrice,
                    SellingPrice = p.SellingPrice,
                    MeasurementUnit = p.MeasurementUnit.Name,
                    Currency = p.Currency.Name,
                    ProductType = p.ProductType.ProductTypeName,
                    Brand = p.Brand.Title,
                })
                .FirstOrDefaultAsync();
            return product;
        }

        public void Update(ProductViewModel model)
        {
            var product = _context.Products.FirstOrDefault(b => b.ProductId == model.ProductId);
            if (product != null)
            {
                var MeasurementUnitId = _context.MeasureUnits
                   .Where(bt => bt.Name.ToLower() == model.MeasurementUnit.ToLower())
                   .Select(bt => bt.Id)
                   .FirstOrDefault();

               
                var CurrencyId = _context.currencies
                   .Where(bt => bt.Name.ToLower() == model.Currency.ToLower())
                   .Select(bt => bt.Id)
                   .FirstOrDefault();

                var ProductTypeId = _context.ProductTypes
                   .Where(bt => bt.ProductTypeName.ToLower() == model.ProductType.ToLower())
                   .Select(bt => bt.ProductTypeId)
                   .FirstOrDefault();

                var BrandId = _context.Brands
                   .Where(bt => bt.Title.ToLower() == model.Brand.ToLower())
                   .Select(bt => bt.Id)
                   .FirstOrDefault();


                product.ProductName = model.ProductName;
                product.ProductCode = model.ProductCode;
                product.Barcode = model.Barcode;
                product.ProductDescription = model.ProductDescription;
                product.BuyingPrice = model.BuyingPrice;
                product.SellingPrice = model.SellingPrice;
                product.MeasurementUnitId = MeasurementUnitId;
                product.CurrencyId = CurrencyId;
                product.ProductTypeId = ProductTypeId;
                product.BrandId = BrandId;
                if (model.ProductImageFile != null)
                {
                    if (!string.IsNullOrEmpty(product.ProductImageUrl))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Images", product.ProductImageUrl);
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }

                    var result = _attachementServices.UploadAttachement(model.ProductImageFile, "Images");
                    if (!result.IsSuccess)
                        throw new Exception(result.ErrorMessage);

                    product.ProductImageUrl = result.FileName;
                }


                _context.Products.Update(product);

                _context.SaveChanges();
            }


        }

        public async Task<List<string?>> GetForDropdownAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.ProductName)
                .Select(p => p.ProductName)
                .ToListAsync() ;
        }
    }
}