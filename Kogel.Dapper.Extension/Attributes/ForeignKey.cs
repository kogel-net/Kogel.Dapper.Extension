using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Attributes
{
	/// <summary>
	/// 导航属性关联
	/// </summary>
	public class ForeignKey: Attribute
	{
		public string Name { get; set; }
		/// <summary>
		/// 关联字段
		/// </summary>
		/// <param name="Name"></param>
		public ForeignKey(string Name)
		{
			this.Name = Name;
		}
	}
}
