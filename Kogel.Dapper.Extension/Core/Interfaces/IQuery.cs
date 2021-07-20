using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Entites;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface IQuery<T>
    {
        T Get();

        TSource Get<TSource>();

        TReturn Get<TReturn>(Expression<Func<T, TReturn>> select);

        TReturn Get<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        Task<T> GetAsync();

        Task<TSource> GetAsync<TSource>();

        Task<TReturn> GetAsync<TReturn>(Expression<Func<T, TReturn>> select);

        Task<TReturn> GetAsync<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        IEnumerable<T> ToIEnumerable();

        IEnumerable<TSource> ToIEnumerable<TSource>();

        IEnumerable<TReturn> ToIEnumerable<TReturn>(Expression<Func<T, TReturn>> select);

        IEnumerable<TReturn> ToIEnumerable<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        Task<IEnumerable<T>> ToIEnumerableAsync();

        Task<IEnumerable<TSource>> ToIEnumerableAsync<TSource>();

        Task<IEnumerable<TReturn>> ToIEnumerableAsync<TReturn>(Expression<Func<T, TReturn>> select);

        Task<IEnumerable<TReturn>> ToIEnumerableAsync<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        List<T> ToList();

        List<TSource> ToList<TSource>();

        List<TReturn> ToList<TReturn>(Expression<Func<T, TReturn>> select);

        List<TReturn> ToList<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        Task<List<T>> ToListAsync();

        Task<List<TSource>> ToListAsync<TSource>();

        Task<List<TReturn>> ToListAsync<TReturn>(Expression<Func<T, TReturn>> select);

        Task<List<TReturn>> ToListAsync<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        List<T> Page(int pageIndex, int pageSize);

        List<TSource> Page<TSource>(int pageIndex, int pageSize);

        List<TReturn> Page<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select);

        List<TReturn> Page<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        Task<List<T>> PageAsync(int pageIndex, int pageSize);

        Task<List<TSource>> PageAsync<TSource>(int pageIndex, int pageSize);

        Task<List<TReturn>> PageAsync<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select);

        Task<List<TReturn>> PageAsync<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        PageList<T> PageList(int pageIndex, int pageSize);

        PageList<TSource> PageList<TSource>(int pageIndex, int pageSize);

        PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select);

        PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        Task<PageList<T>> PageListAsync(int pageIndex, int pageSize);

        Task<PageList<TSource>> PageListAsync<TSource>(int pageIndex, int pageSize);

        Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select);

        Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

        DataSet ToDataSet(IDbDataAdapter dataAdapter = null);

        DataSet ToDataSet<TReturn>(Expression<Func<T, TReturn>> select, IDbDataAdapter dataAdapter = null);

        DataSet ToDataSet<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null);

        Task<DataSet> ToDataSetAsync(IDbDataAdapter dataAdapter = null);

        Task<DataSet> ToDataSetAsync<TReturn>(Expression<Func<T, TReturn>> select, IDbDataAdapter dataAdapter = null);

        Task<DataSet> ToDataSetAsync<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null);
    }


    public interface IQuery<T, TReturn> : IQuery<T>
    {
        new List<TReturn> ToList();
    }
}
