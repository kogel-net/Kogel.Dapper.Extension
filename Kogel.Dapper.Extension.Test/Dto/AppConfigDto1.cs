using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Dto
{
	public class AppConfigDto1
	{
		/// <summary>
		/// 
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// 编码(唯一标识)
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 父级id
		/// </summary>
		public int ParentId { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 商场id
		/// </summary>
		public string MarketId { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 父级名称
		/// </summary>
		public string ParentName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsDelete { get; set; }
	}
}
