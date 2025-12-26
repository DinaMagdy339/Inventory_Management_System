using Inventory.Repository.BillTypeServices;
using Inventory.ViewModel.Bills;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class BillTypeController : Controller
    {
        private readonly IBillTypeRepo _billTypeRepo;
        private readonly ILogger<BillTypeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public BillTypeController(IBillTypeRepo billTypeRepo, ILogger<BillTypeController> logger, IWebHostEnvironment environment)
        {
            _billTypeRepo = billTypeRepo;
            _logger = logger;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int PageSize = 10, int PageNumber = 1 , string? searchTerm = "")
        {
            try
            {
                var billTypes = await _billTypeRepo.GetAllAsync(PageSize, PageNumber, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(billTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill types");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load bill types.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateBillTypeVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _billTypeRepo.Add(model);
                    TempData["SuccessMessage"] = "Bill Type created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Bill Type");
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var billType = await _billTypeRepo.GetByIdAsync(id);
            return View(billType);
        }
        [HttpPost]
        public IActionResult Edit(BillTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _billTypeRepo.Update(model);
                    TempData["SuccessMessage"] = "Bill Type Updated successfully.";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Updating Bill Type");
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                }
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _billTypeRepo.Delete(id);
                TempData["SuccessMessage"] = "Bill Type deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Bill Type with ID {BillTypeId}", id);

                if (_environment.IsDevelopment())
                    TempData["ErrorMessage"] = "Error deleting Bill Type: " + ex.Message;
                else
                    TempData["ErrorMessage"] = "Error deleting Bill Type. Please check logs for details.";
            }

            return RedirectToAction("Index");
        }


    }
}
