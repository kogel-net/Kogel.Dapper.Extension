using Kogel.Dapper.Extension.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Oracle;
using Oracle.ManagedDataAccess.Client;
using Kogel.Dapper.Extension.Oracle.Extension;
using Dapper;

namespace Kogel.Dapper.Extension.Test.UnitTest.Oracle
{
	public class Command
	{
		string oracleConnection = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.105.0.224)(PORT=1521))
                    (CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=system;Password=A123456a";
		public void Test()
		{

			var guid = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");

			var commne = new Comment()
			{
				Id = 10,
				Content = "test",
				ArticleId = 11,
				Type = 1,
				SubTime = DateTime.Now,
				PId = 0,
				RefCommentId = 0,
				//Guid = guid
			};

			using (var conn = new OracleConnection(oracleConnection))
			{
				//conn.Open();

				EntityCache.Register(typeof(Comment));



				//Guid[] guids = new Guid[] { Guid.NewGuid() };

				//var resultTest = conn.CommandSet<Comment>()
				//	.Where(x => guids.Contains(x.Guid))
				//	.Delete();

				//////测试codefirst
				////CodeFirst codeFirst = new CodeFirst(conn);
				////codeFirst.SyncStructure();

				//DynamicParameters parameters = new DynamicParameters();
				//parameters.Add("Guid_1_1_1_sssssss_sssssssGuid_1_1_1_sssssss_sssssss", guid);

				//var sss = conn.Execute($@"UPDATE Comments SET Guid=:Guid_1_1_1_sssssss_sssssssGuid_1_1_1_sssssss_sssssss ", parameters);

				//根据成员修改
				var result = conn.CommandSet<Comment>()
				.Where(x => x.Id > commne.Id || x.Id < commne.Id)
				.Update(x => new Comment()
				{
					Content = commne.Content,
					SubTime = commne.SubTime,
					///Guid = guid
				});
				////全部修改
				//var result1 = conn.CommandSet<Comment>()
				//    .Where(x => x.Id == commne.Id)
				//    .Update(commne);

				//新增
				var result2 = conn.CommandSet<Comment>()
					.InsertIdentity(new Comment()
					{
						ArticleId = 11,
						Type = 1,
						SubTime = DateTime.Now,
						Content = "test",
						PId = 0,
						RefCommentId = 0
					});
				//新增返回自增id
				//var result3 = conn.CommandSet<Comment>()
				//	.InsertIdentity(new Comment()
				//	{
				//		ArticleId = 11,
				//		Type = 1,
				//		SubTime = DateTime.Now,
				//		Content = "test",
				//		PId = 0,
				//		RefCommentId = 0
				//	});
				//批量新增
				var result4 = conn.CommandSet<Comment>()
					.Insert(new List<Comment>()
					{
						commne,
						commne,
						commne
					});

				var list = new List<Guid>() { guid };
				////删除
				//var result5 = conn.CommandSet<Comment>()
				//	.Where(x => x.Guid == list.FirstOrDefault())
				//	.Delete();
			}
		}
	}
}
