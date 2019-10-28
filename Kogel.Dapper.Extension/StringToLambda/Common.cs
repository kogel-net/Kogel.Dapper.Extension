using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Lenic.DI
{
    /// <summary>
    /// 公用扩展类
    /// </summary>
    [DebuggerStepThrough]
    internal static class Common
    {
        /// <summary>
        /// 对 source 的每个元素执行指定操作.
        /// </summary>
        /// <typeparam name="T">source 中的元素类型</typeparam>
        /// <param name="obj">数据源.</param>
        /// <param name="action">需要执行的逻辑方法.</param>
        public static void Foreach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in source)
            {
                action(item, index++);
            }
        }

        /// <summary>
        /// 获取去掉可空类型（Nullable&lt;&gt;）的类型.
        /// </summary>
        /// <param name="type">当前类型的实例对象.</param>
        /// <returns>去掉可空类型（Nullable&lt;&gt;）的类型.</returns>
        public static Type GetNoneNullableType(this Type type)
        {
            if (IsNullable(type))
                return Nullable.GetUnderlyingType(type);
            return type;
        }

        /// <summary>
        /// 获取可空类型（Nullable&lt;&gt;）的类型.
        /// </summary>
        /// <param name="type">当前类型的实例对象.</param>
        /// <returns>可空类型（Nullable&lt;&gt;）的类型.</returns>
        public static Type GetNullableType(this Type type)
        {
            if (!IsNullable(type) && type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            return type;
        }

        /// <summary>
        /// 获取一个值, 通过该值指示当前类型是否为可空类型（Nullable&lt;&gt;）.
        /// </summary>
        /// <param name="type">当前类型的实例对象.</param>
        /// <returns>
        ///   <c>true</c> 表示为可空类型（Nullable&lt;&gt;）; 否则返回 <c>false</c>.
        /// </returns>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
