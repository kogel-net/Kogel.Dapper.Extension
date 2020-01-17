using Dapper;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static Dapper.SqlMapperExtension;

namespace Kogel.Dapper.Extension.Extension
{
	/// <summary>
	/// 导航属性扩展
	/// </summary>
	public static class NavigationExtension
	{
		/// <summary>
		/// 获取导航属性的类型列表
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static Type[] GetNavigationTypes<T>(SqlProvider provider)
		{
			List<Type> typeList = new List<Type>() { typeof(T) };
			for (var i = 0; i < 6; i++)
			{
				if (provider.JoinList.Count > i)
					typeList.Add(provider.JoinList[i].IsDto ? provider.JoinList[i].DtoType : provider.JoinList[i].TableType);
				else
					typeList.Add(typeof(DontMap));
			}
			return typeList.ToArray();
		}

		/// <summary>
		/// 单个返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static T QueryFirstOrDefaults<T>(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction)
		{
			if (provider.JoinList.Any(x => x.Action == JoinAction.Navigation && x.IsMapperField))
				return (T)typeof(SqlMapperExtension)
					.GetMethod("QueryFirstOrDefault")
					.MakeGenericMethod(GetNavigationTypes<T>(provider))
					.Invoke(null, new object[] { dbCon, provider, transaction });
			else
				return dbCon.QueryFirstOrDefault<T>(provider.SqlString, provider.Params, transaction);
		}

		/// <summary>
		/// 单个异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<T> QueryFirstOrDefaultAsyncs<T>(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction)
		{
			if (provider.JoinList.Any(x => x.Action == JoinAction.Navigation && x.IsMapperField))
				return (Task<T>)typeof(SqlMapperExtension)
					   .GetMethod("QueryFirstOrDefaultAsync")
					   .MakeGenericMethod(GetNavigationTypes<T>(provider))
					   .Invoke(null, new object[] { dbCon, provider, transaction });
			else
				return dbCon.QueryFirstOrDefaultAsync<T>(provider.SqlString, provider.Params, transaction);
		}

		/// <summary>
		/// 列表返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static IEnumerable<T> Querys<T>(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction)
		{
			if (provider.JoinList.Any(x => x.Action == JoinAction.Navigation && x.IsMapperField))
				return (IEnumerable<T>)typeof(SqlMapperExtension)
					   .GetMethod("Query")
					   .MakeGenericMethod(GetNavigationTypes<T>(provider))
					   .Invoke(null, new object[] { dbCon, provider, transaction });
			else
				return dbCon.Query<T>(provider.SqlString, provider.Params, transaction);
		}

		/// <summary>
		/// 列表异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<IEnumerable<T>> QueryAsyncs<T>(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction)
		{
			if (provider.JoinList.Any(x => x.Action == JoinAction.Navigation && x.IsMapperField))
				return (Task<IEnumerable<T>>)typeof(SqlMapperExtension)
					   .GetMethod("QueryAsync")
					   .MakeGenericMethod(GetNavigationTypes<T>(provider))
					   .Invoke(null, new object[] { dbCon, provider, transaction });
			else
				return dbCon.QueryAsync<T>(provider.SqlString, provider.Params, transaction);
		}

		/// <summary>
		/// DataReader返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static GridReader QueryMultiples<T>(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction)
		{
			return dbCon.QueryMultiple(provider.SqlString, provider.Params, transaction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static T QuerySingles<T>(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction)
		{
			return dbCon.QuerySingleOrDefault<T>(provider.SqlString, provider.Params, transaction);
		}

		/// <summary>
		/// 数据集返回
		/// </summary>
		/// <param name="dbCon"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <param name="dataAdapter"></param>
		/// <returns></returns>
		public static DataSet QueryDataSets(this IDbConnection dbCon, SqlProvider provider, IDbTransaction transaction, IDbDataAdapter dataAdapter)
		{
			if (dataAdapter == null)
				dataAdapter = new SqlDataAdapter();
			return dbCon.QueryDataSet(dataAdapter, provider.SqlString, provider.Params, transaction);
		}
	}
}
