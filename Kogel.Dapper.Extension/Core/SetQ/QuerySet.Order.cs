using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Entites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QuerySet<T> : Aggregation<T>, IQuerySet<T>
    {
        public IQuerySet<T> OrderBy<TProperty>(Expression<Func<TProperty, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Asc);
            return this;
        }

        public IQuerySet<T> OrderBy(Expression<Func<T, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Asc);
            return this;
        }

        /// <inheritdoc />
        public IQuerySet<T> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Desc);
            return this;
        }

        public IQuerySet<T> OrderByDescing(Expression<Func<T, object>> field)
        {
            if (field != null)
                OrderbyExpressionList.Add(field, EOrderBy.Desc);
            return this;
        }

        public IQuerySet<T> OrderBy(string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
                OrderbyBuilder.Append(orderBy);
            return this;
        }
    }
}
