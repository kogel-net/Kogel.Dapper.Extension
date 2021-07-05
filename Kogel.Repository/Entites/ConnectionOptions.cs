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
    public class ConnectionOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 是否是主链接
        /// </summary>
        public bool IsCurrent { get; set; }
    }
}
