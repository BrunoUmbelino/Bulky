namespace Bulky.Models.ViewModels
{
    public class ShopCartVM
    {
        public IEnumerable<ShopCart> ShoppingCartItems { get; set; } = new List<ShopCart>();
        public OrderHeader OrderHeader { get; set; }
    }
}
