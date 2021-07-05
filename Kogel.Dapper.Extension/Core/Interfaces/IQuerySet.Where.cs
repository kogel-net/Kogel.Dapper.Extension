using Kogel.Dapper.Extension.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public partial interface IQuerySet<T>
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQuerySet<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQuerySet<T> Where<TWhere>(Expression<Func<TWhere, bool>> predicate);

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="dynamicTree"></param>
        /// <returns></returns>
        IQuerySet<T> Where(Dictionary<string, DynamicTree> dynamicTree);

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IQuerySet<T> Where(string sqlWhere, object param = null);

        /// <summary>
        /// 带前置条件的Where判断
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="where"></param>
        /// <param name="truePredicate"></param>
        /// <param name="falsePredicate"></param>
        /// <returns></returns>
        IQuerySet<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate);

        /// <summary>
        /// 带前置条件的Where判断
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="where"></param>
        /// <param name="truePredicate"></param>
        /// <param name="falsePredicate"></param>
        /// <returns></returns>
        IQuerySet<T> WhereIf<TWhere>(bool where, Expression<Func<TWhere, bool>> truePredicate, Expression<Func<TWhere, bool>> falsePredicate);
    }
}
