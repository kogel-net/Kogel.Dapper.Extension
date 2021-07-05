using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Repository.Entites
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionPool
    {
        /// <summary>
        /// 
        /// </summary>
        public Func<IDbConnection, IDbConnection> FuncConnection { get; set; }

        /// <summary>
        /// 连接字符串(内部判断使用)
        /// </summary>
        internal string ConnectionString { get; set; }

        /// <summary>
        /// 数据库名称（自定义标识）
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 数据库名称（实际名称）
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 数据源地址
        /// </summary>
        public string DataSource { get; set; }
    }
}
