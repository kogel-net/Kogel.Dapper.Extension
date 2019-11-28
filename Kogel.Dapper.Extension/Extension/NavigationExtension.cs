using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static T QueryFirstOrDefaults<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction, IProviderOption providerOption)
		{
			return DbCon.QueryFirstOrDefault<T>(sql, param, transaction);
		}
		/// <summary>
		/// 单个异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<T> QueryFirstOrDefaultAsyncs<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
		}
		/// <summary>
		/// 列表返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static IEnumerable<T> Querys<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction, IProviderOption providerOption)
		{
			return DbCon.Query<T>(sql, param, transaction);
		}
		/// <summary>
		/// 列表异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<IEnumerable<T>> QueryAsyncs<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryAsync<T>(sql, param, transaction);
		}


		public static GridReader QueryMultiples<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryMultiple(sql, param, transaction);
		}

		public static T QuerySingles<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QuerySingleOrDefault<T>(sql, param, transaction);
		}
	}
}
