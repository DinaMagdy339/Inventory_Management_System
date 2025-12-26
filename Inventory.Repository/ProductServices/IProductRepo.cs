using Inventory.ViewModel.Products;
using Inventory.Helper.Paging;

namespace Inventory.Repository.ProductServices
{
    public interface IProductRepo
    {
        Task<PaginatedList<AllProductsVM>> GetAllAsync(int pageNumber, int pageSize ,string? searchTerm);
        Task<ProductViewModel> GetByIdAsync(int id);
        void Add(ProductViewModel model);
        void Update(ProductViewModel model);
        void Delete(int id);
        Task<List<string?>> GetForDropdownAsync();


    }
}
