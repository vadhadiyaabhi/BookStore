using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        bool Update(Category category);

        // Now defined in IUnitOfWork
        //void Save();
    }
}
