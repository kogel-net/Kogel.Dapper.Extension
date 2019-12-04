using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension;

namespace Lige.Model
{
	/// <summary>
	/// 類OrderDetail。
	/// </summary>
	[Display(Rename = "LiGe_OrderDetail")]
	public partial class OrderDetail:IBaseEntity<int>
	{
		/// <summary>
		/// 
		/// </summary>
		[Identity]
		public int Id { get; set; }
		/// <summary>
		/// 訂單編號
		/// </summary>
		public string OrderNo { get; set; }
		/// <summary>
		/// 商品代碼
		/// </summary>
		public string ProductCode { get; set; }
		/// <summary>
		/// 商品名稱
		/// </summary>
		public string ProductName { get; set; }
		/// <summary>
		/// 支付方式
		/// </summary>
		public string ProductWayCode { get; set; }
		/// <summary>
		/// 價錢
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 積分
		/// </summary>
		public decimal Point { get; set; }
		/// <summary>
		/// 件數
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int ShoppingCartId { get; set; }
		/// <summary>
		/// 創建人
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

