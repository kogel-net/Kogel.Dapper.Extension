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
	public class TestRepository : BaseRepository
	{
		public override void OnConfiguring(RepositoryOptionsBuilder builder)
		{
			builder.BuildConnection(new MySqlConnection("Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;"));
		}
	}

	public class TestRepositoryQuery
	{
		public void Test()
		{
			using (TestRepository testRepository = new TestRepository())
			{
				try
				{

					testRepository.UnitOfWork.BeginTransaction(() =>
					{
						var comment = testRepository.Orm.QuerySet<Comment>().ToList();

						testRepository.Orm.CommandSet<Comment>()
							.Where(x => x.Id == comment.FirstOrDefault().Id)
							.Update(comment.FirstOrDefault());

						new TestRepositoryQuery1().Test();
					});
				}
				catch (System.Exception ex)
				{

				}
				testRepository.UnitOfWork.Commit();
			}
		}
	}
	public class TestRepositoryQuery1
	{
		public void Test()
		{
			TestRepository testRepository = new TestRepository();
			var comment = testRepository.Orm.QuerySet<Comment>().ToList();

			testRepository.Orm.CommandSet<Comment>()
				.Insert(comment);

		}
	}
}
