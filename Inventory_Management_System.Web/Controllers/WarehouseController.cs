using Inventory.Repository.BranchServices;
using Inventory.Repository.WarehouseServices;
using Inventory.ViewModel.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly ILogger<WarehouseController> _logger;
        private readonly IWarehouseRepo _warehouseRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly IBranchRepo _branchRepo;

        public WarehouseController(ILogger<WarehouseController> logger, IWarehouseRepo warehouseRepo, IWebHostEnvironment environment, IBranchRepo branchRepo)
        {
            _logger = logger;
            _warehouseRepo = warehouseRepo;
            _environment = environment;
            _branchRepo = branchRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNum = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var warehouses = await _warehouseRepo.GetAllWarehousesAsync(pageNum, pageSize, searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching warehouses");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load warehouses.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                var warehouse = _warehouseRepo.GetWarehouseByIdAsync(id).Result;
                if (warehouse == null)
                {
                    return NotFound();
                }
                return View(warehouse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching warehouse details");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load warehouse details.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task <IActionResult> Create()
        {
            var branches =await _branchRepo.GetForDropdownAsync();
            ViewBag.Branches = branches;
            return View(new CreatedWarehouseVM());
        }
        [HttpPost]
        public IActionResult Create(CreatedWarehouseVM warehouseVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _warehouseRepo.Create(warehouseVM);
                    TempData["SuccessMessage"] = "Warehouse created successfully.";
                    return RedirectToAction("Index");
                }
                return View(warehouseVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to create warehouse.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task <IActionResult> Edit(int id)
        {
            try
            {
                var warehouse =await _warehouseRepo.GetWarehouseByIdAsync(id);
                if (warehouse == null)
                {
                    return NotFound();
                }
                var model = new CreatedWarehouseVM
                {
                    WarehouseId = warehouse.WarehouseId,
                    WarehouseName = warehouse.WarehouseName,
                    Description = warehouse.Description,
                    BranchName = warehouse.BranchName
                };

                var branches =await _branchRepo.GetForDropdownAsync();
                ViewBag.Branches = branches;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching warehouse for edit");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load warehouse for editing.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(CreatedWarehouseVM warehouseVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _warehouseRepo.Update(warehouseVM);
                    TempData["SuccessMessage"] = "Warehouse updated successfully.";
                    return RedirectToAction("Index");
                }
                return View(warehouseVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating warehouse");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to update warehouse.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _warehouseRepo.Delete(id);
                TempData["SuccessMessage"] = "Warehouse deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting warehouse");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to delete warehouse.";
                return RedirectToAction("Index");
            }
        }
    }
}
