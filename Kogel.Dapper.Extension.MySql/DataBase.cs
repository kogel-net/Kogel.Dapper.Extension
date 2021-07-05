using System;
using System.Data;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Entites;

namespace Kogel.Dapper.Extension.MySql
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
            return new QuerySet<T>(sqlConnection, new MySqlProvider());
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
            return new QuerySet<T>(sqlConnection, new MySqlProvider(), dbTransaction);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static ICommandSet<T> CommandSet<T>(this IDbConnection sqlConnection)
        {
            return new CommandSet<T>(sqlConnection, new MySqlProvider());
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
            return new CommandSet<T>(sqlConnection, new MySqlProvider(), dbTransaction);
        }
    }
}
