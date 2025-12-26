using Inventory.Repository.CustomerTypeServices;
using Inventory.ViewModel.Customers;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class CustomerTypeController : Controller
    {
        private readonly ILogger<CustomerTypeController> _logger;
        private ICustomerTypeRepo _customerTypeRepo;
        public CustomerTypeController(ILogger<CustomerTypeController> logger , ICustomerTypeRepo customerTypeRepo)
        {
            _logger = logger;
            _customerTypeRepo = customerTypeRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index( int pageNumber = 1 , int pageSize = 10 , string SearchTerm = "")
        {
            var customerTypes =await _customerTypeRepo.GetAllAsync(pageSize, pageNumber, SearchTerm);
            ViewBag.SearchTerm = SearchTerm;
            return View(customerTypes);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CustomerTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _customerTypeRepo.Add(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CustomerTypeName", ex.Message);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customerType =await _customerTypeRepo.GetByIdAsync(id);
            return View(customerType);
        }
        [HttpPost]
        public IActionResult Edit(CustomerTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _customerTypeRepo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CustomerTypeName", ex.Message);
                }
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _customerTypeRepo.Delete(id);
                TempData["SuccessMessage"] = "Customer Type deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Customer Type with ID {CustomerTypeId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the Customer Type.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
