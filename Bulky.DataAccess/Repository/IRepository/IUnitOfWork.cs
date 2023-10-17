namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository ProductRepo { get; }
        IProductImageRepository ProductImageRepo { get; }
        ICategoryRepository CategoryRepo { get; }

        ICompanyRepository CompanyRepo { get; }
        IApplicationUserRepository ApplicationUserRepo { get; }

        IShopCartRepository ShopCartRepo { get; }
        IShopCartItemRepository ShopCartItemRepo { get; }
        IOrderHeaderRepository OrderHeaderRepo { get; }
        IOrderDetailRepository OrderDetailRepo { get; }

        void Save();
    }
}
