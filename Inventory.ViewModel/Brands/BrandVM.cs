using Inventory.Models;
using System.ComponentModel.DataAnnotations;
namespace Inventory.ViewModel.Brands
{
    public class BrandVM
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<BrandProductVM> Products { get; set; } = new();


    }
}
