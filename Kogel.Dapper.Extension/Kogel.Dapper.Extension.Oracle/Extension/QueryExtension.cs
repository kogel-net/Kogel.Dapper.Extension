using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.SetQ;

namespace Kogel.Dapper.Extension.Oracle.Extension
{
    public static class QueryExtension
    {
        //oracle不支持返回多视图对象
        /* public static List<T> UpdateSelect<T>(this Query<T> query, Expression<Func<T, T>> updator)
         {
             var sqlProvider = query.SqlProvider;
             var dbCon = query.DbCon;
             var dbTransaction = query.DbTransaction;
             sqlProvider.FormatUpdateSelect(updator);

             return dbCon.Query<T>(sqlProvider.SqlString, sqlProvider.Params, dbTransaction).ToList();
         }

         public static async Task<IEnumerable<T>> UpdateSelectAsync<T>(this Query<T> query, Expression<Func<T, T>> updator)
         {
             var sqlProvider = query.SqlProvider;
             var dbCon = query.DbCon;
             var dbTransaction = query.DbTransaction;
             sqlProvider.FormatUpdateSelect(updator);

             return await dbCon.QueryAsync<T>(sqlProvider.SqlString, sqlProvider.Params, dbTransaction);
         }
         */
    }
}
