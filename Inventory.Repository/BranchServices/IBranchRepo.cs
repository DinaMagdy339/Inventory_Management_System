using Inventory.Helper.Paging;
using Inventory.ViewModel.Branchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BranchServices
{
    public interface IBranchRepo
    {
        Task<PaginatedList<AllBranchViewModel>> GetAllAsync(int pageSize, int pageNumber, string? searchTerm);
        void Add(CreateBranchViewModel model);
        Task<BranchViewModel> GetByIdAsync(int? id);
        void Update(BranchViewModel model);
        void Delete(int branchId);
        Task<List<string>> GetForDropdownAsync();

    }
}
