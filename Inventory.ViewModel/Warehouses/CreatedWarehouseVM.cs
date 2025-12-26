using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Warehouses
{
    public class CreatedWarehouseVM
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Description { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
