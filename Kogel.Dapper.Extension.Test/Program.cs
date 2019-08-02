using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Extension.From;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.Test.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Kogel.Dapper.Extension.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=42.157.195.21,4344;Initial Catalog=Qx_Sport_Common;User ID=qxdev;Password=qxdev123456;";
            Stopwatch stopwatch = new Stopwatch();

            using (var conn = new SqlConnection(connectionString))
            {
                var commentIn = conn.QuerySet<Comment>()
                     .Where(x => x.Id.NotIn(new int[] { 1, 2, 3 }) && x.SubTime.AddMinutes(50) < DateTime.Now.AddDays(-1) && x.Type.IsNotNull())
                     .ToList();

                var commentIn2 = conn.QuerySet<Comment>()
                    .Where(x => x.Content.In(new string[] { "test1", "test2" }))
                    .ToList();

                var commentBe = conn.QuerySet<Comment>()
                    .Where(x => x.Id.Between(1, 5))
                    .ToList();

                var commentBe2 = conn.QuerySet<Comment>()
                    .Where(x => x.SubTime.Between(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(1)) && (x.Content == "test1" && x.Content.Contains("test")))
                    .ToList();

                var comment99 = conn.QuerySet<Comment>()
                    .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                    .Where(x => x.Content == "test1" && x.Content.Contains("t"))
                    .Where<Comment, News>((a, b) => a.SubTime < DateTime.Now.AddDays(-5) && a.Id > a.Id % 1)
                    .Get(x => new Comment()
                    {
                        Id = 123,

                        ArticleId = x.ArticleId
                    });

                var comment1 = conn.QuerySet<Comment>()
                    .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                    .Where(x => x.Content == "test1" && x.Content.Contains("t"))
                    .Where<Comment, News>((a, b) => a.SubTime < DateTime.Now.AddDays(-5) && a.Id > a.Id % 1)
                    .Get(x => new
                    {
                        count = Convert.ToInt32("(select count(1) from Comment_4)"),
                        //test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y=>y==1),
                        aaa = "6666",
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
                         rownum = Convert.ToInt32("ROW_NUMBER() OVER(ORDER BY Comment.Id)"),
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
                         rownum = Convert.ToInt32("ROW_NUMBER() OVER(ORDER BY Comment.Id)"),
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

                var edit = conn.CommandSet<Comment>()
                    .Where(x => x.Content == "test")
                    .Update(x => new Comment
                    {
                        Content = "test1"
                    });

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
                    .Where(x => x.Content == "test1")
                    .ToList();
                int result = conn.CommandSet<Comment>().BatchInsert(commentList, 1000);

                int result2 = conn.CommandSet<Comment>().Where(x => x.Id > 14).Delete();
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
