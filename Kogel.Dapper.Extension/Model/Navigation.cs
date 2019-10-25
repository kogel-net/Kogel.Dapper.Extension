using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Model
{
	/// <summary>
	/// 导航属性的关联解析实体
	/// </summary>
	public class Navigation
	{
		/// <summary>
		/// 属性字段(当前定义的字段)
		/// </summary>
		public string AssoField { get; set; }
		/// <summary>
		/// 当前表设置的关联字段
		/// </summary>
		public string CurrentAssoField { get; set; }
		/// <summary>
		/// 连接表关联的字段(一般为主键)
		/// </summary>
		public string JoinAssoField { get; set; }
		/// <summary>
		/// 连接表实体
		/// </summary>
		public Type JsonAssoTable { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public NavigationEnum NavigationType { get; set; }
	}

	public enum NavigationEnum
	{
		Model = 0,
		List = 1
	}
}
