using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        bool Update(Company company);
    }
}
