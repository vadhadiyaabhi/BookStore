using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        bool Update(OrderDetails orderDetails);
    }
}
