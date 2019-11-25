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

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
	public class TestRepository : BaseRepository<Comment>//Comment为实体类
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
			using (TestRepository testRepository = new TestRepository())
			{
				var querySet = testRepository.QuerySet();//查询对象
				var commandSet = testRepository.CommandSet();//执行对象
			}

			using (TestRepository testRepository = new TestRepository())
			{
				//开始事务
				testRepository.UnitOfWork.BeginTransaction(() =>
				{
					var comment = testRepository.Orm.QuerySet<Comment>().ToList();

					testRepository.Orm.CommandSet<Comment>()
						.Where(x => x.Id == comment.FirstOrDefault().Id)
						.Update(comment.FirstOrDefault());
					//其他仓储类代码块
					new TestRepositoryQuery1().Test();
				});
			
				//提交
				testRepository.UnitOfWork.Commit();
			}
		}
	}
	public class TestRepositoryQuery1
	{
		public void Test()
		{
			TestRepository testRepository = new TestRepository();
			var comment = testRepository.Orm.QuerySet<Comment>().PageList(1,10).Items;

			//testRepository.Orm.CommandSet<Comment>()
			//	.Insert(comment);


			//根据主键获取
			var getComment = testRepository.FindById(3);

			testRepository.Insert(getComment);
	
			testRepository.Update(getComment);

			testRepository.Delete(getComment.Id);


		}
	}
}
