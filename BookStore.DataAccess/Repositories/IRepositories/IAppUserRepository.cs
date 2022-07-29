using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        //bool Update(AppUser appUser);

        // Now defined in IUnitOfWork
        //void Save();
    }
}
