using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _context;
        public CompanyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void update(Company company)
        {
            var companyDB = _context.Companies.FirstOrDefault(c=>c.Id == company.Id);

            if (companyDB != null)
            {
                companyDB.Name = company.Name;
                companyDB.StreetAddress = company.StreetAddress;
                companyDB.City = company.City;
                companyDB.State = company.State;
                companyDB.PostalCode = company.PostalCode;

                _context.Companies.Update(companyDB);
            }
        }
    }
}
