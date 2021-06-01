using Kogel.Dapper.Extension.Extension.From;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    /// <summary>
    /// 多表查询扩展
    /// </summary>
    public partial interface IQuerySet<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        IQuery<T, TReturn> Select<TReturn>(Expression<Func<T, TReturn>> select);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        ISelectFrom<T, T1, T2> From<T1, T2>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        ISelectFrom<T, T1, T2, T3> From<T1, T2, T3>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <returns></returns>
        ISelectFrom<T, T1, T2, T3, T4> From<T1, T2, T3, T4>();
    }
}
