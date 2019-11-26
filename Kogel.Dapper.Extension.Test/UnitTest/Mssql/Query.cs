using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.Test.Model;
using Lige.Model;
using Lige.ViewModel.APP.Shopping;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mssql
{
    public class Query
    {
        string mssqlConnection = "Data Source=42.157.195.21,4344;Initial Catalog=Qx_Sport_Common;User ID=qxdev;Password=qxdev123456;";
        public void Test()
        {
			//        using (var conn = new SqlConnection(mssqlConnection))
			//        {
			//var aaa = conn.QuerySet<Comment>().Where(x => 1 == 1).ToList();

			////单个属性返回
			//var ContentList = conn.QuerySet<Comment>()
			//                 .AsTableName(typeof(Comment), "Comment_4")
			//                 .Where(x => x.Id > 0)
			//                 .ToList(x => x.Content);
			//            //单条记录
			//            var commne = conn.QuerySet<Comment>()
			//                .AsTableName(typeof(Comment), "Comment_4")
			//                .Where(x => x.Id > 0)
			//                .Get();

			//            //翻页
			//            var comment1 = conn.QuerySet<Comment>()
			//                .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
			//                .Where(x => x.Id.Between(80, 100)
			//                && x.SubTime.AddDays(-10) < DateTime.Now && x.Id > 10
			//                && x.Id > new QuerySet<News>(conn, new MsSqlProvider()).Where(y => y.Id < 3 && x.Id < y.Id).Sum<News>(y => y.Id))
			//                .From<Comment, News>()
			//                .OrderBy<News>(x => x.Id)
			//                .PageList(1, 1, (a, b) => new
			//                {
			//                    test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y => y == 1),
			//                    aaa = "6666" + "777",
			//                    Content = a.Content + "'test'" + b.Headlines + a.IdentityId,
			//                    bbb = new QuerySet<Comment>(conn, new MsSqlProvider())
			//                                .Where(y => y.ArticleId == b.Id && y.Content.Contains("test"))
			//                                .Sum<Comment>(x => x.Id),
			//                    ccc = a.IdentityId,
			//                    ddd = Convert.ToInt32("(select count(1) from Comment)"),
			//                    a.Id,
			//                    cccTime = DateTime.Now
			//                });

			//            //计总
			//            var sum = conn.QuerySet<Comment>()
			//                   .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
			//                   .Where(x => x.Content == "test")
			//                   .WithNoLock()
			//                   .Sum<News>(x => x.Id);
			//            //计数
			//            var count = conn.QuerySet<Comment>()
			//                 .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
			//                 .Where(x => x.Content == "test1")
			//                 .Where<News>(x => x.NewsLabel.Contains("足球"))
			//                 .WithNoLock()
			//                 .Count();
			//        }
			SqlMapper.Aop.OnExecuting += Aop_OnExecuting;

			using (var connection = new SqlConnection("server=localhost;database=Lige;user=sa;password=!RisingupTech/././.;max pool size=300"))
			{
				var pageList = connection.QuerySet<Lige.Model.Order>()
					.WhereIf(0 != 0, x => x.IsDelete == false && x.Status == 0, x => x.IsDelete == false)
					.OrderByDescing(x => x.CreateDate)
					.PageList(1, 10, x => new OrderResDto()
					{
						Id = x.Id,
						OrderNo = x.OrderNo,
						OrderTime = x.CreateDate,
						Status = x.Status,
						Amount = x.Amount,
						Point = x.Point,
						OrderDetailList = connection.QuerySet<OrderDetail>()
						.Where(y => y.IsDelete == false && y.OrderNo == x.OrderNo)
						.WhereIf(1 == 1, y => y.IsDelete == false && y.OrderNo == x.OrderNo, y => y.IsDelete == false)
						.Join<OrderDetail, Product>((a, b) => a.ProductCode == b.ProductCode, JoinMode.LEFT, true)
						.From<OrderDetail, Product>()
						.OrderBy<Product>(y => y.Id)
						.ToList(true, (a, b) => new OrderDetailResDto()
						{
							Id = a.Id,
							Name = a.ProductName,
							Point = a.Point,
							Price = a.Price,
							Qty = a.Qty,
							OriginalPrice = b.Price,
							OriginalPoint = b.Point,
						}, null)
					});
				var test = connection.QuerySet<Order>()
					.OrderBy(x => x.Id)
					.PageList(1, 10);
			}
		}

		private void Aop_OnExecuting(ref CommandDefinition command)
		{
			
		}
	}
}
