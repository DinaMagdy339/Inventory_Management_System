using Inventory.Helper.Paging;
using Inventory.ViewModel.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.CurrencyServices
{
    public interface ICurrencyRepo
    {
        Task<PaginatedList<CurrencyViewModel>> GetAllCurrencies (int pageNum, int pageSize , string? searchTerm);
        Task<CurrencyViewModel> GetCurrencyById (int id);
        void Add (CurrencyViewModel model);
        void Update (CurrencyViewModel model);
        void Delete (int id);
        Task<List<string>> GetForDropdownAsync();

    }
}
