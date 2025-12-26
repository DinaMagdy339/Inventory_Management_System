using Inventory.Repository.BrandServices;
using Inventory.ViewModel.Brands;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandRepo _brandRepo;
        private readonly ILogger<BrandController> _logger;
        private readonly IWebHostEnvironment _environment;
        public BrandController(IBrandRepo brandRepo, ILogger<BrandController> logger, IWebHostEnvironment environment)
        {
            _brandRepo = brandRepo;
            _logger = logger;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var brands = await _brandRepo.GetAllAsync(pageSize, pageNum, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching brands");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load brands.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var brand = await _brandRepo.GetByIdAsync(id);
                return View(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Brand details");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Brand details.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var brand = await _brandRepo.GetByIdAsync(id);
                return View(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Brand for edit");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Brand for editing.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(BrandVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _brandRepo.Update(model);
                    TempData["SuccessMessage"] = "Brand updated successfully.";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Brand");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to update Brand.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(BrandVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _brandRepo.Add(model);
                    TempData["SuccessMessage"] = "Brand created successfully.";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Brand");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to create Brand.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _brandRepo.Delete(id);
                TempData["SuccessMessage"] = "Brand deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Brand");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to delete Brand.";
                return RedirectToAction("Index");
            }
        }
    }
}
