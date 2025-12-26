using Inventory.Models;
using Inventory.Helper.Paging;
using Inventory.ViewModel.Bills;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.ViewModel.PaymentVouchers;

namespace Inventory.Repository.BillServices
{
    public class BillRepo : IBillRepo
    {
        private readonly ApplicationDbContext _context;

        public BillRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(BillViewModel BillVM)
        {
            var billTypeId = _context.BillTypes
                .Where(bt => bt.BillTypeName.ToLower() == BillVM.BillTypeName.ToLower())
                .Select(bt => bt.BillTypeId)
                .FirstOrDefault();

            var vendorId = _context.Vendors
                .Where(v => v.VendorName.ToLower() == BillVM.VendorName.ToLower())
                .Select(v => v.VendorId)
                .FirstOrDefault();

            var grnId = _context.ReceivedNotes
                .Where(g => g.Name.ToLower() == BillVM.GRNNumber.ToLower())
                .Select(g => g.Id)
                .FirstOrDefault();

            if (billTypeId == 0) throw new Exception($"BillType '{BillVM.BillTypeName}' not found.");
            if (vendorId == 0) throw new Exception($"Vendor '{BillVM.VendorName}' not found.");
            if (grnId == 0) throw new Exception($"GRN '{BillVM.GRNNumber}' not found.");

            var bill = new Bill()
            {
                BillName = BillVM.BillName,
                BillDate = BillVM.BillDate,
                BillDueDate = BillVM.BillDueDate,
                VendorId = vendorId,
                BillTypeId = billTypeId,
                GoodsReceivedNoteId = grnId,
                VendorInvoiceNumber = BillVM.VendorInvoiceNumber,
                VendorDeliveryNumber = BillVM.VendorDeliveryNumber
            };

            _context.Bills.Add(bill);
            _context.SaveChanges();
        }
        public void Update(BillViewModel BillVM)
        {
            var bill = _context.Bills.FirstOrDefault(b => b.BillId == BillVM.BillId);
            if (bill != null)
            {
                var billTypeId = _context.BillTypes
                    .Where(bt => bt.BillTypeName == BillVM.BillTypeName)
                    .Select(bt => bt.BillTypeId)
                    .FirstOrDefault();

                var vendorId = _context.Vendors
                    .Where(v => v.VendorName == BillVM.VendorName)
                    .Select(v => v.VendorId)
                    .FirstOrDefault();

                var grnId = _context.ReceivedNotes
                    .Where(g => g.Name == BillVM.GRNNumber)
                    .Select(g => g.Id)
                    .FirstOrDefault();

                bill.BillName = BillVM.BillName;
                bill.BillDate = BillVM.BillDate;
                bill.BillDueDate = BillVM.BillDueDate;
                bill.VendorDeliveryNumber = BillVM.VendorDeliveryNumber;
                bill.VendorInvoiceNumber = BillVM.VendorInvoiceNumber;
               
                bill.VendorId = vendorId;
                bill.BillTypeId = billTypeId;
                bill.GoodsReceivedNoteId = grnId;
                


                _context.SaveChanges();
            }
        }
        public void Delete(int billId)
        {
            var bill =  _context.Bills.FirstOrDefault(b=>b.BillId==billId);
            if (bill != null)
            {
                try
                {
                    _context.Bills.Remove(bill);
                    _context.SaveChanges();

                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                    {
                        throw new InvalidOperationException(
                            "لا يمكن الحذف لأنه مرتبط ببيانات أخرى"
                        );
                    }

                    throw;
                }

            }
        }
        public async Task<BillViewModel> GetByIdAsync(int billId)
        {
            var bill = await _context.Bills
                .Include(b => b.Vendor)
                .Include(b => b.BillType)
                .Include(b => b.PaymentVouchers)
                .Include(b => b.GoodsReceivedNote)
                .FirstOrDefaultAsync(b => b.BillId == billId);

            if (bill == null)
                return null;

            var billViewModel = new BillViewModel()
            {
                BillId = bill.BillId,
                BillName = bill.BillName,
                BillDate = bill.BillDate,
                BillDueDate = bill.BillDueDate,
                VendorInvoiceNumber = bill.VendorInvoiceNumber,
                VendorDeliveryNumber = bill.VendorDeliveryNumber,

                GoodsReceivedNoteId = bill.GoodsReceivedNoteId,
                GRNNumber = bill.GoodsReceivedNote?.Name ?? "",
                GRNDate = bill.GoodsReceivedNote?.GRNDate ?? DateTimeOffset.MinValue,

                VendorId = bill.VendorId,
                VendorName = bill.Vendor?.VendorName ?? "",

                BillTypeId = bill.BillTypeId,
                BillTypeName = bill.BillType?.BillTypeName ?? "",

                PaymentVouchers = bill.PaymentVouchers.Select(pv => new PaymentVoucherViewModel
                {
                    Id = pv.Id,
                    PaymentAmount = pv.PaymentAmount,
                    IsFullPayment = pv.IsFullPayment
                }).ToList()

            };

            return billViewModel;
        }
        public async Task<PaginatedList<AllBillViewModel>> GetAllFilteredAsync(BillFilterationViewModel BF , int pageNumber , int pageSize)
        {
            var query = _context.Bills
                .Include(b => b.Vendor)
                .Include(b => b.BillType)
                .Include(b => b.PaymentVouchers)
                .Include( b => b.GoodsReceivedNote)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(BF.BillName))
            {
                string lowerBillName = BF.BillName.ToLower();
                query = query.Where(b => b.BillName.ToLower().Contains(lowerBillName));
            }

            if (!string.IsNullOrEmpty(BF.VendorName))
            {
                string lowerVendorName = BF.VendorName.ToLower();
                query = query.Where(b =>
                    b.Vendor != null &&
                    b.Vendor.VendorName.ToLower().Contains(lowerVendorName)
                );
            }

            if (!string.IsNullOrEmpty(BF.BillTypeName))
            {
                string lowerBillName = BF.BillTypeName.ToLower();
                query = query.Where(b =>
                    b.BillType != null &&
                    b.BillType.BillTypeName.ToLower().Contains(lowerBillName)
                );
            }

            if (!string.IsNullOrEmpty(BF.GoodsReceivedNote))
            {
                string lowerGRNNumber = BF.GoodsReceivedNote.ToLower();
                query = query.Where(b =>
                    b.GoodsReceivedNote != null &&
                    b.GoodsReceivedNote.Name.ToLower().Contains(lowerGRNNumber)
                );
            }
            if (!string.IsNullOrEmpty(BF.PaymentStatus))
            {
                var status = BF.PaymentStatus.ToLower();

                if (status == "Paid")
                    query = query.Where(b => b.PaymentVouchers.Any(pv => pv.IsFullPayment));

                else if (status == "Partially Paid")
                    query = query.Where(b => b.PaymentVouchers.Any() && b.PaymentVouchers.Any(pv => !pv.IsFullPayment));

                else if (status == "Unpaid")
                    query = query.Where(b => !b.PaymentVouchers.Any());
            }

            query = BF.SortByDueDateAsc ? query.OrderBy(b => b.BillDueDate) : query.OrderByDescending(b => b.BillDueDate);
            var result = query.Select(b => new AllBillViewModel
            {
                BillId = b.BillId,
                BillName = b.BillName,
                BillDate = b.BillDate,
                BillDueDate = b.BillDueDate,
                VendorId = b.VendorId,
                VendorName = b.Vendor != null ? b.Vendor.VendorName : "",
                BillTypeId = b.BillTypeId,
                BillTypeName = b.BillType != null ? b.BillType.BillTypeName : "",
                PaymentStatus = b.PaymentVouchers.Any()
                    ? b.PaymentVouchers.All(p => p.IsFullPayment) ? "Paid" : "Partial"
                    : "Unpaid"
            });

            return await PaginatedList<AllBillViewModel>.CreateAsync(result, pageNumber, pageSize);

        }
        public async Task<PaginatedList<AllBillViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Bills
                .Include(b => b.Vendor)
                .Include(b => b.BillType)
                .AsNoTracking()
                .OrderByDescending(b => b.BillDate)
                .Select(b => new AllBillViewModel
                {
                    BillId = b.BillId,
                    BillName = b.BillName,
                    BillDate = b.BillDate,
                    BillDueDate = b.BillDueDate,
                    VendorId = b.VendorId,
                    VendorDeliveryNumber = b.VendorDeliveryNumber,
                    VendorInvoiceNumber = b.VendorInvoiceNumber,
                    VendorName = b.Vendor != null ? b.Vendor.VendorName : "",
                    BillTypeId = b.BillTypeId,
                    BillTypeName = b.BillType != null ? b.BillType.BillTypeName : "",
                    PaymentVouchers = b.PaymentVouchers.Select(pv => new PaymentVoucherViewModel
                    {
                        Id = pv.Id,
                        PaymentAmount = pv.PaymentAmount,
                        IsFullPayment = pv.IsFullPayment
                    }).ToList()

                });

            return await PaginatedList<AllBillViewModel>.CreateAsync(query, pageNumber, pageSize);
        }
      
    }
}
