using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Dapper.Extension.Core.SetQ
{
	/// <summary>
	/// 查询
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Query<T> : AbstractSet, IQuery<T>
	{
		public readonly SqlProvider SqlProvider;
		public readonly IDbConnection DbCon;
		public readonly IDbTransaction DbTransaction;

		protected DataBaseContext<T> SetContext { get; set; }

		protected Query(IDbConnection conn, SqlProvider sqlProvider)
		{
			SqlProvider = sqlProvider;
			DbCon = conn;
		}

		protected Query(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction)
		{
			SqlProvider = sqlProvider;
			DbCon = conn;
			DbTransaction = dbTransaction;
		}

		public T Get()
		{
			SqlProvider.FormatGet<T>();
			return DbCon.QueryFirstOrDefaults<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, SqlProvider.ProviderOption);

		}
		public TSource Get<TSource>()
		{
			SqlProvider.FormatGet<T>();
			return DbCon.QueryFirstOrDefaults<TSource>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, SqlProvider.ProviderOption);
		}
		public TReturn Get<TReturn>(Expression<Func<T, TReturn>> select)
		{
			SqlProvider.Context.Set.SelectExpression = select;
			SqlProvider.FormatGet<T>();
			return DbCon.QueryFirst_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
		}
		public TReturn Get<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect)
		{
			if(where)
				SqlProvider.Context.Set.SelectExpression = trueSelect;
			else
				SqlProvider.Context.Set.SelectExpression = falseSelect;
			SqlProvider.FormatGet<T>();
			return DbCon.QueryFirst_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
		}
		public async Task<T> GetAsync()
		{
			SqlProvider.FormatGet<T>();
			return await DbCon.QueryFirstOrDefaultAsyncs<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
		}
		public IEnumerable<T> ToIEnumerable()
		{
			SqlProvider.FormatToList<T>();
			return DbCon.Querys<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, SqlProvider.ProviderOption);
		}
		public IEnumerable<TSource> ToIEnumerable<TSource>()
		{
			SqlProvider.FormatToList<T>();
			return DbCon.Querys<TSource>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, SqlProvider.ProviderOption);
		}
		public IEnumerable<TReturn> ToIEnumerable<TReturn>(Expression<Func<T, TReturn>> select)
		{
			SqlProvider.Context.Set.SelectExpression = select;
			SqlProvider.FormatToList<T>();
			return DbCon.Query_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
		}
		public IEnumerable<TReturn> ToIEnumerable<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect)
		{
			if (where)
				SqlProvider.Context.Set.SelectExpression = trueSelect;
			else
				SqlProvider.Context.Set.SelectExpression = falseSelect;
			SqlProvider.FormatToList<T>();
			return DbCon.Query_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
		}
		public async Task<IEnumerable<T>> ToIEnumerableAsync()
		{
			SqlProvider.FormatToList<T>();
			return await DbCon.QueryAsyncs<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
		}

		public List<T> ToList()
		{
			SqlProvider.FormatToList<T>();
			return DbCon.Querys<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, SqlProvider.ProviderOption).ToList();
		}
		public List<TSource> ToList<TSource>()
		{
			SqlProvider.FormatToList<T>();
			return DbCon.Querys<TSource>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, SqlProvider.ProviderOption).ToList();
		}
		public List<TReturn> ToList<TReturn>(Expression<Func<T, TReturn>> select)
		{
			SqlProvider.Context.Set.SelectExpression = select;
			SqlProvider.FormatToList<T>();
			return DbCon.Query_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
		}
		public List<TReturn> ToList<TReturn>(bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect)
		{
			if (where)
				SqlProvider.Context.Set.SelectExpression = trueSelect;
			else
				SqlProvider.Context.Set.SelectExpression = falseSelect;
			SqlProvider.FormatToList<T>();
			return DbCon.Query_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
		}
		public async Task<List<T>> ToListAsync()
		{
			SqlProvider.FormatToList<T>();
			var iEnumerable = await DbCon.QueryAsyncs<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
			return iEnumerable.ToList();
		}

		public PageList<T> PageList(int pageIndex, int pageSize)
		{
			SqlProvider.FormatToPageList<T>(pageIndex, pageSize);
			using (var queryResult = DbCon.QueryMultiples<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction))
			{
				//oracle不支持返回多条结果集
				var pageTotal = 0;
				if (SqlProvider.IsSelectCount)
				{
					SqlProvider.FormatCount();
					pageTotal = DbCon.QuerySingles<int>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
				}
				else
				{
					pageTotal = queryResult.ReadFirst<int>();
				}
				var itemList = queryResult.Read<T>().ToList();
				return new PageList<T>(pageIndex, pageSize, pageTotal, itemList);
			}
		}
		public PageList<TSource> PageList<TSource>(int pageIndex, int pageSize)
		{
			SqlProvider.FormatToPageList<T>(pageIndex, pageSize);
			using (var queryResult = DbCon.QueryMultiples<TSource>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction))
			{
				//oracle不支持返回多条结果集
				var pageTotal = 0;
				if (SqlProvider.IsSelectCount)
				{
					SqlProvider.FormatCount();
					pageTotal = DbCon.QuerySingles<int>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
				}
				else
				{
					pageTotal = queryResult.ReadFirst<int>();
				}
				var itemList = queryResult.Read<TSource>().ToList();
				return new PageList<TSource>(pageIndex, pageSize, pageTotal, itemList);
			}
		}

		public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T, TReturn>> select)
		{
			//查询总行数
			SqlProvider.FormatCount();
			var pageTotal = DbCon.QuerySingles<int>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
			//查询数据
			SqlProvider.Context.Set.SelectExpression = select;
			SqlProvider.FormatToPageList<T>(pageIndex, pageSize, false);
			var itemList = DbCon.Query_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
			return new PageList<TReturn>(pageIndex, pageSize, pageTotal, itemList);
		}

		public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T, TReturn>> trueSelect, Expression<Func<T, TReturn>> falseSelect)
		{
			//查询总行数
			SqlProvider.FormatCount();
			var pageTotal = DbCon.QuerySingles<int>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
			if (where)
				SqlProvider.Context.Set.SelectExpression = trueSelect;
			else
				SqlProvider.Context.Set.SelectExpression = falseSelect;
			SqlProvider.FormatToPageList<T>(pageIndex, pageSize, false);
			var itemList = DbCon.Query_1<TReturn>(SqlProvider.SqlString, SqlProvider.ProviderOption, SqlProvider.Params, DbTransaction);
			return new PageList<TReturn>(pageIndex, pageSize, pageTotal, itemList);
		}
	}
}
