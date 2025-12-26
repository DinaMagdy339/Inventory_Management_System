using Inventory.Repository.BankServices;
using Inventory.ViewModel.Banks;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class BankController : Controller
    {
        private readonly IBankRepo _bankRepo;
        private readonly ILogger<BankController> _logger;
        private readonly IWebHostEnvironment _environment;
        public BankController(IBankRepo bankRepo, ILogger<BankController> logger, IWebHostEnvironment environment)
        {
            _bankRepo = bankRepo;
            _logger = logger;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var banks = await _bankRepo.GetAllAsync(pageSize, pageNum, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(banks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching banks");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load banks.";
                return RedirectToAction("Index");
            }
        }
              [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var bank = await _bankRepo.GetByIdAsync(id);
                return View(bank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Bank details for edit");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Bank details for edit.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(BankVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _bankRepo.Update(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Bank");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to update Bank.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(BankVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _bankRepo.Add(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Bank");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to create Bank.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _bankRepo.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Bank");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to delete Bank.";
                return RedirectToAction("Index");
            }
        }
    }
}
