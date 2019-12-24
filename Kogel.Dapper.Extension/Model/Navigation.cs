using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Model
{
	/// <summary>
	/// 导航属性的关联解析实体
	/// </summary>
	public class Navigation
	{
		public Navigation()
		{
			//WhereExpressionList = new List<LambdaExpression>();
		}

		/// <summary>
		/// 属性字段(当前定义的字段)
		/// </summary>
		public string AssoField { get; set; }
		/// <summary>
		/// 关联条件
		/// </summary>
		public string JoinWhere { get; set; }
		/// <summary>
		/// 连接表实体
		/// </summary>
		public Type JsonAssoTable { get; set; }
		/// <summary>
		/// 导航类型(0实体，1列表(集合))
		/// </summary>
		public NavigationEnum NavigationType { get; set; }
	}

	public enum NavigationEnum
	{
		Model = 0,
		List = 1
	}
}
