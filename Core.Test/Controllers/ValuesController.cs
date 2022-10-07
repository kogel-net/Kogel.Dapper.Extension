using Core.Test.Model;
using Kogel.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Attributes;
using System.Xml.Linq;
using Kogel.Repository.Interfaces;
using System.Data.SqlClient;

namespace Core.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IRepository<FlowOrder> repository;

        IBaseRepository<Test> repository1;
        IBaseRepository<Test> repository2;
        public ValuesController(IRepository<FlowOrder> repository)
        {
            this.repository = repository;
            //copy第一个仓储对象
            this.repository1 = repository.QuerySet<Test>().GetRepository();
            //并切换到另一个数据源
            this.repository1.ChangeDataBase("master");
            //copy第二个仓储对象
            this.repository2 = repository.QuerySet<Test>().GetRepository();
            //并切换到另一个数据源
            this.repository2.ChangeDataBase("KPcba");


            SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
        }

        private void Aop_OnExecuting(ref CommandDefinition command)
        {

        }

        //// GET api/values
        //[HttpGet]
        //public ActionResult<object> Get()
        //{
        //    var flowOrder = repository.QuerySet()
        //        .Top(100)
        //        .Distinct()
        //        .ToList(x => new
        //        {
        //            x.CustomerCode
        //        });
        //    return flowOrder;
        //}

        [HttpGet]
        public ActionResult<object> TestUnitOfWork()
        {   
            //测试不同数据库中多阶段事务提交
            repository1.UnitOfWork.BeginTransaction(XaBeginMethod);
            //这里会做统一提交
            repository1.UnitOfWork.Rollback();
            return "success";
        }

        public void XaBeginMethod()
        {
            _ = repository1.CommandSet()
                 .Where(x => x.Name == "1")
                 .Update(x => new Test
                 {
                     Name = Function.IfNull(x.Name, "123") + "4"
                 });

            repository1.Insert(new Test { Name = "1" });
            repository2.Insert(new Test { Name = "2" });

            repository2.UnitOfWork.BeginTransaction(() =>
            {
                repository2.Insert(new Test { Name = "3" });
            });
            //因为嵌套工作单元和不是一个db连接的关系，这里是一个伪提交
            repository2.UnitOfWork.Commit();

            //甚至写sql都可以哦，一样会强制进入到事务中
            using (var conn = new SqlConnection("server=192.168.3.9;uid=sa;pwd=ABCabc123;database=XiaoMingMall;"))
            {
                var id = conn.ExecuteScalar<int>("Insert into test(name)values('kogel牛逼!')SELECT @@IDENTITY");
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    [Display(Rename = "test")]
    public class Test
    {
        /// <summary>
        /// 
        /// </summary>
        [Identity]
        [Display(Rename = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Rename = "name")]
        public string Name { get; set; }
    }
}
