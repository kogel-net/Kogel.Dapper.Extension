using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
    class Command
    {
        public void Test()
        {
            //正常模式下仓储使用
            using (var repository = new FlowOrderRepository())
            {
                var flowList = repository.QuerySet()
                       .ToList();

               var count= repository.Insert(flowList);

            }
        }
    }
}
