using Kogel.Dapper.Extension.Oracle;
using Kogel.Dapper.Extension.Test.Model;
using Oracle.ManagedDataAccess.Client;
using Kogel.Dapper.Extension.Oracle.Extension;

namespace Kogel.Dapper.Extension.Test.UnitTest.Oracle
{
    public class Query
    {

        public void Test()
        {

            using (var conn = new OracleConnection("user id=adonis;password=adonis;data source=192.168.0.120:1521/helowin; Pooling=false;"))
            {
                //EntityCache.Register(new System.Type[] { typeof(FlowOrder), typeof(HeadOrder), typeof(WarehouseOrder) });
                //var codeFirst = new CodeFirst(conn);
                //codeFirst.SyncStructure();

                var flowOrders = conn.QuerySet<FlowOrder>()
                    .Where(x => x.CustomerCode == "test")
                    .Top(2)
                    .ToList();
            }

        }
    }
}
