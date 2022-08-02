using Kogel.Dapper.Extension;
using System;
using System.Linq.Expressions;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface IAggregation<T>
    {
        /// <summary>
        /// 行数
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 行数
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();
        /// <summary>
        /// 总和
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sumExpression"></param>
        /// <returns></returns>
        TResult Sum<TResult>(Expression<Func<T, TResult>> sumExpression);
        /// <summary>
        /// 总和
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sumExpression"></param>
        /// <returns></returns>
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> sumExpression);
        /// <summary>
        /// 最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="minExpression"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<T, TResult>> minExpression);
        /// <summary>
        /// 最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="minExpression"></param>
        /// <returns></returns>
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> minExpression);
        /// <summary>
        /// 最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="maxExpression"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<T, TResult>> maxExpression);
        /// <summary>
        /// 最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="maxExpression"></param>
        /// <returns></returns>
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> maxExpression);
    }
}
