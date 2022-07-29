using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// This is the basic setup that we've done to implement the SOLID repository, That will work same for all the classes

namespace BookStore.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(AppDbContext db)
        {
            _db = db;


            // ==========================================================================================================================
            //_db.Products.Include(x => x.Category).Include(x => x.CoverType);                            // Include property to fetch data from dependent tables also


            // ------------------------------------------ very very usefull
            // IF that table is also dependent on some other table, then we might want to go to that table also
            // In this case we have .ThenInclude() - read & understand 
            // same way we have .Add() method to add data on dependent table, we dont need to do it manually LIKE - first store & get id and on that id store in child table
            // This things will be handled by entity framework


            // ==========================================================================================================================


            // This will set our dbSet to perticular instance of the class that is calling our repository
            // For ex: dbSet = _db.Categories In Case of Category class
            this.dbSet = _db.Set<T>();                           
        }

        public async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        // includeProperties = "Category, CoverType"            // uppercase is must
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }
            

            // MIMIP -------------------------------------------Note down the include props logic
            if(includeProperties != null)
            {
                foreach(var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
                
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties)
        {
            IQueryable<T> query = dbSet;

            // MIMIP -------------------------------------------Note down the include props logic
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

            }


            return await query.FirstOrDefaultAsync(filter);
        }

        public bool Remove(T entity)
        {
            dbSet.Remove(entity);
            return true;
        }

        public bool RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            return true;
        }
    }
}
