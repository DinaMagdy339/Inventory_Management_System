using Inventory.Repository;
using Inventory.Repository.BillServices;
using Inventory.ViewModel.Bills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Web.Controllers
{
    public class BillController : Controller
    {
        private readonly IBillRepo _billRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BillController> _logger;
        private readonly ApplicationDbContext _context;

        public BillController(IBillRepo billRepo, IWebHostEnvironment environment, ILogger<BillController> logger, ApplicationDbContext dbContext)
        {
            _billRepo = billRepo;
            _environment = environment;
            _logger = logger;
            _context = dbContext;
        }

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var bills = await _billRepo.GetAllAsync(pageNumber, pageSize);
                return View(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bills");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load bills.";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region GetAllFiltered
        [HttpGet]
        public async Task<IActionResult> GetAllFiltered(BillFilterationViewModel billFilterationViewModel, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var bills = await _billRepo.GetAllFilteredAsync(billFilterationViewModel, pageNumber, pageSize);

              
                ViewBag.Vendors = await _context.Vendors
                    .Select(v => v.VendorName)
                    .ToListAsync();
                ViewBag.Bills = await _context.Bills
                   .Select(v => v.BillName)
                   .ToListAsync();
                ViewBag.BillTypes = await _context.BillTypes
                    .Select(bt => bt.BillTypeName)
                    .ToListAsync();
                ViewBag.PaymentStatuses = new List<string> { "Paid", "Unpaid", "Partially Paid" };
                ViewBag.ReceivedNotes = await _context.ReceivedNotes
                  .Select(v => v.Name)
                  .ToListAsync();
                var model = new BillIndexViewModel
                {
                    Filter = billFilterationViewModel,
                    Bills = bills
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching filtered bills");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load filtered bills.";
                return RedirectToAction(nameof(Index));
            }

        }
        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            try
            {
                var bill = await _billRepo.GetByIdAsync(id.Value);
                if (bill == null)
                {
                    return NotFound("Bill not found.");
                }
                return View(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill details with ID {BillId}", id);

                if (_environment.IsDevelopment())
                    throw;

                TempData["ErrorMessage"] = "Unable to load bill details.";
                return RedirectToAction(nameof(Index));
            }

        }
        #endregion

        #region Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.BillTypes = await _context.BillTypes
                .Select(bt => bt.BillTypeName)
                .ToListAsync() ?? new List<string>();

            ViewBag.Vendors = await _context.Vendors
                .Select(v => v.VendorName)
                .ToListAsync() ?? new List<string>();

            ViewBag.ReceivedNotes = await _context.ReceivedNotes
              .Select(v => v.Name)
              .ToListAsync() ?? new List<string>();

            return View();
        }

        [HttpPost]
        public IActionResult Create(BillViewModel model)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                _billRepo.Add(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");

                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "Unable to create bill.");

                return View(model);
            }

        }
        #endregion

        #region Edit
        [Authorize(Roles = "Bill" )]
        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.BillTypes = await _context.BillTypes
                .Select(bt => new { bt.BillTypeId, bt.BillTypeName })
                .ToListAsync();

            ViewBag.Vendors = await _context.Vendors
                .Select(v => new { v.VendorId, v.VendorName })
                .ToListAsync();

            ViewBag.ReceivedNotes = await _context.ReceivedNotes
                .Select(g => new { g.Id, g.Name })
                .ToListAsync();

            if (!id.HasValue) return BadRequest();

            try
            {
                var bill = await _billRepo.GetByIdAsync(id.Value);
                return View(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill for edit with ID {BillId}", id);

                if (_environment.IsDevelopment())
                    throw;

                TempData["ErrorMessage"] = "Unable to load bill for editing.";
                return RedirectToAction(nameof(Index));
            }

        }
        [Authorize(Roles = "Bill")]  

        [HttpPost]
        public IActionResult Edit(BillViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _billRepo.Update(model);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating bill");

                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        ModelState.AddModelError(string.Empty, "Unable to update bill.");

                    return View(model);
                }

            }
            return View(model);
        }
        #endregion

        #region Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _billRepo.Delete(id);
                TempData["SuccessMessage"] = "Bill deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill with ID {BillId}", id);

                TempData["ErrorMessage"] = _environment.IsDevelopment()
                    ? ex.Message
                    : "Unable to delete bill.";
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
