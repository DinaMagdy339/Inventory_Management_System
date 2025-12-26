using Inventory.Helper.Paging;
using Inventory.ViewModel.Warehouses;

namespace Inventory.Repository.WarehouseServices
{
    public interface IWarehouseRepo
    {
        Task<PaginatedList<AllWarehouseVM>> GetAllWarehousesAsync(int pageNum,int oageSize,String ? SearchTerm);
        Task<WarehouseVM> GetWarehouseByIdAsync(int warehouseId);
        void Create(CreatedWarehouseVM warehouseVM);
        void Update(CreatedWarehouseVM warehouseVM);
        void Delete(int warehouseId);
        Task<List<string>> GetForDropdownAsync();

    }
}
