using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productDB = _context.Products.FirstOrDefault(p=>p.Id == product.Id);

            if (productDB != null)
            {
                productDB.Title = product.Title;
                productDB.Description = product.Description;
                productDB.ISBN13 = product.ISBN13;
                productDB.Author = product.Author;
                productDB.PriceList = product.PriceList;
                productDB.PriceStandart = product.PriceStandart;
                productDB.PriceOver50 = product.PriceOver50;
                productDB.PriceOver100 = product.PriceOver100;
                productDB.CategoryId = product.CategoryId;
                productDB.Images = product.Images;

                _context.Products.Update(productDB);
            } 
        } 
    }
}
