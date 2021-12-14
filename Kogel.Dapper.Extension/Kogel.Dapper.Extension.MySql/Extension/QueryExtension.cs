using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetC;

namespace Kogel.Dapper.Extension.MySql
{
    public static class QueryExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updator"></param>
        /// <returns></returns>
        public static List<T> UpdateSelect<T>(this ICommandSet<T> query, Expression<Func<T, T>> updator)
        {
            return UpdateSelect(query as CommandSet<T>, updator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updator"></param>
        /// <returns></returns>
        public static List<T> UpdateSelect<T>(this CommandSet<T> query, Expression<Func<T, T>> updator)
        {
            var sqlProvider = query.SqlProvider;
            var dbCon = query.DbCon;
            var dbTransaction = query.DbTransaction;
            sqlProvider.FormatUpdateSelect(updator);
            return dbCon.Query<T>(sqlProvider.SqlString, sqlProvider.Params, dbTransaction).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updator"></param>
        /// <returns></returns>
        public static async Task<List<T>> UpdateSelectAsync<T>(this ICommandSet<T> query, Expression<Func<T, T>> updator)
        {
            return await UpdateSelectAsync(query as CommandSet<T>, updator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updator"></param>
        /// <returns></returns>
        public static async Task<List<T>> UpdateSelectAsync<T>(this CommandSet<T> query, Expression<Func<T, T>> updator)
        {
            var sqlProvider = query.SqlProvider;
            var dbCon = query.DbCon;
            var dbTransaction = query.DbTransaction;
            sqlProvider.FormatUpdateSelect(updator);
            return (await dbCon.QueryAsync<T>(sqlProvider.SqlString, sqlProvider.Params, dbTransaction)).ToList();
        }
    }
}
