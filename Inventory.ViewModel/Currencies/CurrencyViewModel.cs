using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Currencies
{
    public class CurrencyViewModel
    {

        public int Id { get; set; }
        public string Code { get; set; }
        [Required]

        public string Name { get; set; }
        public string Description { get; set; }

    }
}
