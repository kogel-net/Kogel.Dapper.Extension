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
        /// 顺序
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        IQuerySet<T> OrderBy(Expression<Func<T, object>> field);

        /// <summary>
        /// 倒序
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        IQuerySet<T> OrderByDescing(Expression<Func<T, object>> field);

        /// <summary>
        /// 顺序
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        IQuerySet<T> OrderBy<TProperty>(Expression<Func<TProperty, object>> field);

        /// <summary>
        /// 倒叙
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        IQuerySet<T> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field);

        /// <summary>
        /// 字符串拼接排序
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IQuerySet<T> OrderBy(string orderBy);
    }
}
