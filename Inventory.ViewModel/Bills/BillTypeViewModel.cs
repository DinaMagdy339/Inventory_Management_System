using Inventory.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Bills
{
    public class BillTypeViewModel
    {
        public int BillTypeId { get; set; }

        [Required]
        public string BillTypeName { get; set; } = default!; 
        public string Description { get; set; } = default!; 

        public BillTypeViewModel() { }

       
    }
}