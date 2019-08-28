using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.Test.Model;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mssql
{
    public class Query
    {
        string mssqlConnection = "Data Source=42.157.195.21,4344;Initial Catalog=Qx_Sport_Common;User ID=qxdev;Password=qxdev123456;";
        public void Test()
        {
            using (var conn = new SqlConnection(mssqlConnection))
            {
                //单个属性返回
                var ContentList = conn.QuerySet<Comment>()
                     .AsTableName(typeof(Comment), "Comment_4")
                     .Where(x => x.Id > 0)
                     .ToList(x => x.Content);
                //单条记录
                var commne = conn.QuerySet<Comment>()
                    .AsTableName(typeof(Comment), "Comment_4")
                    .Where(x => x.Id > 0)
                    .Get();

                //翻页
                var comment1 = conn.QuerySet<Comment>()
                    .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                    .Where(x => x.Id.Between(80, 100)
                    && x.SubTime.AddDays(-10) < DateTime.Now && x.Id > 10
                    && x.Id > new QuerySet<News>(conn, new MsSqlProvider()).Where(y => y.Id < 3 && x.Id < y.Id).Sum<News>(y => y.Id))
                    .From<Comment, News>()
                    .OrderBy<News>(x => x.Id)
                    .PageList(1, 1, (a, b) => new
                    {
                        test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y => y == 1),
                        aaa = "6666" + "777",
                        Content = a.Content + "'test'" + b.Headlines + a.IdentityId,
                        bbb = new QuerySet<Comment>(conn, new MsSqlProvider())
                                    .Where(y => y.ArticleId == b.Id && y.Content.Contains("test"))
                                    .Sum<Comment>(x => x.Id),
                        ccc = a.IdentityId,
                        ddd = Convert.ToInt32("(select count(1) from Comment)"),
                        a.Id,
                        cccTime = DateTime.Now
                    });

                //计总
                var sum = conn.QuerySet<Comment>()
                       .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                       .Where(x => x.Content == "test")
                       .WithNoLock()
                       .Sum<News>(x => x.Id);
                //计数
                var count = conn.QuerySet<Comment>()
                     .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                     .Where(x => x.Content == "test1")
                     .Where<News>(x => x.NewsLabel.Contains("足球"))
                     .WithNoLock()
                     .Count();
            }
        }
    }
}
