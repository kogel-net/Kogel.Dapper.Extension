using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	/// <summary>
	/// 類Order。
	/// </summary>
	[Display(Rename = "LiGe_Order")]
	public partial class Order
	{
		/// <summary>
		/// 
		/// </summary>
		[Identity]
		public int Id { get; set; }
		/// <summary>
		/// 訂單號
		/// </summary>
		public string OrderNo { get; set; }
		/// <summary>
		/// 訂單類型
		/// </summary>
		public int OrderType { get; set; }
		/// <summary>
		/// 商戶id
		/// </summary>
		public int StoreId { get; set; }
		/// <summary>
		/// 商戶名稱
		/// </summary>
		public string StoreName { get; set; }
		/// <summary>
		/// 用戶id
		/// </summary>
		public int AppUserId { get; set; }
		/// <summary>
		/// 用戶名稱
		/// </summary>
		public string AppUserName { get; set; }
		/// <summary>
		/// 備注
		/// </summary>
		public string Remark { get; set; }
		/// <summary>
		/// 訂單狀態
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 總價
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// 總積分
		/// </summary>
		public decimal Point { get; set; }
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
}

