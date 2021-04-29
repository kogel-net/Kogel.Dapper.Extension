using Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository;
using Kogel.Repository;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
    class Command
    {
        public void Test()
        {
            //正常模式下仓储使用
            using (var repository = new FlowOrderRepository())
            {
                repository.UnitOfWork.BeginTransaction(() =>
                {

                    var flowList = repository.QuerySet()
                           .NotUnitOfWork()
                           .ToList();

                    var count = repository.Insert(flowList);
                });

                repository.UnitOfWork.Rollback();
            }
        }
    }
}
