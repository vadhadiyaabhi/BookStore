using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories;

public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
{
    private readonly AppDbContext _db;
    public OrderDetailsRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public bool Update(OrderDetails orderDetails)
    {
        _db.OrderDetails.Update(orderDetails);
        return true;
    }
}
