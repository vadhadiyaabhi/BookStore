using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private readonly AppDbContext _db;
    public CompanyRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public bool Update(Company company)
    {
        _db.Companies.Update(company);
        return true;
    }
}
