using Inventory.Repository.BranchServices;
using Inventory.Repository.BrandServices;
using Inventory.Repository.CurrencyServices;
using Inventory.Repository.MeasureUnitServices;
using Inventory.Repository.ProductServices;
using Inventory.Repository.ProductTypeService;
using Inventory.ViewModel.MeasureUnits;
using Inventory.ViewModel.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Web.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductRepo _productRepo;
        private readonly IMeasureUnitRepo _m;
        private readonly IBrandRepo _br;
        private readonly ICurrencyRepo _c;
        private readonly IProductTypeRepo _pt;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepo productRepo, IBrandRepo br, IMeasureUnitRepo m, ICurrencyRepo c, IProductTypeRepo pt,
            IWebHostEnvironment environment, 
            ILogger<ProductController> logger)
        {
            _m = m;
            _pt = pt;
            _br = br;
            _c = c;
            _productRepo = productRepo;
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string? searchTerm = "")
        {
            try
            {
                var product = await _productRepo.GetAllAsync(pageNum, pageSize, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load product.";
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _productRepo.GetByIdAsync(id).Result;
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Product details");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Product details.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var mm = await _m.GetForDropdownAsync();
            ViewBag.MeasureUnits = mm;
            var brr = await _br.GetForDropdownAsync();
            ViewBag.Brands = brr;
            var cc = await _c.GetForDropdownAsync();
            ViewBag.Currencys = cc;
            var ptt = await _pt.GetForDropdownAsync();
            ViewBag.ProductTypes = ptt;

            return View(new ProductViewModel());
        }

        [HttpPost]
        public IActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productRepo.Add(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating product");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to create product.");
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var m = await _m.GetForDropdownAsync();
            ViewBag.MeasureUnit = m;
            var br = await _br.GetForDropdownAsync();
            ViewBag.Brand = br;
            var c = await _c.GetForDropdownAsync();
            ViewBag.Currency = c;
            var pt = await _pt.GetForDropdownAsync();
            ViewBag.ProductType = pt;


            var product = await _productRepo.GetByIdAsync(id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productRepo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update product.");

                    return View(model);
                }

            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _productRepo.Delete(id);
                TempData["SuccessMessage"] = "product deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {Id}", id);

                TempData["ErrorMessage"] = _environment.IsDevelopment()
                    ? ex.Message
                    : "Unable to delete product.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

