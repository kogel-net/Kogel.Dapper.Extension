using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;
using Kogel.Repository;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
    class Command
    {
        public void Test()
        {
            SqlMapper.Aop.OnExecuting += Aop_OnExecuting;

            //正常模式下仓储使用
            using (var repository = new FlowOrderRepository())
            {
                //repository.UnitOfWork.BeginTransaction(() =>
                //{

                //    var flowList = repository.QuerySet()
                //           .NotUnitOfWork()
                //           .ToList();

                //    var count = repository.Insert(flowList);
                //});

                //repository.UnitOfWork.Rollback();

                repository.ChangeDataBase("fps_test");

                var json = JsonConvert.SerializeObject(repository.Orm.Query("SELECT * FROM customer_pay_info LIMIT 1"));


                var headOrders = repository.QuerySet<HeadOrder>()
                      .Where(x => x.CustomerCode == "G1119")
                      .Top(100)
                      .ToList();

                int index = 0;

                for (var i = 0; i < 50; i++)
                {
                    foreach (var item in headOrders)
                    {
                        item.OrderNumber = Guid.NewGuid().ToString("N");
                    }

                    var result = repository.CommandSet<HeadOrder>()
                          .Insert(headOrders);

                    Console.WriteLine(index);
                }
                 



            }
        }

        private void Aop_OnExecuting(ref CommandDefinition command)
        {

        }
    }
}
