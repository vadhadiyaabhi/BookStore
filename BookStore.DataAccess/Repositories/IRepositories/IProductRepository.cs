using BookStore.Models;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {
        bool Update(Product product);
    }
}
