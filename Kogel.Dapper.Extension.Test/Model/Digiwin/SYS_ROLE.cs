using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension.Test.Model.Digiwin
{
	public class SYS_ROLE : RBACEntityBase
	{
		public string ROLE_CODE { get; set; }
		public string ROLE_NAME { get; set; }

		[Display(IsField = false)]
		public override string Id { get; set; }
	}

	public class SYS_USER : RBACEntityBase
	{
		#region 用户属性

		[Display(IsField = false)]
		public override string Id { get; set; }

		/// <summary>
		/// 用户代号
		/// </summary>
		public string USER_CODE { get; set; }

		/// <summary>
		/// 用户名
		/// </summary>
		public string USER_NAME { get; set; }

		/// <summary>
		/// 用户密码
		/// </summary>
		public string PASSWORD { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		public string EMAIL { get; set; }

		/// <summary>
		/// 用户手机号码
		/// </summary>
		public string PHONE { get; set; }

		[Display(IsField = false)]
		public List<SYS_ROLE> UserRoles { get; set; }

		#endregion
	}

	public class SYS_USER_ROLE : RBACEntityBase
	{
		[Display(IsField =false)]
		public override string Id { get; set; }

		[Display(SqlDbType = System.Data.SqlDbType.UniqueIdentifier)]
		public string USER_GUID { get; set; }

		[Display(SqlDbType = System.Data.SqlDbType.UniqueIdentifier)]
		public string ROLE_GUID { get; set; }
	}
}
