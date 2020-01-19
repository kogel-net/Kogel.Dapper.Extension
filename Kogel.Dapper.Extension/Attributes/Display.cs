using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Kogel.Dapper.Extension.Attributes
{
	public class Display : BaseAttrbute
	{
		/// <summary>
		/// 是否需要隐射该字段
		/// </summary>
		public bool IsField { get; set; }
		/// <summary>
		/// 重命名(数据库隐射名称(默认为类名))
		/// </summary>
		public string Rename { get; set; }
		/// <summary>
		/// 名称空间(例如sqlserver中的dbo)
		/// </summary>
		public string Schema { get; set; }
		/// <summary>
		/// 指定 as 名称
		/// </summary>
		public string AsName { get; set; }
		/// <summary>
		/// 数据库字段类型
		/// </summary>
		public SqlDbType SqlDbType { get; set; }
		/// <summary>
		/// 长度
		/// </summary>
		public int Length { get; set; }
		/// <summary>
		/// 描述字段特性
		/// </summary>
		/// <param name="Name">名称</param>
		/// <param name="Description">描述</param>
		/// <param name="IsField">是否是表关联字段(实体类为True)</param>
		public Display(string Name = null, string Description = null, string Rename = null, string Schema = null, string AsName = null, bool IsField = true, SqlDbType SqlDbType = SqlDbType.Structured, int Length = 0)
		{
			this.Name = Name;
			this.Description = Description;
			this.IsField = IsField;
			this.Rename = string.IsNullOrEmpty(Rename) ? Name : Rename;
			this.Schema = Schema;
			this.AsName = AsName;
			this.SqlDbType = SqlDbType;
			this.Length = Length;
		}
	}
}
