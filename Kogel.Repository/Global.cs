using Kogel.Repository.Entites;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Repository
{
    /// <summary>
    /// 全局设置
    /// </summary>
    internal static class Global
    {
        /// <summary>
        /// 初始化
        /// </summary>
        static Global()
        {
            ConnectionPool = new ConcurrentBag<ConnectionPool>();
            ProviderPool = new ConcurrentBag<ProviderPool>();
        }

        /// <summary>
        /// 总连接池 (所有连接都会注册到总连接池中，总连接池中不区分主从)
        /// </summary>
        internal static ConcurrentBag<ConnectionPool> ConnectionPool { get; }

        /// <summary>
        /// 解析方池（总连接池中不区分主从）
        /// </summary>
        internal static ConcurrentBag<ProviderPool> ProviderPool { get; }
    }
}
