using Microsoft.EntityFrameworkCore.ChangeTracking;
using Product.Domain.Commons;
using System.Linq.Expressions;

namespace Product.Data.Interfaces.IRepositories
{
    public interface IRepository<TEntity> where TEntity : Auditable
    {
        public void Delete(long id);

        IQueryable<TEntity> SelectAll();

        public TEntity Insert(TEntity entity);

        public void Update(long id, TEntity entity);

        public void TrackingDeteched(TEntity entity);

        public Task<TEntity?> FindByIdAsync(long id);

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

        public Task<int> SaveChangesAsync();

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : Auditable;
    }
}
