using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.MySql;
using Kogel.Dapper.Extension.Test.Model;
using MySql.Data.MySqlClient;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
   public class Query
    {
        string mysqlConnection = "Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;";
        public void Test()
        {
            using (var conn = new MySqlConnection(mysqlConnection))
            {
                DateTime dateTime = DateTime.Now.AddDays(-10);
                //单个属性返回
                var ContentList = conn.QuerySet<Comment>()
                     .Where(x => x.Id > 0 && x.SubTime > dateTime)
                     .Where(x => x.SubTime > DateTime.Now)
                     .ToList(x => x.Content);

                var commne = conn.QuerySet<Comment>()
                    .Where(x => x.Id > 0)
                    .Get();
                //翻页
                var comment1 = conn.QuerySet<Comment>()
                    .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                    .Where(x => x.Id.Between(80, 100)
                    && x.SubTime.AddDays(-10) < DateTime.Now && x.Id > 10
                    && x.Id > new QuerySet<News>(conn, new MySqlProvider()).Where(y => y.Id < 3 && x.Id < y.Id).Sum<News>(y => y.Id))
                    .From<Comment, News>()
                    .OrderBy<News>(x => x.Id)
                    .PageList(1, 1, (a, b) => new
                    {
                        test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y => y == 1),
                        aaa = "6666" + "777",
                        Content = a.Content + "'test'" + b.Headlines + a.IdentityId,
                        bbb = new QuerySet<Comment1>(conn, new MySqlProvider())
                                    .Where(y => y.ArticleId == b.Id && y.Content.Contains("test"))
                                    .Sum<Comment1>(x => x.Id),
                        ccc = a.IdentityId,
                        a.Id,
                        times = DateTime.Now
                    });


                //多视图查询
                var comment5 = conn.QuerySet<Comment>()
                     .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                     .Join<Comment, ResourceMapping>((a, b) => a.Id == b.FKId)
                     .Where(x => x.Content == "test")
                     .From<Comment, News, ResourceMapping>()
                     .OrderBy<News>(x => x.Id)
                     .Where((a, b, c) => a.ArticleId == b.Id)
                     .PageList(1, 10, (a, b, c) => new
                     {
                         id = a.Id,
                         name = b.NewsLabel,
                         resource = c.RPath,
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
