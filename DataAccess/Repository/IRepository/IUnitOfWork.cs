namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICateroryRepository CateroryRepository { get; }
        void Save();
    }
}
