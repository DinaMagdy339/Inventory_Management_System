using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Warehouses
{
    public class AllWarehouseVM
    {
        public int WarehouseId { get; set; }
        [Required]
        public string WarehouseName { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public string BranchName { get; set; }
    }
}
