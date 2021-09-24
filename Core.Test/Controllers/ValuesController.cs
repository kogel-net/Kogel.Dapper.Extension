using Core.Test.Model;
using Kogel.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;

namespace Core.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IRepository<FlowOrder> repository;
        public ValuesController(IRepository<FlowOrder> repository)
        {
            this.repository = repository;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<object> Get()
        {
            var flowOrder = repository.QuerySet()
                .Top(100)
                .Distinct()
                .ToList(x => new
                {
                    x.CustomerCode
                });
            return flowOrder;
        }
    }
}
