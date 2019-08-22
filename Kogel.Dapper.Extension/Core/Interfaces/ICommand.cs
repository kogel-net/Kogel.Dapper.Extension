using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface ICommand<T>
    {
        int Update(T entity);

        Task<int> UpdateAsync(T entity);

        int Update(Expression<Func<T, T>> updateExpression);

        Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression);

        int Delete();

        Task<int> DeleteAsync();
        #region insert
        int Insert(T entity);

        Task<int> InsertAsync(T entity);

        int InsertIdentity(T entity);
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        int BatchInsert(IEnumerable<T> entities, int timeout = 120);

        Task<int> BatchInsertAsync(IEnumerable<T> entities, int timeout = 120);
        #endregion
    }
}
