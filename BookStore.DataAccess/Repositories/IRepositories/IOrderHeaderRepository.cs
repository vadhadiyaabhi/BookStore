using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        bool Update(OrderHeader orderHeader);

        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        void UpdatePaymentStatus(int id, string sessionId, string paymentIntentId);
    }
}
