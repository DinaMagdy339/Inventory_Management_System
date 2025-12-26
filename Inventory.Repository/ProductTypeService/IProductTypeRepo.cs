using Inventory.Helper.Paging;
using Inventory.ViewModel.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.ProductTypeService
{
    public interface IProductTypeRepo
    {
        Task<PaginatedList<ProductTypeViewModel>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<ProductTypeViewModel> GetById(int id);
        void Create(ProductTypeViewModel model);
        void Delete(int id);
        void Update(ProductTypeViewModel model);

        Task<List<string>> GetForDropdownAsync();

    }
}
