using Microsoft.EntityFrameworkCore.Storage;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Repository.Abstraction;
using System;
using System.Threading.Tasks;

namespace Timpra.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int CommitChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> CommitChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void CreateTransaction()
        {
            if (_transaction == null)
            {
                _transaction = _dbContext.Database.BeginTransaction();
            }
            else
            {
                throw new InvalidOperationException("Transaction already exists! Use Rollback or Commit to close it.");
            }
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}
