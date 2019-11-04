using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface IQuery<T>
    {
        T Get();

        TReturn Get<TReturn>(Expression<Func<T, TReturn>> select);

        Task<T> GetAsync();

        IEnumerable<T> ToIEnumerable();

        IEnumerable<TReturn> ToIEnumerable<TReturn>(Expression<Func<T, TReturn>> select);

        Task<IEnumerable<T>> ToIEnumerableAsync();

		List<T> ToList();

		List<TReturn> ToList<TReturn>(Expression<Func<T, TReturn>> select);

		Task<List<T>> ToListAsync();

		PageList<T> PageList(int pageIndex, int pageSize);

        PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select);
    }
}
