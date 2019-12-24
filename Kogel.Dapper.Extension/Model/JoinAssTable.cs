using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Kogel.Dapper.Extension.Model
{
	public class JoinAssTable
	{
		public JoinAssTable()
		{
			MapperList = new Dictionary<string, string>();
		}

		public JoinAction Action { get; set; }
		public JoinMode JoinMode { get; set; }
		public string RightTabName { get; set; }
		public string LeftTabName { get; set; }
		public string RightAssName { get; set; }
		public string LeftAssName { get; set; }
		public Type TableType { get; set; }
		public string JoinSql { get; set; }
		public Type PropertyType { get; set; }
		/// <summary>
		/// 表首字段
		/// </summary>
		public string FirstFieldName => MapperList.Values.AsList()[0];
		/// <summary>
		/// 隐射目录
		/// </summary>
		public Dictionary<string, string> MapperList { get; set; }
	}

	/// <summary>
	/// 连表方式
	/// </summary>
	public enum JoinAction
	{
		Default,//默认表达式
		Sql,//sql查询
		Navigation//导航属性
	}

	public enum JoinMode
	{
		LEFT,//左连接
		RIGHT,//右连接
		INNER,//内连接
		FULL,//全连接
	}
}
