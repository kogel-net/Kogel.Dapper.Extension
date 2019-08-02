using System;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.SetQ;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface IOrder<T>
    {
        /// <summary>
        /// 顺序
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        Order<T> OrderBy(Expression<Func<T, object>> field);
        /// <summary>
        /// 倒序
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        Order<T> OrderByDescing(Expression<Func<T, object>> field);
        /// <summary>
        /// 顺序
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        Order<T> OrderBy<TProperty>(Expression<Func<TProperty, object>> field);

        /// <summary>
        /// 倒叙
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        Order<T> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field);
    }
}
