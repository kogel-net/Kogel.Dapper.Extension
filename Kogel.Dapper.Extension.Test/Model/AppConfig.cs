using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model
{
	[Display(Rename = "Lige_AppConfig", AsName = "AppConfig")]
	public class AppConfig
	{
		/// <summary>
		/// 
		/// </summary>
		[Identity]
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
		/// 
		/// </summary>
		public string CreateUser { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UpdateUser { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsDelete { get; set; }

	}

	/// <summary>
	/// 多表联查扩展类（当一次查询用到多次相同表时使用）
	/// </summary>
	[Display(Rename = "Lige_AppConfig", AsName = "AppConfig1")]
	public class AppConfig1 : AppConfig
	{

	}
}
