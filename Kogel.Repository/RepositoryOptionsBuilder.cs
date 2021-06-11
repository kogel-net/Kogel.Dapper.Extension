using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension;
using System.Data.Common;
using System.Collections.Concurrent;

namespace Kogel.Repository
{
    public class RepositoryOptionsBuilder
    {
        /// <summary>
        /// 总连接池 (所有连接都会注册到总连接池中，总连接池中不区分主从)
        /// </summary>
        internal static readonly ConcurrentBag<ConnectionPool> _connectionPool = new ConcurrentBag<ConnectionPool>();

        /// <summary>
        /// 当前仓储 数据库连接池   1.连接对象 2.是否是主连接对象 3.库名称(切换库时使用)
        /// </summary>
        internal List<ConnectionOptions> CurrentConnectionPool { get; set; } = new List<ConnectionOptions>();

        /// <summary>
        /// 数据提供者
        /// </summary>
        internal SqlProvider Provider { get; private set; }

        /// <summary>
        /// 是否自动同步
        /// </summary>
        internal bool IsAutoSyncStructure { get; private set; }

        /// <summary>
        /// (仓储使用)注册连接方式
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        public RepositoryOptionsBuilder BuildConnection(Func<IDbConnection, IDbConnection> connection, string dbName = "master")
        {
            RegisterDataBase(connection, dbName);
            return this;
        }

        /// <summary>
        /// (全局使用)注册连接方式
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dbName"></param>
        /// <param name="isReplace">是否替换掉之前db连接</param>
        public static void RegisterDataBase(Func<IDbConnection, IDbConnection> connection, string dbName, bool isReplace = false)
        {
            //验证总连接池是否注册过
            //不存在时注入到总连接池中
            if (!_connectionPool.Any(x => x.DbName == dbName))
            {
                //首次测试注册得到连接字符串(防止传空)
                using (var dbConn = connection.Invoke(null))
                {
                    var dbConnection = dbConn as DbConnection;
                    _connectionPool.Add(new ConnectionPool
                    {
                        FuncConnection = connection,
                        DbName = dbName,
                        ConnectionString = dbConnection.ConnectionString,
                        Database = dbConnection.Database,
                        DataSource = dbConnection.DataSource
                    });
                }
            }
            else
            {
                if (isReplace)
                {
                    var connectionPool = _connectionPool.FirstOrDefault(x => x.DbName == dbName);
                    //首次测试注册得到连接字符串(防止传空)
                    using (var dbConn = connection.Invoke(null))
                    {
                        connectionPool.FuncConnection = connection;
                        connectionPool.ConnectionString = dbConn.ConnectionString;
                    }
                }
            }
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        internal ConnectionOptions GetConnection(string dbName = "master")
        {
            lock (CurrentConnectionPool)
            {
                //先从当前仓储链接池中取出
                ConnectionOptions connectionOptions = CurrentConnectionPool.FirstOrDefault(x => x.DbName == dbName);
                if (connectionOptions == null)
                {
                    //通过orm对象获取
                    if (dbName == "Orm")
                        connectionOptions = CurrentConnectionPool.FirstOrDefault(x => x.IsCurrent);

                    if (connectionOptions == null)
                    {
                        //如果不存在从总连接池中生存一个新的连接
                        var connection = _connectionPool.FirstOrDefault(x => (x.DbName == dbName) || (dbName == "Orm" && x.DbName == "master"));
                        if (connection == null && dbName == "Orm")
                        {
                            connection = _connectionPool.FirstOrDefault();
                            if (connection == null)
                                throw new DapperExtensionException($"请在 OnConfiguring 中配置Connection，DbName：{dbName}");

                            var dbConn = connection.FuncConnection.Invoke(null);
                            connectionOptions = new ConnectionOptions
                            {
                                Connection = dbConn,
                                DbName = connection.DbName,
                                IsCurrent = true
                            };
                        }
                        else if (connection == null)
                        {
                            throw new DapperExtensionException($"请在 OnConfiguring 中配置Connection，DbName：{dbName}");
                        }
                        else
                        {
                            var dbConn = connection.FuncConnection.Invoke(null);
                            connectionOptions = new ConnectionOptions
                            {
                                Connection = dbConn,
                                DbName = connection.DbName
                            };
                        }
                        CurrentConnectionPool.Add(connectionOptions);
                    }
                }
                return connectionOptions;
            }
        }

        /// <summary>
        /// 配置数据库提供者
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public RepositoryOptionsBuilder BuildProvider(SqlProvider provider)
        {
            this.Provider = provider;
            return this;
        }

        /// <summary>
        /// 配置是否自动同步实体结构到数据库
        /// </summary>
        /// <param name="IsAutoSyncStructure"></param>
        /// <returns></returns>
        public RepositoryOptionsBuilder BuildAutoSyncStructure(bool IsAutoSyncStructure = false)
        {
            this.IsAutoSyncStructure = IsAutoSyncStructure;
            return this;
        }
    }

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
