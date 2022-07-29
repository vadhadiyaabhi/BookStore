using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly AppDbContext _db;

        public CoverTypeRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public bool Update(CoverType coverType)
        {
            _db.CoverTypes.Update(coverType);
            return true;
        }
    }
}
