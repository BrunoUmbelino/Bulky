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
                productDB.ISBN = product.ISBN;
                productDB.Author = product.Author;
                productDB.ListPrice = product.ListPrice;
                productDB.PriceUp50 = product.PriceUp50;
                productDB.PriceUp100 = product.PriceUp100;
                productDB.PriceAbove100 = product.PriceAbove100;
                productDB.CategoryId = product.CategoryId;
                productDB.Images = product.Images;

                _context.Products.Update(productDB);
            } 
        } 
    }
}
