using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICateroryRepository CateroryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CateroryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context);
            CompanyRepository = new CompanyRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
