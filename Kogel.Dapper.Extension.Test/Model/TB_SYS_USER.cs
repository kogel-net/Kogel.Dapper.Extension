using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model
{
	public class TB_SYS_USER : IBaseEntity<TB_SYS_USER, int>
	{
		[Display(IsField = false)]
		public override int Id { get; set; }
		/// <summary>
		/// 用户ID
		/// </summary>
		public string USER_ID { get; set; }

		/// <summary>
		/// 用户姓名
		/// </summary>
		public string USER_NAME { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string PASSWORD { get; set; }

		/// <summary>
		///部门名称
		/// </summary>
		public string DEPT { get; set; }

		/// <summary>
		/// 部门名称
		/// </summary>
		[Display(IsField = false)]
		public string DEPT_NAME { get; set; }

		/// <summary>
		/// 是否管理员
		/// </summary>
		public string ISADMIN { get; set; }

		/// <summary>
		/// 是否禁用
		/// </summary>
		public string DISABLE { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string REMARK { get; set; }

		/// <summary>
		/// 事业部ID
		/// </summary>
		[Display(IsField = false)]
		public string BU_ID { get; set; }

		/// <summary>
		/// 事业部名称
		/// </summary>
		[Display(IsField = false)]
		public string BU_NAME { get; set; }
	}

	/// <summary>
	/// 部门
	/// </summary>
	[Display(AsName = "DEP", Rename = "TB_SYS_DEPT")]
	public class TB_SYS_DEPT : IBaseEntity<TB_SYS_DEPT, int>
	{
		[Display(IsField = false)]
		public override int Id { get; set; }

		/// <summary>
		/// 部门ID
		/// </summary>
		public string DEPT_ID { get; set; }

		/// <summary>
		/// 部门名称
		/// </summary>
		public string DEPT_NAME { get; set; }
	}

	/// <summary>
	/// 标准定额员工表
	/// </summary>
	[Display(AsName = "EMP", Rename = "TB_TDG_UPH_EMP")]
	public class TB_TDG_UPH_EMP : IBaseEntity<TB_TDG_UPH_EMP, int>
	{
		[Display(IsField = false)]
		public override int Id { get; set; }
		/// <summary>
		/// 工号
		/// </summary>
		public string EMP_ID { get; set; }

		/// <summary>
		/// 员工姓名
		/// </summary>
		public string EMP_NAME { get; set; }

		/// <summary>
		/// 事业部ID
		/// </summary>
		public string BU_ID { get; set; }

		/// <summary>
		/// 事业部名称
		/// </summary>
		public string BU_NAME { get; set; }

		/// <summary>
		/// 岗位ID
		/// </summary>
		public string POS_ID { get; set; }

		/// <summary>
		/// 岗位名称
		/// </summary>
		public string POS_NAME { get; set; }

		/// <summary>
		/// 所属线体
		/// </summary>
		public string LINES { get; set; }

		/// <summary>
		/// 班别
		/// </summary>
		public string SHIFT_TYPE { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string REMARK { get; set; }

		/// <summary>
		/// 是否离职
		/// </summary>
		public string STATUS { get; set; }

		/// <summary>
		/// 考核类别
		/// </summary>
		public string KPI_TYPE { get; set; }


		/// <summary>
		/// 显示字段
		/// </summary>
		[Display(IsField = false)]
		public string DisplayName
		{
			get { return EMP_ID + EMP_NAME; }
		}
	}
}
