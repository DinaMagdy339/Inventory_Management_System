using Inventory.Helper.Paging;
using Inventory.ViewModel.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.CustomerServices
{
    public interface ICustomerRepo
    {
        Task<PaginatedList<CustomerViewModel>> GetAllAsync(int PageSize,int PageNumber , string? searchTerm);
        Task<CustomerViewModel> GetByIdAsync(int id);
        void Create(CustomerViewModel customerViewModel);
        void Update(CustomerViewModel customerViewModel);
        void Delete(int id);
        Task<List<string>> GetForDropdownAsync();


    }
}
