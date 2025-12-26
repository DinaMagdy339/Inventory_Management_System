using Inventory.Helper.Paging;

namespace Inventory.ViewModel.Bills
{
    public class BillIndexViewModel
    {
        public BillFilterationViewModel Filter { get; set; } = new BillFilterationViewModel();
        public PaginatedList<AllBillViewModel> Bills { get; set; }
    }
}
