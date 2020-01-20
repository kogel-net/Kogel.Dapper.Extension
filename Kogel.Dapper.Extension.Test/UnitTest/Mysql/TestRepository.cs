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

	public class SqlServerRepository : BaseRepository<Lige.Model.Order>
	{
		public override void OnConfiguring(RepositoryOptionsBuilder builder)
		{
			builder
				.BuildConnection(new SqlConnection("server=risingup.life98.cn,55940;database=Lige;user=sa;password=!RisingupTech/././.;max pool size=300"))
				.BuildProvider(new MsSqlProvider())
				.BuildAutoSyncStructure(true);
		}
	}

	public class TestRepositoryQuery
	{
		public void Test()
		{
			using (MysqlRepository mysqlRepository = new MysqlRepository())
			{
				using (SqlServerRepository sqlServerRepository = new SqlServerRepository())
				{
					try
					{
						//开启mysql事务
						var mysqlUnitWork = mysqlRepository.UnitOfWork.BeginTransaction(() =>
						{
							mysqlRepository.Insert(new Comment()
							{
								Content = "test"
							});
						});
						//开始sqlserver事务
						var sqlserverUnitWork = sqlServerRepository.UnitOfWork.BeginTransaction(() =>
						{
							//sqlServerRepository.Insert(new Lige.Model.Order()
							//{
							//	Remark = "test"
							//});
						});
						//都完成后一起提交
						mysqlUnitWork.Commit();
						sqlserverUnitWork.Commit();
					}
					catch (System.Exception ex)
					{

					}
				}
			}
		}
	}
}
