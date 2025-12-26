using Inventory.Helper.Paging;
using Inventory.ViewModel.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.BillServices
{
    public interface IBillRepo
    {

        void Add(BillViewModel billViewModel);
        void Update(BillViewModel billViewModel);
        void Delete(int billId);
        Task<BillViewModel> GetByIdAsync(int billId);
        Task<PaginatedList<AllBillViewModel>> GetAllFilteredAsync(BillFilterationViewModel billFilterationViewModel, int PageNumber, int PageSize);
        Task<PaginatedList<AllBillViewModel>> GetAllAsync(int pageNumber, int pageSize);


    }
}
