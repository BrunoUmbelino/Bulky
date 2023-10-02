using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required] [MaxLength(30)] 
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required] 
        public string ISBN { get; set; } = string.Empty;
        
        [Required] 
        public string Author { get; set; } = string.Empty;

        [Required] [Display(Name = "List Price")] [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required] [Display(Name = "Price between 1-50 units")] [Range(1, 1000)]
        public double PriceUp50 { get; set; }

        [Required] [Display(Name = "Price between 51-100 units")] [Range(1, 1000)]
        public double PriceUp100 { get; set; }

        [Required] [Display(Name = "Price above 100 units")] [Range(1, 1000)]
        public double PriceAbove100 { get; set; }

        public int CategoryId { get; set; }

        [ValidateNever] [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        [ValidateNever]
        public List<ProductImage> Images { get; set; } = new();
    }
}
