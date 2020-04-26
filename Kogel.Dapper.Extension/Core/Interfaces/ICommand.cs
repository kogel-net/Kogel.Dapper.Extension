using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface ICommand<T>
    {
        int Insert(T entity, string[] excludeFields = null);

        Task<int> InsertAsync(T entity, string[] excludeFields = null);

        int Insert(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

        Task<int> InsertAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

        int InsertIdentity(T entity, string[] excludeFields = null);

        Task<int> InsertIdentityAsync(T entity, string[] excludeFields = null);


        int Update(T entity, string[] excludeFields = null);

        Task<int> UpdateAsync(T entity, string[] excludeFields = null);

        int Update(Expression<Func<T, T>> updateExpression);

        Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression);

        int Update(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

        Task<int> UpdateAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

        int Delete();
        Task<int> DeleteAsync();

        int Delete(T model);
        Task<int> DeleteAsync(T model);

    }
}
