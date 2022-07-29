using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private readonly AppDbContext _db;
    public ShoppingCartRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public int DecrementCount(ShoppingCart cart, int count)
    {
        cart.Count -= count;
        return cart.Count;
    }

    public int IncrementCount(ShoppingCart cart, int count)
    {
        cart.Count += count;
        return cart.Count;
    }
}
