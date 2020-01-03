using System;
using System.Collections.Generic;
using System.Text;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	[Display(Rename ="Lige_AdminUser")]
	public class AdminUser
	{
		/// <summary>
		/// 
		/// </summary>
		[Identity]
		public int Id { get; set; }
		/// <summary>
		/// 賬戶
		/// </summary>
		public string Account { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Email { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CreateUser { get; set; }
		/// <summary>
		/// 創建時間
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UpdateUser { get; set; }
		/// <summary>
		/// 修改時間
		/// </summary>
		public DateTime UpdateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsDelete { get; set; }
	}
}
