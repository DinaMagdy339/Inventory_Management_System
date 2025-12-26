using Inventory.Helper.Paging;
using Inventory.ViewModel.MeasureUnits;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.MeasureUnitServices
{
    public interface IMeasureUnitRepo
    {
        Task<PaginatedList<MeasureUnitVM>> GetAllAsync(int pageSize, int pageNumber, string? SearchTerm);
        Task<MeasureUnitVM> GetByIdAsync(int id);
        void Add(MeasureUnitVM model);
        void Update(MeasureUnitVM model);
        void Delete(int id);
        Task<List<SelectListItem>> GetForDropdownAsync();
    }
}
