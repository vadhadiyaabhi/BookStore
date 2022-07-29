using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        bool Update(CoverType coverType);
    }
}
