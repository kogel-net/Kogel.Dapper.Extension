using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using System.Text;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Order<T> : Option<T>, IOrder<T>
    {
        protected Order(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {
            OrderbyExpressionList = new Dictionary<LambdaExpression, EOrderBy>();
            OrderbyBuilder = new StringBuilder(); 
        }

        protected Order(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {
            OrderbyExpressionList = new Dictionary<LambdaExpression, EOrderBy>();
            OrderbyBuilder = new StringBuilder();
        }

        /// <inheritdoc />
        public virtual Order<T> OrderBy<TProperty>(Expression<Func<TProperty, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Asc);

            return this;
        }

        public Order<T> OrderBy(Expression<Func<T, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Asc);

            return this;
        }

        /// <inheritdoc />
        public virtual Order<T> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Desc);

            return this;
        }

        public Order<T> OrderByDescing(Expression<Func<T, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Desc);

            return this;
        }
        public Order<T> OrderBy(string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
                OrderbyBuilder.Append(orderBy);

            return this;
        }
    }
}
