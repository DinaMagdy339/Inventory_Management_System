using Inventory.Repository.CurrencyServices;
using Inventory.Repository.MeasureUnitServices;
using Inventory.ViewModel.Currencies;
using Inventory.ViewModel.MeasureUnits;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class MeasureUnitController : Controller
    {
        private readonly IMeasureUnitRepo _measureUnitRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MeasureUnitController> _logger;

        public MeasureUnitController(IMeasureUnitRepo measureUnitRepo, IWebHostEnvironment environment, ILogger<MeasureUnitController> logger)
        {
            _measureUnitRepo = measureUnitRepo;
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string? searchTerm = "")
        {
            try
            {
                var measureUnit = await _measureUnitRepo.GetAllAsync(pageSize, pageNum, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(measureUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching MeasureUnit");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load MeasureUnit.";
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(MeasureUnitVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _measureUnitRepo.Add(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating MeasureUnit");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to create MeasureUnit.");

                    return View(model);
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var measureUnit = await _measureUnitRepo.GetByIdAsync(id);
            return View(measureUnit);
        }
        [HttpPost]
        public IActionResult Edit(MeasureUnitVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _measureUnitRepo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating MeasureUnit");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update MeasureUnit.");

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
                _measureUnitRepo.Delete(id);
                TempData["SuccessMessage"] = "MeasureUnit deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting MeasureUnit with ID {Id}", id);

                TempData["ErrorMessage"] = _environment.IsDevelopment()
                    ? ex.Message
                    : "Unable to delete MeasureUnit.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

