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
            repository1.UnitOfWork.Commit();
            return "success";
        }

        public void XaBeginMethod()
        {
            repository1.Insert(new Test { Name = "1" });
            repository2.Insert(new Test { Name = "2" });

            repository2.UnitOfWork.BeginTransaction(() =>
            {
                repository2.Insert(new Test { Name = "3" });
            });
            //因为嵌套工作单元和不是一个db连接的关系，这里是一个伪提交
            repository2.UnitOfWork.Commit();
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
