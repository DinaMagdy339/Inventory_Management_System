using Inventory.Models;
using Inventory.Repository.CustomerServices;
using Inventory.Repository.InvoiceServices;
using Inventory.Repository.ProductServices;
using Inventory.Repository.StockServices;
using Inventory.Repository.WarehouseServices;
using Inventory.ViewModel.Products;
using Inventory.ViewModel.StockBatchs;
using Inventory.ViewModel.Stocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System;

namespace Inventory.Web.Controllers
{
    public class StockController : Controller
    {
        
        #region 

        private readonly IStockRepo _stockRepo;
        private readonly ILogger<StockController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ICustomerRepo _customerRepo;
        private readonly IInvoiceRepo _invoiceRepo;
        private readonly IProductRepo _productRepo;
        private readonly IWarehouseRepo _warehouseRepo;
        private const string SESSION_KEY = "SelectedProducts";


        public StockController(
          IStockRepo stockRepo,
          ILogger<StockController> logger,
          IWebHostEnvironment environment,
          ICustomerRepo customerRepo,
          IInvoiceRepo invoiceRepo,
          IProductRepo productRepo,
          IWarehouseRepo warehouseRepo
            )
        {
            _stockRepo = stockRepo;
            _logger = logger;
            _environment = environment;
            _customerRepo = customerRepo;
            _invoiceRepo = invoiceRepo;
            _productRepo = productRepo;
            _warehouseRepo = warehouseRepo;
        }

        #endregion

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index(int PageNumber = 1, int PageSize = 10, string? searchTerm = "")
        {
            try
            {
                var stocks = await _stockRepo
                    .GetAllStocksAsync(PageNumber, PageSize, searchTerm);
                foreach (var item in stocks)
                {
                    if (item.TotalQuantity < 50)
                    {
                        _logger.LogWarning(
                            "Low stock alert for product {ProductName} - Qty: {Qty}",
                            item.ProductName,
                            item.TotalQuantity);
                    }
                }


                ViewBag.SearchTerm = searchTerm;
                return View(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stocks");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load stocks.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var stock = await _stockRepo.GetStockByIdAsync(id);
                if (stock == null)
                    return NotFound();

                return View(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock details");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load stock details.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.product =await _productRepo.GetForDropdownAsync();
            ViewBag.warehouse =await _warehouseRepo.GetForDropdownAsync();

            return View();
        }

        [HttpPost]
        public IActionResult Create(CreatedStockVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _stockRepo.AddStock(model);

                TempData["SuccessMessage"] = "Stock created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock");

                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "Unable to create stock.");
               
                return View(model);
            }
        }

        #endregion

        #region Edit


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var stock = await _stockRepo.GetStockByStockIdAsync(id);
                if (stock == null)
                    return NotFound();
                stock.Batches ??= new List<StockBatchVM>();


                ViewBag.warehouse = await _warehouseRepo.GetForDropdownAsync();

                ViewBag.product = await _productRepo.GetForDropdownAsync();
                return View(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock for edit");

                if (_environment.IsDevelopment())
                    return BadRequest(ex.Message);

                TempData["ErrorMessage"] = "Unable to load stock.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatedStockVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _stockRepo.Update(model);

                TempData["SuccessMessage"] = "Stock updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock");

                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "Unable to update stock.");

                return View(model);
            }
        }

        #endregion

        #region Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _stockRepo.Delete(id);
                TempData["SuccessMessage"] = "Stock deactivated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Withdraw
        [HttpGet]
        public async Task<IActionResult> Withdraw(string productSearch = null, int? productId = null, string customerName = null)
        {
            var model = new WithdrawForInvoiceVM
            {
                ProductSearch = productSearch,
                ProductId = productId,
                CustomerName = customerName,
                Products = !string.IsNullOrEmpty(productSearch) ? await _stockRepo.GetProductsAsync(productSearch) : new List<ProductViewModel>(),
                Batches = productId.HasValue ? await _stockRepo.GetBatchesAsync(productId.Value) : new List<StockBatchVM>(),
                OpenInvoices = !string.IsNullOrWhiteSpace(customerName) ? await _invoiceRepo.GetOpenInvoicesForCustomerAsync(customerName) : new List<Invoice>()
            };

            ViewBag.Customers = await _customerRepo.GetForDropdownAsync() ?? new List<string>();
            model.SelectedProducts = HttpContext.Session.GetObject<List<WithdrawItemVM>>(SESSION_KEY) ?? new List<WithdrawItemVM>();

            return View(model);
        }

        [HttpPost]
        public IActionResult AddToSession(WithdrawItemVM item)
        {
            var selectedProducts = HttpContext.Session.GetObject<List<WithdrawItemVM>>(SESSION_KEY) ?? new List<WithdrawItemVM>();

            var exist = selectedProducts.FirstOrDefault(x => x.ProductId == item.ProductId && x.StockBatchId == item.StockBatchId);
            if (exist != null)
            {
                exist.Quantity = item.Quantity;
                exist.UnitPrice = item.UnitPrice;
                exist.UnitDiscount = item.UnitDiscount;
            }
            else
            {
                selectedProducts.Add(item);
            }

            HttpContext.Session.SetObject(SESSION_KEY, selectedProducts);
            return RedirectToAction("Withdraw");
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawFinal(string customerName, int? invoiceId)
        {
            var selectedProducts = HttpContext.Session.GetObject<List<WithdrawItemVM>>(SESSION_KEY);
            if (selectedProducts == null || !selectedProducts.Any())
            {
                TempData["Error"] = "لا توجد منتجات للسحب.";
                return RedirectToAction("Withdraw");
            }

            var success = await _stockRepo.WithdrawMultipleAsync(selectedProducts, customerName, invoiceId, null);
            if (success)
            {
                HttpContext.Session.Remove(SESSION_KEY);
                TempData["Success"] = "تم السحب بنجاح لكل المنتجات المختارة.";
            }

            return RedirectToAction("Withdraw");
        }

        #endregion   
    }
}