using Dapper;
using Kogel.Dapper.Extension.MySql.Extension;
using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;
using Kogel.Repository.Interfaces;
using System;
using MySql.Data.MySqlClient;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
    public class Query
    {

        public void Test()
        {

            SqlMapper.Aop.OnExecuting += (ref CommandDefinition command) =>
            {
                //生成的sql
                var sql = command.CommandText;
                //生成的参数
                var param = command.Parameters;
#if DEBUG
                Console.WriteLine(sql);
#endif
            };

            using (var repository = new FlowOrderRepository())
            {
                var flowOrder = repository.QuerySet()
                    .AsTableName(typeof(FlowOrder), "flow_order_1")
                    .Where(x => x.DeliveredTime.HasValue && x.CustomerCode == "test")
                    .Get();


                repository.ChangeDataBase("fps_2021");


                var gc_Fps_FlowOrder = repository.QuerySet()
                    .AsTableName(typeof(FlowOrder), "flow_order_1")
                    .Where(x => x.DeliveredTime.HasValue && x.CustomerCode == "test")
                    .Get();
            }



        }



    }
}
