using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCartItem>, IShoppingCartRepository
    {
        private AppDbContext _context;
        
        public ShoppingCartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ShoppingCartItem shoppingCart)
        {
            _context.ShoppingCarts.Update(shoppingCart);
        }
    }
}
