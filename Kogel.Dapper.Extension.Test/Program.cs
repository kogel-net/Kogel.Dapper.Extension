using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Extension.From;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.Test.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using System.Linq;

namespace Kogel.Dapper.Extension.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var mssqlConnection = "Data Source=42.157.195.21,4344;Initial Catalog=Qx_Sport_Common;User ID=qxdev;Password=qxdev123456;";
            var mysqlConnection = "Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;";
            Stopwatch stopwatch = new Stopwatch();

            using (var conn = new SqlConnection(mssqlConnection))
            {
                //var comment1 = conn.QuerySet<Comment>().Sum<Comment>(x => x.Id);
                var comment1 = conn.QuerySet<Comment>()
                    .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                    .Where(x => x.Id.Between(80, 100) && x.SubTime.AddDays(-10) < DateTime.Now && x.Id>10)
                    .From<Comment, News>()
                    .Get((a, b) => new
                    {
                        test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y => y == 1),
                        aaa = "6666" + "777",
                        Content = a.Content + "'test'" + b.Headlines + a.IdentityId,
                        bbb = new QuerySet<Comment>(conn, new MsSqlProvider())
                              .Where(y => y.ArticleId == b.Id && y.Content.Contains("test")).Sum<Comment>(x => x.Id),
                        //ccc=a.IdentityId,
                        //ddd=Convert.ToInt32("select count(1) from Comment")
                    });
                    
                var edit = conn.CommandSet<Comment>()
                           .Where(x => x.Id.In(new int[] { 1, 2, 3 }))
                           .Update(x => new Comment
                           {
                               StarCount = x.StarCount + 1
                           });


                var comment2 = conn.QuerySet<Comment>()
                     .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                     .Where(x => x.Content == "test")
                     .WithNoLock()
                     .OrderBy(x => x.Id)
                     .PageList(1, 2, x => new
                     {
                         id = x.Id,
                         name = 123,
                         test = x.Content,
                         //rownum = Convert.ToInt32("ROW_NUMBER() OVER(ORDER BY Comment.Id)"),
                         NewsLable = "News.NewsLabel"
                     });



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
                         //rownum = Convert.ToInt32("ROW_NUMBER() OVER(ORDER BY Comment.Id)"),
                     });


                var sum = conn.QuerySet<Comment>()
                       .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                       .Where(x => x.Content == "test")
                       .WithNoLock()
                       .Sum<News>(x => x.Id);

                var count = conn.QuerySet<Comment>()
                     .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                     .Where(x => x.Content == "test1")
                     .Where<News>(x => x.NewsLabel.Contains("足球"))
                     .WithNoLock()
                     .Count();

               

                var querySet = conn.QuerySet<Comment>()
                     .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                     .Join<Comment, ResourceMapping>((a, b) => a.Id == b.FKId)
                     .Where(x => x.Content == "test");

                var list = new testFrom<Comment, Comment, News, ResourceMapping>(querySet)
                      .ToList((a, b, c) => new
                      {
                          id = a.Id,
                          name = b.NewsFrom
                      });

                var commentList = conn.QuerySet<Comment>()
                    .Where(x => x.Content.Contains("t"))
                    .ToList();
                int result = conn.CommandSet<Comment>().BatchInsert(commentList, 1000);

                int result2 = conn.CommandSet<Comment>().Where(x => x.Id > 83).Delete();
            }
            stopwatch.Stop();

            Console.Write(stopwatch.Elapsed.TotalSeconds);
        }
    }
    public class testFrom<T, T1, T2, T3> : ISelect<T>
    {
        public testFrom(QuerySet<T> querySet) : base(querySet)
        {

        }
        public IEnumerable<TReturn> ToList<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return base.ToList<TReturn>(select);
        }
    }
}
