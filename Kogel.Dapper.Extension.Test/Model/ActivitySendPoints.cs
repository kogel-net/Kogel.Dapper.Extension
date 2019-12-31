using System;
using System.Collections.Generic;
using System.Text;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	[Display(Rename = "Lige_ActivitySendPoints")]
	public class ActivitySendPoints: IBaseEntity<ActivitySendPoints, int>
	{
		[Identity]
		public override int Id { get;set; }
		/// <summary>
		/// 
		/// </summary>
		public string CardNo { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string OrderNo { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string N5_ProfileID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string MachineId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Points { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CredateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateTime { get; set; }
		/// <summary>
		/// 0失败 1成功
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal TransactionAmount { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int SqlId { get; set; }

	}
}
