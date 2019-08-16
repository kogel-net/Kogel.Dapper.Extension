using Kogel.Dapper.Extension;

namespace Kogel.Dapper.Extension
{
    public class DbContextOptionsBuilder
    {
        internal string connectionString { get; private set; }
        internal SqlProvider provider { get; private set; }
        /// <summary>
        /// 配置字符串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DbContextOptionsBuilder UseConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            return this;
        }
        /// <summary>
        /// 配置连接方式
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public DbContextOptionsBuilder UseSqlProvider(SqlProvider provider)
        {
            this.provider = provider;
            return this;
        }
    }
}
