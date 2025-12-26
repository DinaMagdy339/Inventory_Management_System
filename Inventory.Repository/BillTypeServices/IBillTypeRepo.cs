using Inventory.Helper.Paging;
using Inventory.ViewModel.Bills;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BillTypeServices
{
    public interface IBillTypeRepo
    {
        Task<PaginatedList<AllBillTypeViewModel>> GetAllAsync(int PageSize , int PageNumber, string? searchTerm);
        Task<BillTypeViewModel> GetByIdAsync(int id);
        void Add(CreateBillTypeVM model);
        void Update(BillTypeViewModel model);
        void Delete(int id);

    }
}
