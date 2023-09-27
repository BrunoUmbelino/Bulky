using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models
{
    public class ProductImage
    {
        [Key] public int Id { get; set; }
        [Required] public string ImageUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))] public Product? Product { get; set; }
    }
}
