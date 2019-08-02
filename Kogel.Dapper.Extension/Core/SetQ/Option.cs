using System;
using System.Data;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.Interfaces;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Option<T> : Query<T>, IOption<T>
    {
        protected Option(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {

        }

        protected Option(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {

        }

        public int? TopNum { get; set; }

        /// <inheritdoc />
        //public virtual Query<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        //{
        //    SelectExpression = selector;

        //    var currentQuerySet = (QuerySet<T>)this;

        //    return new QuerySet<TResult>(DbCon, SqlProvider, typeof(T), currentQuerySet.WhereExpression, currentQuerySet.SelectExpression, currentQuerySet.TopNum, currentQuerySet.OrderbyExpressionList, DbTransaction, currentQuerySet.NoLock);
        //}

        /// <inheritdoc />
        public virtual Option<T> Top(int num)
        {
            TopNum = num;
            return this;
        }
    }
}
