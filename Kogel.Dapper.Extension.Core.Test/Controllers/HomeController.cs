using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kogel.Dapper.Extension.Core.Test.Models;
using System.Data;
using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.MsSql;

namespace Kogel.Dapper.Extension.Core.Test.Controllers
{
    public class HomeController : Controller
    {
        private IDbConnection conn;
        public HomeController(IDbConnection conn)
        {
            this.conn = conn;
        }
        public IActionResult Index()
        {
            var commentList = conn.QuerySet<News>().Where(x => x.Id.Between(1, 100)).ToList();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
