using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kogel.Dapper.Core.Test.Models;
using System.Data;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Core.Test.Models.Model;

namespace Kogel.Dapper.Core.Test.Controllers
{
	public class HomeController : Controller
	{
		IDbConnection dbConnection;
		public HomeController(IDbConnection dbConnection)
		{
			this.dbConnection = dbConnection;
		}
		public IActionResult Index()
		{
			var list = dbConnection.QuerySet<Product>()
				//.Where(x=>x.)
				.ToList();
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
