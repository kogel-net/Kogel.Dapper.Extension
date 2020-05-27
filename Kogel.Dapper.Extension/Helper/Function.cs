using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension
{
    /// <summary>
    /// 函数列表
    /// </summary>
    public static class Function
    {
        #region 聚合函数
        public static int Count<T>(T countExpression)
        {
            return 0;
        }

        public static T Sum<T>(T sumExpression)
        {
            return default(T);
        }

        public static T Max<T>(T maxExpression)
        {
            return default(T);
        }

        public static T Min<T>(T minExpression)
        {
            return default(T);
        }

        public static T Avg<T>(T avgExpression)
        {
            return default(T);
        }
        #endregion

        #region 基础函数
        /// <summary>
        /// 拼接（oracle使用，mssql可以直接使用+号）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static T Concact<T>(T left, T right)
        {
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static T IfNull<T>(T left, T right)
        {
            return default(T);
        }
        #endregion
    }
}
