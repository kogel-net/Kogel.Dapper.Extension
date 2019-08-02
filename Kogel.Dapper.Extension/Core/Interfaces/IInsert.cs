using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface IInsert<T>
    {
        int Insert(T entity);

        Task<int> InsertAsync(T entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        int BatchInsert(IEnumerable<T> entities, int timeout = 120);

        Task<int> BatchInsertAsync(IEnumerable<T> entities, int timeout = 120);
    }
}
