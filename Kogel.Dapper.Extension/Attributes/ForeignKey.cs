using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Attributes
{
	/// <summary>
	/// 导航关联
	/// </summary>
	public class ForeignKey : Attribute
	{
		/// <summary>
		/// 索引字段
		/// </summary>
		public string IndexField { get; set; }
		/// <summary>
		/// (导航表)关联字段
		/// </summary>
		public string AssoField { get; set; }
		/// <summary>
		/// 导航关联
		/// </summary>
		/// <param name="IndexField">索引字段</param>
		/// <param name="AssoField">(导航表)关联字段</param>
		public ForeignKey(string IndexField, string AssoField)
		{
			this.IndexField = IndexField;
			this.AssoField = AssoField;
		}
	}
}
