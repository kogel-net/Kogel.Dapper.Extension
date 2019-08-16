using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension
{
   public class DbSet<T>
    {
        public QuerySet<T> Select()
        {
            //IDbConnection dbConnection = new DbContextConnection(this.builder)
            //    .CreateConnection();
            //var querySet = new QuerySet<T>(dbConnection, builder.provider);
            //ConnectionList.Add(dbConnection);
            //return querySet
            return null;
        }
        public CommandSet<T> Excute()
        {
            //IDbConnection dbConnection = new DbContextConnection(this.builder)
            //  .CreateConnection();
            //var commandSet = new CommandSet<T>(dbConnection, builder.provider);
            //ConnectionList.Add(dbConnection);
            //return commandSet;
            return null;
        }
    }
}
