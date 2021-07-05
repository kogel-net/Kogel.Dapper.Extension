using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 条件筛选
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QuerySet<T> : Aggregation<T>, IQuerySet<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQuerySet<T> Where(Expression<Func<T, bool>> predicate)
        {
            WhereExpressionList.Add(predicate);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQuerySet<T> Where<TWhere>(Expression<Func<TWhere, bool>> predicate)
        {
            WhereExpressionList.Add(predicate);
            return this;
        }

        /// <summary>
        /// 动态化查讯(转换成表达式树集合)  注意，int参数不会判断为0的值
        /// </summary>
        /// <param name="dynamicTree"></param>
        /// <returns></returns>
        public IQuerySet<T> Where(Dictionary<string, DynamicTree> dynamicTree)
        {
            WhereExpressionList.AddRange(SqlProvider.FormatDynamicTreeWhereExpression<T>(dynamicTree));
            return this;
        }

        /// <summary>
        /// 使用sql查询条件
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IQuerySet<T> Where(string sqlWhere, object param = null)
        {
            WhereBuilder.Append(" AND " + sqlWhere);
            if (param != null)
            {
                Params.AddDynamicParams(param, true);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWhere1"></typeparam>
        /// <typeparam name="TWhere2"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public IQuerySet<T> Where<TWhere1, TWhere2>(Expression<Func<TWhere1, TWhere2, bool>> exp)
        {
            WhereExpressionList.Add(exp);
            return this;
        }

        /// <summary>
        /// 带前置条件的Where判断
        /// </summary>
        /// <param name="where"></param>
        /// <param name="truePredicate"></param>
        /// <param name="falsePredicate"></param>
        /// <returns></returns>
        public IQuerySet<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate)
        {
            if (where)
                WhereExpressionList.Add(truePredicate);
            else
                WhereExpressionList.Add(falsePredicate);
            return this;
        }

        /// <summary>
        /// 带前置条件的Where判断
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="where"></param>
        /// <param name="truePredicate"></param>
        /// <param name="falsePredicate"></param>
        /// <returns></returns>
        public IQuerySet<T> WhereIf<TWhere>(bool where, Expression<Func<TWhere, bool>> truePredicate, Expression<Func<TWhere, bool>> falsePredicate)
        {
            if (where)
                WhereExpressionList.Add(truePredicate);
            else
                WhereExpressionList.Add(falsePredicate);
            return this;
        }
    }
}
