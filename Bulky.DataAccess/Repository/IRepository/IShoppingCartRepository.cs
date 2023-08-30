using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCartItem>
    {
        void Update(ShoppingCartItem shoppingCart);
    }
}
