using System;
using System.Collections.Generic;
using System.Text;

namespace Lige.ViewModel.Web.AppUser
{
	/// <summary>
	/// 后臺管理用戶
	/// </summary>
	public class AdminUserDto
	{
		/// <summary>
		/// 
		/// </summary>
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
		/// 唯一標識
		/// </summary>
		public string Token { get; set; }
	}
}
