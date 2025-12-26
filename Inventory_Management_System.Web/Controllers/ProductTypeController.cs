using Inventory.Repository.ProductServices;
using Inventory.Repository.ProductTypeService;
using Inventory.ViewModel.Products;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class ProductTypeController : Controller
    {
        private readonly IProductTypeRepo _productTypeRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductTypeController> _logger;

        public ProductTypeController(IProductTypeRepo productTypeRepo, IWebHostEnvironment environment, ILogger<ProductTypeController> logger)
        {
            _productTypeRepo = productTypeRepo;
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 10, int pageSize = 1, string? searchTerm = "")
        {
            try
            {
                var ProductType = await _productTypeRepo.GetAllAsync(pageSize, pageNum, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(ProductType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Product Type");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Product Type.";
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productTypeRepo.Create(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Product Type");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to create Product Type.");

                    return View(model);
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var ProductType = await _productTypeRepo.GetById(id);
            return View(ProductType);
        }
        [HttpPost]
        public IActionResult Edit(ProductTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productTypeRepo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Product Type");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update Product Type.");

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
                _productTypeRepo.Delete(id);
                TempData["SuccessMessage"] = "Product Type deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Product Type with ID {Id}", id);

                TempData["ErrorMessage"] = _environment.IsDevelopment()
                    ? ex.Message
                    : "Unable to Product Type product.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}