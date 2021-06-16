using Dapper;
using Kogel.Dapper.Extension.MySql.Extension;
using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;
using Kogel.Repository.Interfaces;
using System;
using MySql.Data.MySqlClient;
using Kogel.Repository;
using Kogel.Dapper.Extension.MySql;
using Kogel.Dapper.Extension.Test.Model.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Kogel.Dapper.Extension.Model;

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
                //Console.WriteLine(sql);
#endif
            };


            //正常模式下仓储使用
            using (var repository = new FlowOrderRepository())
            {
                repository.UnitOfWork.BeginTransaction(() =>
                {
                    var aaa = (from t in repository.Orm.QuerySet<FlowOrder>() where t.CustomerCode == "test" select new { t.Id, t.OrderNumber, t.CustomerCode }).ToList();


                    var flowOrder = repository.QuerySet()
                      .ResetTableName(typeof(FlowOrder), "flow_order_1")
                      //.Where(x => x.DeliveredTime.HasValue && x.CustomerCode == "test")
                      .WhereIf(false, x => x.CustomerCode == "test", x => 1 == 1)
                      .From<FlowOrder, FlowOrder>()
                      .Where((a, b) => a.Id == 1)
                      .GetQuerySet()
                      .Get(x => new
                      {
                          x.Id,
                          x.OrderNumber,
                          x.UpdateTime
                      });

                    //Convert.ToInt32("sss");



                    //切换数据库
                    repository.ChangeDataBase("fps_2021");

                    var gc_Fps_FlowOrder = repository.QuerySet()
                        .ResetTableName(typeof(FlowOrder), "flow_order_1")
                        .ResetTableName(typeof(WarehouseOrder), "warehouseorder_1")
                        .Where(x => x.DeliveredTime.HasValue && x.CustomerCode.StartsWith("test"))
                        .Join<FlowOrder, WarehouseOrder>((a, b) => a.OrderNumber == b.OrderNumber, JoinMode.LEFT, false)
                        .Top(10)
                        .OrderBy("")
                        .ToList();

                    repository.ChangeDataBase("master");

                    gc_Fps_FlowOrder = repository.QuerySet()
                      .ResetTableName(typeof(FlowOrder), "flow_order_1")
                      .Where(x => x.DeliveredTime.HasValue && x.CustomerCode == "test")
                      .Top(10)
                      .ToList();

                }, System.Data.IsolationLevel.RepeatableRead);

                repository.UnitOfWork.Rollback();
            }

            var conn = new MySqlConnection("server=192.168.0.120;port=3306;user id=root;password=123456;database=gc_fps_receivable;");
            //自定义仓储释放时 conn也会释放
            using (var divReposirory = conn.QuerySet<FlowOrder>().GetRepository())
            {
                //使用自定义仓储
                var flowOrder = divReposirory.FindById(4);


                var tupleList = divReposirory.QuerySet()
                         .ToListAsync(x => new
                         {
                             Id = x.Id, 
                             dateTime = x.DeliveredReceiveTime
                         }).Result;

            }

        }
    }
}
