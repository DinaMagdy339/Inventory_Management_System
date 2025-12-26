using Inventory.Repository.CustomerServices;
using Inventory.Repository.CustomerTypeServices;
using Inventory.ViewModel.Customers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly ICustomerTypeRepo _customerTypeRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(ICustomerRepo customerRepo, ICustomerTypeRepo customerTypeRepo, IWebHostEnvironment environment, ILogger<CustomerController> logger)
        {
            _customerRepo = customerRepo;
            _customerTypeRepo = customerTypeRepo;
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string? searchTerm = "")
        {
            try
            {
                var customer = await _customerRepo.GetAllAsync(pageSize, pageNum, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load customers.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customer = await _customerRepo.GetByIdAsync(id);
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Customer details");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Customer details.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var customer = await _customerRepo.GetByIdAsync(id);
                var types = await _customerTypeRepo.GetForDropdownAsync();
                ViewBag.CustomerTypes = types;
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Customer details");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load Customer details.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _customerRepo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Customer");
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update Customer.");
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var types = await _customerTypeRepo.GetForDropdownAsync();
            ViewBag.CustomerTypes = types;
            return View(new CustomerViewModel());
        }        
        [HttpPost]
        public async Task <IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _customerRepo.Create(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Customer");
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to create Customer.");
                }
            }
          
            return View(model);


        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _customerRepo.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Customer");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to delete Customer.";
                return RedirectToAction("Index");
            }
        }
    }
}
