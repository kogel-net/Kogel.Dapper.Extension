using Kogel.Dapper.Extension.Extension.From;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 多表查询扩展
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public partial class QuerySet<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public ISelectFrom<T, T1, T2> From<T1, T2>()
        {
            return new ISelectFrom<T, T1, T2>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        public ISelectFrom<T, T1, T2, T3> From<T1, T2, T3>()
        {
            return new ISelectFrom<T, T1, T2, T3>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <returns></returns>
        public ISelectFrom<T, T1, T2, T3, T4> From<T1, T2, T3, T4>()
        {
            return new ISelectFrom<T, T1, T2, T3, T4>(this);
        }
    }
}
