using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository CategoryRepo { get; private set; }
        public IProductRepository ProductRepo { get; private set; }
        public ICompanyRepository CompanyRepo { get; private set; }
        public IShoppingCartRepository ShoppingCartRepo { get; private set; }
        public IApplicationUserRepository ApplicationUserRepo { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepo { get; private set; }
        public IOrderDetailRepository OrderDetailRepo { get; private set; }
        public IProductImageRepository ProductImageRepo {  get; private set; }
        
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CategoryRepo = new CategoryRepository(_context);
            ProductRepo = new ProductRepository(_context);
            CompanyRepo = new CompanyRepository(_context);
            ShoppingCartRepo = new ShoppingCartRepository(_context);
            ApplicationUserRepo = new ApplicationUserRepository(_context);
            OrderHeaderRepo = new OrderHeaderRepository(_context);
            OrderDetailRepo = new OrderDetailRepository(_context);
            ProductImageRepo = new ProductImageRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
