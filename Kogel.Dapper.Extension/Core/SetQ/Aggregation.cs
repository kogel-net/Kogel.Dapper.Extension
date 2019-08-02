using System;
using System.Data;
using System.Linq.Expressions;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 聚合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Aggregation<T> : Order<T>, IAggregation<T>
    {
        protected Aggregation(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {

        }

        protected Aggregation(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {

        }

        /// <inheritdoc />
        public int Count()
        {
            SqlProvider.FormatCount();
            return DbCon.QuerySingle<int>(SqlProvider.SqlString, SqlProvider.Params);
        }

        /// <inheritdoc />
        public int Sum<TResult>(Expression<Func<TResult, object>> sumExpression)
        {
            SqlProvider.FormatSum<TResult>(sumExpression);
            return DbCon.QuerySingle<int>(SqlProvider.SqlString, SqlProvider.Params);
        }
    }
}
