using Inventory.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Branchs
{
    public class AllBranchViewModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = default!;
        public string Description { get; set; }= default!;
        public string Currency { get; set; } = default!;
        public string City { get; set; } = default!;
       
    }
}
