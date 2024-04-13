using Microsoft.EntityFrameworkCore;
using Product.Data.DbContexts;
using Product.Data.Interfaces.IRepositories;
using Product.Domain.Commons;
using System.Linq.Expressions;

namespace Product.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Auditable
    {
        protected readonly ProductDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ProductDbContext productDbContext)
        {
            this._dbContext = productDbContext;
            this._dbSet = productDbContext.Set<TEntity>();
        }
        public virtual TEntity Insert(TEntity entity)
            => _dbSet.Add(entity).Entity;

        public virtual void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity is not null)
                _dbSet.Remove(entity);
        }
        
        public IQueryable<TEntity> SelectAll()
            => _dbSet;

        public virtual async Task<TEntity?> FindByIdAsync(long id)
            => await _dbSet.FindAsync(id);

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
            => await _dbSet.FirstOrDefaultAsync(expression);

        public void TrackingDeteched(TEntity entity)
        {
            _dbContext.Entry<TEntity>(entity!).State = EntityState.Detached;
        }

        public virtual void Update(long id, TEntity entity)
        {
            entity.Id = id;
            _dbSet.Update(entity);
        }
    }
}
