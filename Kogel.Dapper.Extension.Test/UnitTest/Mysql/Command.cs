using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;
using Kogel.Repository;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;
using System;
using Newtonsoft.Json;
using Kogel.Dapper.Extension.MySql.Extension;
using Kogel.Dapper.Extension.MySql;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;

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
                //    repository.UnitOfWork.BeginTransaction(() =>
                //    {
                //        //var flowList = repository.QuerySet()
                //        //       .Top(10)
                //        //       .ToList();

                //        //var count = repository.Insert(flowList);

                //        //BatchInsert(repository.Orm);

                //    });

                //    repository.UnitOfWork.Commit();

                // var json = JsonConvert.SerializeObject(repository.Orm.Query("SELECT * FROM customer_pay_info LIMIT 1"));


                var flowOrders = repository.QuerySet<FlowOrder>()
                      //.Where(x => x.CustomerCode == "G1119")
                      .OrderBy(x => x.Id)
                      .Top(100)
                      .ToList();

                int index = 0;

                //for (var i = 0; i < 50; i++)
                //{



                //    Console.WriteLine(index);
                //}

                foreach (var item in flowOrders)
                {
                    item.UpdateTime = DateTime.Now;
                    //item.OrderNumber = Guid.NewGuid().ToString("N");

                    item.DeliveredTime = DateTime.Now;


                }

                // List<Task> tasks = new List<Task>();

                // tasks.Add(Task.Run(async () =>
                //{
                //    var result1 = await repository.CommandSet<FlowOrder>()
                //      .InsertAsync(flowOrders);
                //    Console.WriteLine(result1);
                //}));

                // Task.WaitAll(tasks.ToArray());


                var result = repository.CommandSet<FlowOrder>()
                     .Update(flowOrders, new MySqlDataAdapter(), new string[] { "reference_number" });

                var testData = flowOrders.FirstOrDefault();

                var updateFlowOrder = repository.CommandSet<FlowOrder>()
                    .Where(x => x.Id.Between(4, 5))
                    .UpdateSelect(x => new FlowOrder
                    {
                        Id = x.Id,
                        AccountCode = x.AccountCode
                    });



                result = repository.CommandSet<FlowOrder>()
                   .Delete(testData);


                var newId = repository.CommandSet<FlowOrder>()
                .InsertIdentity(testData);
            }
        }

        private void Aop_OnExecuting(ref CommandDefinition command)
        {

        }

        //测试批量插入10w条数据
        private void BatchInsert(IDbConnection conn)
        {
            var flowOrders = new FlowOrder
            {
                AccountCode = "test",
                BusinessType = "so",
                CreateTime = DateTime.Now,
                CustomerCode = "test",
                DeliveredReceiveTime = DateTime.Now,
                DeliveredStatus = 1,
                DeliveredTime = DateTime.Now,
                OrderCreateTime = DateTime.Now,
                ReferenceNumber = "testref",
                Source = 1,
            };

            int total = 100000;
            List<FlowOrder> flows = new List<FlowOrder>();
            for (var i = 0; i < total; i++)
            {
                flowOrders.OrderNumber = $"{Guid.NewGuid()}_{i + 1}";
                flows.Add(flowOrders);
            }

            int pageSize = 500;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //分100次插入
            for (var i = 0; i < total / pageSize; i++)
            {
                Console.WriteLine(i + 1);
                var data = flows.Skip(i * pageSize).Take(pageSize);
                conn.CommandSet<FlowOrder>()
                    .Insert(data);
            }
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
        }
    }
}
