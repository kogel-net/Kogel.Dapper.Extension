using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Kogel.Dapper.Extension.Extension
{
	/// <summary>
	/// 导航属性扩展
	/// </summary>
	public static class NavigationExtension
	{
		/// <summary>
		/// 单个返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static T QueryFirstOrDefaults<T>(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction, IProviderOption providerOption)
		{
			return dbCon.QueryFirstOrDefault<T>(sql, param, transaction);
		}
		/// <summary>
		/// 单个异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<T> QueryFirstOrDefaultAsyncs<T>(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction)
		{
			return dbCon.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
		}
		/// <summary>
		/// 列表返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static IEnumerable<T> Querys<T>(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction, IProviderOption providerOption)
		{
			return dbCon.Query<T>(sql, param, transaction);
		}
		/// <summary>
		/// 列表异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<IEnumerable<T>> QueryAsyncs<T>(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction)
		{
			return dbCon.QueryAsync<T>(sql, param, transaction);
		}


		public static GridReader QueryMultiples<T>(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction)
		{
			return dbCon.QueryMultiple(sql, param, transaction);
		}

		public static T QuerySingles<T>(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction)
		{
			return dbCon.QuerySingleOrDefault<T>(sql, param, transaction);
		}
		/// <summary>
		/// 数据集返回
		/// </summary>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <param name="dataAdapter"></param>
		/// <returns></returns>
		public static DataSet QueryDataSets(this IDbConnection dbCon, string sql, object param, IDbTransaction transaction, IDbDataAdapter dataAdapter)
		{
			if (dataAdapter == null)
				dataAdapter = new SqlDataAdapter();
			return dbCon.QueryDataSet(dataAdapter, sql, param, transaction);
		}
	}
}
