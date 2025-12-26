
using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.InvoiceServices
{
    public class InvoiceRepo : IInvoiceRepo
    {
        private readonly ApplicationDbContext _context;
        public InvoiceRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetOpenInvoicesForCustomerAsync(string customerName)
        {
            // جلب رقم العميل
            var customerId = await _context.Customers
                .Where(c => c.CustomerName.ToLower() == customerName.ToLower())
                .Select(c => c.CustomerId)
                .FirstOrDefaultAsync();

            if (customerId == 0)
                return new List<Invoice>(); // العميل مش موجود

            // جلب الفواتير المفتوحة
            var invoices = await _context.Invoices
                .Where(i => i.CustomerId == customerId)
                .Where(i => i.InvoiceItems.Any(ii => !ii.IsPaid))
                .ToListAsync();

            return invoices;
        }

    }
}
