using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Timpra.DataAccess.Repository.Abstraction
{
    public interface IRepository<TEntity>
    {
        Task AddAsync(TEntity item, bool applyChanges = true);
        Task UpdateAsync(TEntity item, int id, bool applyChanges = true);
        Task RemoveAsync(TEntity item, bool applyChanges = true);
        Task RemoveRangeAsync(IEnumerable<TEntity> items, bool applyChanges = true);
        Task<IQueryable<TEntity>> GetAll();
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetByIdAsync<TId>(TId id);
    }
}
