using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface ICommand<T>
    {
        int Update(T entity);

		int Update(IEnumerable<T> entities, int timeout = 120);

        Task<int> UpdateAsync(T entity);

        int Update(Expression<Func<T, T>> updateExpression);

        Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression);

        int Delete();

        Task<int> DeleteAsync();

        int Insert(T entity);

        Task<int> InsertAsync(T entity);

        int InsertIdentity(T entity);

        int Insert(IEnumerable<T> entities, int timeout = 120);

        Task<int> InsertAsync(IEnumerable<T> entities, int timeout = 120);
    }
}
