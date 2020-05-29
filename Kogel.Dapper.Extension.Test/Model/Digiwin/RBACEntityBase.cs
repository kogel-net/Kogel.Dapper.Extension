using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model.Digiwin
{
	/// <summary>
	/// Entity父类
	/// </summary>
	/// <typeparam name="TPrimaryKey">主键</typeparam>
	public abstract class RBACEntityBase<TPrimaryKey> : IBaseEntity<RBACEntityBase, string>
	{
		[Identity(IsIncrease = false)]
		[Display(SqlDbType =System.Data.SqlDbType.UniqueIdentifier)]
		/// <summary>
		/// 主键
		/// </summary>
		public virtual TPrimaryKey GUID { get; set; }

		[Display(SqlDbType = System.Data.SqlDbType.UniqueIdentifier)]
		public virtual string CREATOR { get; set; }

		public virtual DateTime CREATE_TIME { get; set; }

		[Display(SqlDbType = System.Data.SqlDbType.UniqueIdentifier)]
		public virtual string MODIFIER { get; set; }

		public virtual DateTime MODIFY_TIME { get; set; }

		public virtual bool FLAG { get; set; }

		public virtual bool DELETE_FLAG { get; set; }

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	#region Implements

	/// <summary>
	/// Guid 类型主键实体基类
	/// </summary>
	public abstract class RBACEntityBase : RBACEntityBase<string>
	{ }

	#endregion
}
