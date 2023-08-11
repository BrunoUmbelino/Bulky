using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICateroryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
