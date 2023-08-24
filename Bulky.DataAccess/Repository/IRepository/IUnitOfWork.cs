namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICateroryRepository CateroryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        void Save();
    }
}
