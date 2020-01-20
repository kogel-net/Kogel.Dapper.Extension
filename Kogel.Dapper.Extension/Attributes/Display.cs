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
		/// 是否允许为空
		/// </summary>
		public bool IsNull { get; set; }

		/// <summary>
		/// 默认值
		/// </summary>
		public object DefaultValue { get; set; }

		/// <summary>
		/// 描述字段特性
		/// </summary>
		/// <param name="Name">名称</param>
		/// <param name="Description">描述</param>
		/// <param name="Rename">设置对应数据库名称</param>
		/// <param name="Schema">命名空间（sqlserver默认dbo）</param>
		/// <param name="AsName">重命名 as xxx</param>
		/// <param name="IsField">是否是表关联字段(实体类为True)</param>
		/// <param name="SqlDbType">数据库字段类型</param>
		/// <param name="Length">字段长度</param>
		/// <param name="IsNull">是否为空</param>
		/// <param name="DefaultValue">字段默认值</param>
		public Display(string Name = null, string Description = null, string Rename = null, string Schema = null,
			string AsName = null, bool IsField = true, SqlDbType SqlDbType = SqlDbType.Structured, int Length = 0,
			bool IsNull = false, object DefaultValue = null)
		{
			this.Name = Name;
			this.Description = Description;
			this.IsField = IsField;
			this.Rename = string.IsNullOrEmpty(Rename) ? Name : Rename;
			this.Schema = Schema;
			this.AsName = AsName;
			this.SqlDbType = SqlDbType;
			this.Length = Length;
			this.IsNull = IsNull;
			this.DefaultValue = DefaultValue;
		}
	}
}
