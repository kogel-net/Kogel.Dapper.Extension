using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using Kogel.Dapper.Extension.Exception;

namespace Kogel.Dapper.Extension
{
    internal class DbContextConnection
    {
        private DbContextOptionsBuilder builder { get; }
        public DbContextConnection(DbContextOptionsBuilder builder)
        {
            this.builder = builder;
        }
        public IDbConnection CreateConnection()
        {
            IDbConnection dbConnection = null;
            switch (builder.provider.GetType().FullName)
            {
                case "Kogel.Dapper.Extension.MsSql":
                    dbConnection = new SqlConnection(builder.connectionString);
                    break;
                case "Kogel.Dapper.Extension.MySql":
                    dbConnection = new MySqlConnection(builder.connectionString);
                    break;
                case "Kogel.Dapper.Extension.Oracle":
                    dbConnection = new OracleConnection(builder.connectionString);
                    break;
                default:
                    throw new DapperExtensionException("没有找到对应的数据库!");
            }
            return dbConnection;
        }
    }
}
