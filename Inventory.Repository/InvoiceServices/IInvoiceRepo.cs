using Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.InvoiceServices
{
    public interface IInvoiceRepo
    {
        Task<List<Invoice>> GetOpenInvoicesForCustomerAsync(string customerName);
    }
}
