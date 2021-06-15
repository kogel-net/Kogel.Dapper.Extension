using Kogel.Dapper.Extension.Oracle;
using Kogel.Dapper.Extension.Test.Model;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using System.Linq;

namespace Kogel.Dapper.Extension.Test.UnitTest.Oracle
{
    public class Command
    {

        public void Test()
        {
            using (var conn = new OracleConnection("user id=adonis;password=adonis;data source=192.168.0.120:1521/helowin; Pooling=false;"))
            {

                var flowOrder = new FlowOrder
                {
                    AccountCode = "test",
                    BusinessType = "so",
                    OrderNumber = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    CustomerCode = "test",
                    DeliveredReceiveTime = DateTime.Now,
                    DeliveredStatus = 1,
                    DeliveredTime = DateTime.Now,
                    OrderCreateTime = DateTime.Now,
                    ReferenceNumber = "testref",
                    Source = 1,
                    UpdateTime = DateTime.Now
                };
                var result = conn.CommandSet<FlowOrder>().Insert(flowOrder);


                //批量插入
                BatchInsert(conn);
            }

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
                UpdateTime = DateTime.Now
            };

            int total = 10000;
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
