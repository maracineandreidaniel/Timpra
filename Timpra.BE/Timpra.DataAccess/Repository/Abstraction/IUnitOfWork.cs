using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timpra.DataAccess.Repository.Abstraction
{
    public interface IUnitOfWork
    {
        int CommitChanges();
        Task<int> CommitChangesAsync();
        void CreateTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
