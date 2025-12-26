using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.ViewModel.Customers
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        [Required]
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        [RegularExpression(@"^\+?\d+$", ErrorMessage = "رقم الهاتف يجب أن يحتوي على أرقام ويمكن أن يبدأ بعلامة +")]

        public string? Phone { get; set; }
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]

        public string? Email { get; set; }
        [Display(Name = "Customer Type")]
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        [Display(Name = "Contact Person")]
        [RegularExpression(@"^\+?\d+$", ErrorMessage = "رقم الهاتف يجب أن يحتوي على أرقام ويمكن أن يبدأ بعلامة +")]

        public string? ContactPerson { get; set; }

    }
}
