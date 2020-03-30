using System;
using System.Data;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Oracle.Extension;

namespace Kogel.Dapper.Extension.Oracle
{
    public static class DataBase
    {
		static DataBase()
		{
			//注册bool解析
			SqlMapper.RemoveTypeMap(typeof(bool));
			SqlMapper.AddTypeHandler(typeof(bool), new BoolTypeHanlder());
		}
		/// <summary>
		/// 用来解决表达式树不能使用默认参数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sqlConnection"></param>
		/// <returns></returns>
		public static IQuerySet<T> QuerySet<T>(this IDbConnection sqlConnection)
		{
			return new QuerySet<T>(sqlConnection, new OracleSqlProvider(), null);
		}
		public static IQuerySet<T> QuerySet<T>(this IDbConnection sqlConnection, IDbTransaction dbTransaction = null)
		{
			return new QuerySet<T>(sqlConnection, new OracleSqlProvider(), dbTransaction);
		}
		/// <summary>
		/// 用来解决表达式树不能使用默认参数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sqlConnection"></param>
		/// <returns></returns>
		public static ICommandSet<T> CommandSet<T>(this IDbConnection sqlConnection)
		{
			return new CommandSet<T>(sqlConnection, new OracleSqlProvider(), null);
		}
		public static ICommandSet<T> CommandSet<T>(this IDbConnection sqlConnection, IDbTransaction dbTransaction = null)
		{
			return new CommandSet<T>(sqlConnection, new OracleSqlProvider(), dbTransaction);
		}
	}
}
