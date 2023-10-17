using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models
{
    public class ShopCart
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

        public List<ShopCartItem> ShopCartItems { get; set; } = new List<ShopCartItem>();

        public DateTime CreatedIn { get; set; } 

        public DateTime UpdatedIn { get; set; }
    }
}
