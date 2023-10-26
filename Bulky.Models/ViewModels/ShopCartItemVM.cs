using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.ViewModels
{
    public class ShopCartItemVM
    {
        public int? Id { get; set; }

        [Required]
        [Column(TypeName = "smallint")]
        [Range(1, short.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public short Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Required]
        public int ShopCartId { get; set; }

        [ForeignKey(nameof(ShopCartId))]
        public ShopCart? ShopCart { get; set; }
    }
}
