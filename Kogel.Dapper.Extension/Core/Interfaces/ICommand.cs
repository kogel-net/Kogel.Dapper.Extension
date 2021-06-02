using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface ICommand<T>
    {
		int Update(T entity, string[] excludeFields = null, int timeout = 120);

		int Update(IEnumerable<T> entities, IDbDataAdapter adapter, string[] excludeFields = null, int timeout = 120);

        int Update(Expression<Func<T, T>> updateExpression);

        Task<int> UpdateAsync(T entity, string[] excludeFields = null, int timeout = 120);

        Task<int> UpdateAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

        Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression);

        int Delete();

        int Delete(T model);

        Task<int> DeleteAsync();

        Task<int> DeleteAsync(T model);

        int Insert(T entity, string[] excludeFields = null);

        long InsertIdentity(T entity, string[] excludeFields = null);

        int Insert(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

        Task<int> InsertAsync(T entity, string[] excludeFields = null);

        Task<long> InsertIdentityAsync(T entity, string[] excludeFields = null);

        Task<int> InsertAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120);

	

	}
}
