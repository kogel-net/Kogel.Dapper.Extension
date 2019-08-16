using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;
using System.Data;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Core.SetC;

namespace Kogel.Dapper.Extension
{
    public class DbContext : IDisposable
    {
        public DbContext()
        {
            ConnectionList = new List<IDbConnection>();
        }
        /// <summary>
        /// 集体释放数据库连接
        /// </summary>
        public void Dispose()
        {
            foreach (var conn in ConnectionList)
            {
                conn.Dispose();
            }
        }
        private DbContextOptionsBuilder builder { get; set; }
        private List<IDbConnection> ConnectionList { get; set; }
        /// <summary>
        /// 配置连接
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void OnConfiguring(DbContextOptionsBuilder builder)
        {
            this.builder = builder;
        }
    }
}
