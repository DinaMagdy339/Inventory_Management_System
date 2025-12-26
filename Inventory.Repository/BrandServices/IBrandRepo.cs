using Inventory.Helper.Paging;
using Inventory.ViewModel.Brands;
using Inventory.ViewModel.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BrandServices
{
    public interface IBrandRepo
    {
        Task<PaginatedList<BrandVM>> GetAllAsync(int pageSize, int pageNumber, string? SearchTerm);

        Task<BrandVM> GetByIdAsync(int id);
        void Add(BrandVM model);
        void Update(BrandVM model);
        void Delete(int id);

        Task<List<string>> GetForDropdownAsync();

    }
}
