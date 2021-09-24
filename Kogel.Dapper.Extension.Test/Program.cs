using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Extension.From;
//using Kogel.Dapper.Extension.MySql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.Entites;
using System.Collections.Concurrent;

namespace Kogel.Dapper.Extension.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            #region mssql单元测试
            var mssqlQuery = new UnitTest.Mssql.Query();
            mssqlQuery.Test();

            //var mssqlCommand = new UnitTest.Mssql.Command();
            //mssqlCommand.Test();
            #endregion

            #region mysql单元测试
            //var mysqlQuery = new UnitTest.Mysql.Query();
            //mysqlQuery.Test();

            //var mysqlCommand = new UnitTest.Mysql.Command();
            //mysqlCommand.Test();
            #endregion

            //stopwatch.Stop();

            //#region oracle单元测试
            //var oracleQuery = new UnitTest.Oracle.Query();
            //oracleQuery.Test();

            //var oracleCommand = new UnitTest.Oracle.Command();
            //oracleCommand.Test();
            //#endregion
        }
    }
}
