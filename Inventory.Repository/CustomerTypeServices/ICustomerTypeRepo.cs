using Inventory.Helper.Paging;
using Inventory.ViewModel.Customers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.CustomerTypeServices
{
    public interface ICustomerTypeRepo
    {
        Task<PaginatedList<CustomerTypeViewModel>> GetAllAsync(int pageSize, int pageNumber , string? SearchTerm);

        Task<CustomerTypeViewModel> GetByIdAsync(int id);
        void Add(CustomerTypeViewModel model);
        void Update(CustomerTypeViewModel model);
        void Delete(int id);

        Task<List<string>> GetForDropdownAsync();

    }
}
