using Kogel.Dapper.Extension.Oracle;
using Kogel.Dapper.Extension.Test.Model;
using Oracle.ManagedDataAccess.Client;

namespace Kogel.Dapper.Extension.Test.UnitTest.Oracle
{
    public class Query
    {

        public void Test()
        {

            using (var conn = new OracleConnection("user id=adonis;password=123456;data source=192.168.0.120:1521/helowin; Pooling=false;"))
            {
                var flowOrders = conn.QuerySet<FlowOrder>()
                    .Where(x => x.CustomerCode == "test")
                    .Top(2)
                    .ToList();
            }

        }
    }
}
