using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly AppDbContext _db;
    public CategoryRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    //Now this db level method handled by UnitOfWork repository
    //public void Save()
    //{
    //    _db.SaveChanges();
    //}

    public bool Update(Category category)
    {
        _db.Categories.Update(category);
        return true;
    }
}
