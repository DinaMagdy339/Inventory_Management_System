using Inventory.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Branchs
{
    public class BranchViewModel
    {
        public int BranchId { get; set; }
        [Required]
        public string BranchName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CurrencyId { get; set; }
        public string? Currency { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        [RegularExpression(@"^\+?\d+$", ErrorMessage = "رقم الهاتف يجب أن يحتوي على أرقام ويمكن أن يبدأ بعلامة +")]

        public string phone { get; set; } = default!;
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]

        public string Email { get; set; } = default!;
        [Display(Name = "Contact Person")]
        [RegularExpression(@"^\+?\d+$", ErrorMessage = "رقم الهاتف يجب أن يحتوي على أرقام ويمكن أن يبدأ بعلامة +")]

        public string ContactPerson { get; set; } = default!;
        public List<BranchWarehouseVM> Warehouses { get; set; } = new ();


    }
}
