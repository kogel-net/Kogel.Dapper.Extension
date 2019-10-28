using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lenic.DI.Core
{
    /// <summary>
    /// 一般相等比较器
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    [DebuggerStepThrough]
    public class EqualComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// 获得当前相等比较器. 
        /// </summary>
        /// <value>当前相等比较器. </value>
        public Func<T, T, bool> Comparer { get; protected set; }

        /// <summary>
        /// 初始化新建一个 <see cref="EqualComparer&lt;T&gt;"/> 类的实例对象. 
        /// </summary>
        /// <param name="comparer">一个相等比较器. </param>
        public EqualComparer(Func<T, T, bool> comparer)
        {
            Comparer = comparer;
        }

        #region IEqualityComparer<T> 成员

        /// <summary>
        /// 确定指定的对象是否相等. 
        /// </summary>
        /// <param name="x">要比较的第一个类型为 <paramref name="T"/> 的对象. </param>
        /// <param name="y">要比较的第二个类型为 <paramref name="T"/> 的对象. </param>
        /// <returns>如果指定的对象相等. 则为 true；否则为 false. </returns>
        public bool Equals(T x, T y)
        {
            return Comparer(x, y);
        }

        /// <summary>
        /// 返回此实例的哈希代码, 这里永远返回数字 0 .
        /// </summary>
        /// <param name="obj">待获得哈希代码的一个实例对象. </param>
        /// <returns>32 位有符号整数哈希代码. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="obj"/> 的类型为引用类型. <paramref name="obj"/> 为 <c>null</c> . </exception>
        public int GetHashCode(T obj)
        {
            return 0;
        }

        #endregion

        #region IEqualityComparer<T> 成员

        /// <summary>
        /// 确定指定的对象是否相等。
        /// </summary>
        /// <param name="x">要比较的第一个类型为 <paramref name="T"/> 的对象。</param>
        /// <param name="y">要比较的第二个类型为 <paramref name="T"/> 的对象。</param>
        /// <returns>如果指定的对象相等，则为 true；否则为 false。</returns>
        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// 返回此实例的哈希代码.
        /// </summary>
        /// <param name="obj">待获得哈希代码的一个实例对象.</param>
        /// <returns>32 位有符号整数哈希代码.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="obj"/> 的类型为引用类型. <paramref name="obj"/> 为 <c>null</c> . </exception>
        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return GetHashCode(obj);
        }

        #endregion
    }
}
