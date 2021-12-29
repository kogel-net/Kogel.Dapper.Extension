using System;
using System.Data;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Entites;
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
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static IQuerySet<T> QuerySet<T>(this IDbConnection sqlConnection)
        {
            return new QuerySet<T>(sqlConnection, new OracleSqlProvider());
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
            return new QuerySet<T>(sqlConnection, new OracleSqlProvider(), dbTransaction);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static ICommandSet<T> CommandSet<T>(this IDbConnection sqlConnection)
        {
            return new CommandSet<T>(sqlConnection, new OracleSqlProvider());
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
            return new CommandSet<T>(sqlConnection, new OracleSqlProvider(), dbTransaction);
        }
    }
}
