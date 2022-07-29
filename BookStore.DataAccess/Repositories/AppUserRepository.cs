using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories;

public class AppUserRepository : Repository<AppUser>, IAppUserRepository
{
    private readonly AppDbContext _db;
    public AppUserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    //public bool Update(AppUser appUser)
    //{
    //    _db.AppUsers.Update(appUser);
    //    return true;
    //}
}
