using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.ViewModels
{
    public class ProductVM
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ISBN-13")]
        [MaxLength(20)]
        public string ISBN13 { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [Display(Name = "List Price")]
        [DataType(DataType.Currency)]
        public decimal PriceList { get; set; }

        [Required]
        [Display(Name = "Standart Price")]
        [DataType(DataType.Currency)]
        public decimal PriceStandart { get; set; }

        [Required]
        [Display(Name = "Price over 50 units")]
        [DataType(DataType.Currency)]
        public decimal PriceOver50 { get; set; }

        [Required]
        [Display(Name = "Price over 100 units")]
        [DataType(DataType.Currency)]
        public decimal PriceOver100 { get; set; } 

        public int CategoryId { get; set; }

        [ValidateNever]
        public Category? Category { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }

        [ValidateNever]
        public List<ProductImage> Images { get; set; } = new();
    }
}
