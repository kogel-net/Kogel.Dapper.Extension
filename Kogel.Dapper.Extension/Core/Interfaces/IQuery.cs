using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
	public interface IQuery<T>
	{
		T Get();

		TReturn Get<TReturn>(Expression<Func<T, TReturn>> select);

		TReturn Get<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

		Task<T> GetAsync();

		IEnumerable<T> ToIEnumerable();

		IEnumerable<TReturn> ToIEnumerable<TReturn>(Expression<Func<T, TReturn>> select);

		IEnumerable<TReturn> ToIEnumerable<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

		Task<IEnumerable<T>> ToIEnumerableAsync();

		List<T> ToList();

		List<TReturn> ToList<TReturn>(Expression<Func<T, TReturn>> select);

		List<TReturn> ToList<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

		Task<List<T>> ToListAsync();

		PageList<T> PageList(int pageIndex, int pageSize);

		PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select);

		PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect);

		DataSet ToDataSet(IDbDataAdapter dataAdapter = null);

		DataSet ToDataSet<TReturn>(Expression<Func<T, TReturn>> select, IDbDataAdapter dataAdapter = null);

		DataSet ToDataSet<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null);
	}
}
