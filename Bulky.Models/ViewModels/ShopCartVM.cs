using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.ViewModels
{
    public class ShopCartVM
    {
        [Key]
        public int Id { get; set; }

        public List<ShopCartItem> ShopCartItems { get; set; } = new List<ShopCartItem>();

        [Required]
        [Column(TypeName = "smallint")]
        public short QuantityItems => (short)ShopCartItems.Count;

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal TotalValue { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        public OrderHeader OrderHeader { get; set; } = new OrderHeader();
    }
}
