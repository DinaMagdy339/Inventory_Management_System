using Inventory.Repository;
using Inventory.Repository.BranchServices;
using Inventory.ViewModel.Branchs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Web.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchRepo _branchRepo;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BranchController> _logger;
        private readonly IWebHostEnvironment _environment;
        public BranchController(IBranchRepo branchRepo, ApplicationDbContext context , ILogger<BranchController> logger, IWebHostEnvironment environment)
        {
            _branchRepo = branchRepo;
            _context = context;
            _logger = logger;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int PageNumber = 1, int PageSize = 10, string? searchTerm = "")
        {
            var branchs = await _branchRepo.GetAllAsync(PageSize, PageNumber, searchTerm);
            try
            {


                ViewBag.SearchTerm = searchTerm;
                return View(branchs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching branchs");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load branchs.";
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Currencies = await _context.currencies
                .Select(c => c.Name)
                .ToListAsync() ?? new List<string>();
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateBranchViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _branchRepo.Add(model);
                    TempData["SuccessMessage"] = "Branch created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating branch");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to create branch.");

                    return View(model);
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var branch = await _branchRepo.GetByIdAsync(id);
                return View(branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching branch details");
                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);
                TempData["ErrorMessage"] = "Unable to load branch details.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            ViewBag.Currencies = await _context.currencies
                  .Select(c => c.Name)
                  .ToListAsync() ?? new List<string>();

            if (!id.HasValue) return BadRequest();

            var branch = await _branchRepo.GetByIdAsync(id.Value);
            if (branch == null) return NotFound();
            return View(branch);
        }
        [HttpPost]
        public IActionResult Edit(BranchViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _branchRepo.Update(model);
                    TempData["SuccessMessage"] = "Branch updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating branch");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update branch.");

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
                _branchRepo.Delete(id);
                TempData["SuccessMessage"] = "Branch deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred";
            }

            return RedirectToAction(nameof(Index));
        }
    }


}
