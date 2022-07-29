using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;

namespace BookStore.DataAccess.Repositories;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly AppDbContext _db;

    public OrderHeaderRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }


    public bool Update(OrderHeader orderHeader)
    {
        _db.OrderHeaders.Update(orderHeader);
        return true;
    }

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        OrderHeader orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
        if(orderHeaderFromDb != null)
        {
            orderHeaderFromDb.PaymentStatus = paymentStatus;
        }
    }

    public void UpdatePaymentStatus(int id, string sessionId, string paymentIntentId)
    {
        OrderHeader? orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
        if (orderHeaderFromDb != null)
        {
            orderHeaderFromDb.SessionId = sessionId;
            orderHeaderFromDb.PaymentIntentId = paymentIntentId;
        }
    }
}
