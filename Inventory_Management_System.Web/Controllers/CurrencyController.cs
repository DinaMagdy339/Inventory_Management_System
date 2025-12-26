using Inventory.Repository.CurrencyServices;
using Inventory.ViewModel.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyRepo _currencyRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BillController> _logger;

        public CurrencyController(ICurrencyRepo currencyRepo, IWebHostEnvironment environment, ILogger<BillController> logger)
        {
            _currencyRepo = currencyRepo;
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string? searchTerm = "")
        {
            try
            {
                var currencies = await _currencyRepo.GetAllCurrencies(pageNum, pageSize, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(currencies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currencies");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load currencies.";
                return RedirectToAction("Index");
            }
           
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CurrencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _currencyRepo.Add(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Currency");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to create Currency.");

                    return View(model);
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var currency =await _currencyRepo.GetCurrencyById(id);
            return View(currency);
        }
        [HttpPost]
        public IActionResult Edit(CurrencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _currencyRepo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Currency");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update Currency.");

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
                _currencyRepo.Delete(id);
                TempData["SuccessMessage"] = "Currency deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Currency with ID {currencyId}", id);

                TempData["ErrorMessage"] = _environment.IsDevelopment()
                    ? ex.Message
                    : "Unable to delete Currency.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}