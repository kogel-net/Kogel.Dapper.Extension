using Kogel.Dapper.Extension.Attributes;
using System;

namespace Kogel.Dapper.Extension
{
	/// <summary>
	/// 父级实体类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class IBaseEntity<T>
	{
		[Identity]
		public virtual T Id { get; set; }
	}
}
