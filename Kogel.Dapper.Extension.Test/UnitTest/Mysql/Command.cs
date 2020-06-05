using Kogel.Dapper.Extension.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Kogel.Dapper.Extension.MySql;
using Dapper;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql
{
	class Command
	{
		string mysqlConnection = "Server=localhost;Database=Qx_Sport_Common;Uid=root;Pwd=A5101264a;";
		public void Test()
		{
			var commne = new Comment()
			{
				Id = 10,
				Content = "test",
				ArticleId = 11,
				Type = 1,
				//SubTime = DateTime.Now,
				//PId = 0,
				RefCommentId = 0
			};
			DateTime dateTime = DateTime.Now.AddDays(100);
			using (var conn = new MySqlConnection(mysqlConnection))
			{

				var result22= conn.CommandSet<Comment>()
					.Delete(commne);

				//根据成员修改
				var result = conn.CommandSet<Comment>()
				.Where(x => x.Id.In(new int[] { 1, 2, 3 }) && x.IsDeleted == false)
				.Update(x => new Comment()
				{
					// Content = commne.Content,
					SubTime = dateTime,
					IsDeleted = false,
					Content = "123" + x.Content
				});
				//全部修改
				var result1 = conn.CommandSet<Comment>()
					//.Where(x => x.Id == commne.Id)
					.Update(commne, new string[] { "ArticleId" });

				//新增
				var result2 = conn.CommandSet<Comment>()
					.Insert(commne);
				//新增返回自增id
				var result3 = conn.CommandSet<Comment>()
					.InsertIdentity(new Comment()
					{
						ArticleId = 11,
						Type = 1,
						//SubTime = DateTime.Now,
						Content = "test",
						//PId = 0,
						RefCommentId = 0
					});
				//批量新增
				var result4 = conn.CommandSet<Comment>()
					.Insert(new List<Comment>()
					{
						commne,
						commne,
						commne
					});
				//删除
				var result5 = conn.CommandSet<Comment>()
					.Where(x => x.Id == result3)
					.Delete();
			}
		}

		
	}
}
