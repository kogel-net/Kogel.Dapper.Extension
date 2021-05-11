using System.Data;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;

namespace Kogel.Dapper.Extension.MsSql
{
	public static class DataBase
	{
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static IQuerySet<T> QuerySet<T>(this IDbConnection sqlConnection)
        {
            return QuerySet<T>(sqlConnection, null);
        }

        /// <summary>
        /// 查询(带事务)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public static IQuerySet<T> QuerySet<T>(this IDbConnection sqlConnection, IDbTransaction dbTransaction)
        {
            return new QuerySet<T>(sqlConnection, new MsSqlProvider(), dbTransaction);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static ICommandSet<T> CommandSet<T>(this IDbConnection sqlConnection)
        {
            return CommandSet<T>(sqlConnection);
        }

        /// <summary>
        /// 编辑(带事务)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public static ICommandSet<T> CommandSet<T>(this IDbConnection sqlConnection, IDbTransaction dbTransaction)
        {
            return new CommandSet<T>(sqlConnection, new MsSqlProvider(), dbTransaction);
        }
    }
}
