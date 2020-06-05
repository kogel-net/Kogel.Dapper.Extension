using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.MySql;
using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.Test.ViewModel;
using MySql.Data.MySqlClient;
using Dapper;
using System.Threading;
using System.Diagnostics;
using Kogel.Dapper.Extension.Model;
using System.Data;
using static Dapper.SqlMapper;
using Kogel.Dapper.Extension.Extension;
using Newtonsoft.Json;
using Kogel.Dapper.Extension.MySql.Extension;
using Lige.Model;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
    public class Query
    {
        string mysqlConnection = "Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;";
        public void Test()
        {
            //Sql执行前
            SqlMapper.Aop.OnExecuting += (ref CommandDefinition command) =>
            {
                //生成的sql
                var sql = command.CommandText;
                //生成的参数
                var param = command.Parameters;

                Console.WriteLine(sql);
            };
            //Sql执行后
            SqlMapper.Aop.OnExecuted += (ref CommandDefinition command) =>
             {
                 //生成的sql
                 var sql = command.CommandText;
                 //生成的参数
                 var param = command.Parameters;

                 Console.WriteLine(sql);
             };

            //Thread thread = new Thread(() =>
            //{
            //	//执行前
            //	SqlMapper.Aop.OnExecuting += (CommandDefinition Command) =>
            //	{

            //	};
            //	//执行后
            //	SqlMapper.Aop.OnExecuted += (CommandDefinition Command) =>
            //	{

            //	};

            //	using (var conn = new MySqlConnection(mysqlConnection))
            //	{
            //		var list= conn.QuerySet<Comment>().ToList();
            //	}
            //});
            //thread.Start();

            using (var conn = new MySqlConnection(mysqlConnection))
            {
                EntityCache.Register(typeof(AdminUser));


                IBaseEntity entitys = new Comment();
                var test = conn.QuerySet<Comment>()
                    .Where(x => x.PId.HasValue)
                    .WhereIf(false, x => 1 != 1, x => 1 == 1)
                    .Get();


                ////测试codefirst
                //CodeFirst codeFirst = new CodeFirst(conn);
                //codeFirst.SyncStructure();

                var dataSet = conn.QuerySet<Comment>()
                       .Where(x => 1 == 1)
                       .ToDataSet(new MySqlDataAdapter());

                DateTime dateTime = DateTime.Now.AddDays(-10);

                //var comments = conn.Query<Comment>("Select * from Comment").ToList();

                //var lis = conn.Query<News1, List<Comment>, News1, News1, News1, News1>("SELECT * FROM NEWS AS A LEFT JOIN COMMENT AS B ON A.Id=B.ArticleId");

                //var test1 = conn.QuerySet<News1>().Get();

                //var getIfTest = conn.QuerySet<Comment>()
                //	.Get(false, x => new CommentDto()
                //	{
                //		Id = x.Id,
                //		ArticleIds = x.ArticleId
                //	}, x => new CommentDto()
                //	{
                //		Id = x.Id,
                //		Content = x.Content
                //	});

                //int[] array = new int[] { 1, 2, 3 };


                DynamicParameters param = new DynamicParameters();
                param.Add("Id", 10);

                //var aaaa = SqlMapper.Query<Comment2, News>(conn,"", (first, second) =>
                //  {
                //  };
                var comment = conn.QuerySet<News1>()
                .Where(x => x.Comments.Any(y => y.Id.Between(1, 100) && y.ResourceMapping.Any(z => z.Id.Between(1, 100) && z.MenList.Any(l => l.Id.Between(1, 100)))))
                .ToList();

                //var comment11 = conn.QuerySet<News1>()
                //.Where(x => x.Comments.Any(y => y.Id != 10))
                //.Get(x => new NewsDto1
                //{
                //	Id = x.Id,
                //	Title = x.Content,
                //	CommentDto1 = x.Comments.Select(y => new CommentDto1()
                //	{
                //		Id = y.Id,
                //		Content = y.Content
                //	}).ToList()
                //});


                //x.Comments.Select(y => new CommentDto1()
                //{
                //	Id = y.Id,
                //	Content = y.Content
                //}).ToList()
                var testaa = "123";



                //	var list= conn.QueryFirstOrDefault<News1, List<Comment>, DontMap, DontMap, DontMap, DontMap>(@"SELECT
                //* FROM News AS A LEFT JOIN COMMENT AS B ON A.Id=B.ArticleId");

                //var list1 = conn.Query<Comment2, News>("SELECT * FROM COMMENT AS A LEFT JOIN NEWS AS B ON A.ArticleId=B.Id");

                ////使用导航属性
                //var list = conn.Query<Comment, News>("SELECT * FROM COMMENT AS A LEFT JOIN NEWS AS B ON A.ArticleId=B.Id", (a, b) => {
                //	a.News = b;
                // 				}, splitOn: "Id");


                ////var count = conn.QuerySet<Comment>().Count();

                ////单个属性返回
                //var ContentList = conn.QuerySet<Comment>()
                //	 //.Where(x => x.Content.IsNotNull() && !(x.Content == "") && x.IsDeleted)
                //	 //.WhereIf(!string.IsNullOrEmpty("aaa"), x => x.ArticleId == 1, x => x.ArticleId == 2)
                //	 .PageList(1, 20, x => new CommentDto()
                //	 {
                //		 Id = x.Id,
                //		 ArticleIds = x.ArticleId,
                //		 ////count = conn.QuerySet<News>().Where(y => y.Id == x.ArticleId).Count(),
                //		 NewsList = conn.QuerySet<News>()
                //		   .Join<News, Comment>((a, b) => a.Id == b.ArticleId, JoinMode.LEFT, true)
                //		   .Where(y => y.Id == x.ArticleId)
                //		   .From<News, Comment>()
                //		   .ToList((y, z) => new NewsDto()
                //		   {
                //			   Id = y.Id,
                //			   Contents = y.Content,

                //		   }).ToList(),
                //		 NewsDto = conn.QuerySet<News>().Where(y => y.Id == x.ArticleId).Get(y => new NewsDto()
                //		 {
                //			 Id = y.Id,
                //			 Contents = y.Content
                //		 }),
                //		 IsClickLike = conn.QuerySet<News>().Where(y => y.Id == x.ArticleId).Get(y => true)
                //	 });
                var array1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, };
                var commne = conn.QuerySet<Comment>()
                    .Where(x => x.Id > 0 && array1.Contains(x.Id) && x.Content.Replace("1", "2") == x.Content)
                    .Where(x => x.Id.In(array1))
                    .GroupBy(x => new { x.ArticleId })
                    .Having(x => Function.Sum(x.Id) >= 5)
                    .ToList(x => new
                    {
                        x.ArticleId,
                        test1 = Function.Sum(x.Id)
                    });

                var ids = conn.QuerySet<Comment>()
                     .Where(x => x.Id > 0 && array1.Contains(x.Id) && x.Content.Replace("1", "2") == x.Content && x.Content.Contains(null))
                     .Where(x => x.Id.In(array1))
                     .Get(x => Function.Sum(x.Id));
                //翻页
                var comment1 = conn.QuerySet<Comment>()
                    .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                    .Where(x => x.Id.ToString().ToUpper().Equals("3".ToUpper()))
                    .Where(x => x.Id.Between(80, 100)
                    && x.SubTime.Value.AddDays(-10).AddYears(1) < DateTime.Now.AddYears(1) && x.Id > 10
                    && x.Id > new QuerySet<News>(conn, new MySqlProvider()).Where(y => y.Id < 3 && x.Id < y.Id).Sum(y => y.Id))
                    .From<Comment, News>()
                    .OrderBy<News>(x => x.Id)
                    .PageList(1, 1, (a, b) => new
                    {
                        test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y => y == 1),
                        aaa = "6666" + "777",
                        Content = a.Content + "'test'" + b.Headlines + a.IdentityId,
                        bbb = conn.QuerySet<Comment1>()
                                    .Where(y => y.ArticleId == b.Id && y.Content.Contains("test"))
                                    .Sum(x => x.Id),
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
                       .Sum(x => x.Id);
                //计数
                var count = conn.QuerySet<Comment>()
                     .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
                     .Where(x => x.Content == "test1")
                     .Where<News>(x => x.NewsLabel.Contains("足球"))
                     .WithNoLock()
                     .Count();
            }
        }

        public void TestMaxAndMin()
        {
            using (var conn = new MySqlConnection(mysqlConnection))
            {
                var min = conn.QuerySet<Comment>().Where(x => 1 != 1).Min(x => x.Id);

                var max = conn.QuerySet<Comment>().Max(x => x.Id);
            }
        }

        public class Ormtest
        {
            public int id { set; get; }

            public string test1 { set; get; }


            public string test2 { set; get; }
        }
    }
}
