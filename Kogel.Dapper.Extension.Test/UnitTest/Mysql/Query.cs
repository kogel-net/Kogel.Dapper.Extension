using Dapper;
using Kogel.Dapper.Extension.MySql.Extension;
using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;
using Kogel.Repository.Interfaces;
using System;
using MySql.Data.MySqlClient;
using Kogel.Repository;
using Kogel.Dapper.Extension.MySql;

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
            //正常模式下仓储使用
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

            var conn = new MySqlConnection("server=localhost;port=3306;user id=root;password=A5101264a;database=gc_fps_receivable;");
            //自定义仓储释放时 conn也会释放
            using (var divReposirory = conn.QuerySet<FlowOrder>().GetRepository())
            {
                //使用自定义仓储
                var flowOrder = divReposirory.FindById(1);
            }

        }
    }
}
