using Kogel.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Kogel.Dapper.Extension.MySql;
using Kogel.Dapper.Extension.Test.Model;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.Oracle;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
	public class MysqlRepository : BaseRepository<Comment>//Comment为实体类
	{
		public override void OnConfiguring(RepositoryOptionsBuilder builder)
		{
			builder
				.BuildConnection(new MySqlConnection("Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;"))//配置连接方式
				.BuildProvider(new MySqlProvider());//配置数据库提供者

		}
	}

	public class MysqlRepository1 : BaseRepository<News>
	{
		public override void OnConfiguring(RepositoryOptionsBuilder builder)
		{
			builder
				.BuildConnection(new MySqlConnection("Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;"))//配置连接方式
				.BuildProvider(new MySqlProvider());//配置数据库提供者

		}
	}

	public class TestRepositoryQuery
	{
		public void Test()
		{
			using (MysqlRepository mysqlRepository = new MysqlRepository())
			{
				var aaa = mysqlRepository.QuerySet<Comment>()
					   .Distinct()
					   .PageList(1, 1, x => new
					   {
						   id = Function.Concact(Function.Concact(x.Content, "test"), x.ArticleId.ToString())
					   });


				//mysqlRepository.UnitOfWork.BeginTransaction(() =>
				//{
				//	mysqlRepository.Insert(new Comment
				//	{
				//		Content = "test11111",
				//		ArticleId = 11,
				//		Type = 1,
				//		SubTime = DateTime.Now,
				//		PId = 0,
				//		RefCommentId = 0
				//	});

				//	MysqlRepository1 mysqlRepository1 = new MysqlRepository1();

				//	mysqlRepository1.Insert(new News
				//	{
				//		NewsLabel = "test",
				//		Headlines = "test",
				//		Content = "test",
				//		NewsFrom = "test",

				//	});
				//	//throw new System.Exception("test");
				//});

				//mysqlRepository.UnitOfWork.Commit();
			}
		}
	}
}
