using Inventory.Helper.Paging;
using Inventory.ViewModel.Banks;
using Inventory.ViewModel.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BankServices
{
    public interface IBankRepo
    {
        Task<PaginatedList<BankVM>> GetAllAsync(int pageSize, int pageNumber, string? SearchTerm);

        Task<BankVM> GetByIdAsync(int id);
        void Add(BankVM model);
        void Update(BankVM model);
        void Delete(int id);

        Task<List<string>> GetForDropdownAsync();

    }
}
