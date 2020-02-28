using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model
{
	[Display(Rename = "Lige_ProductWay")]
	public class ProductWay : IBaseEntity<ProductWay, int>
	{
		/// <summary>
		/// 
		/// </summary>
		[Identity]
		public override int Id { get; set; }
		/// <summary>
		/// 同步的標識
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 積分
		/// </summary>
		public decimal Point { get; set; }
		/// <summary>
		/// 金額
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 商品代碼
		/// </summary>
		public string ProductCode { get; set; }
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
		/// <summary>
		/// 支付方式類型
		/// </summary>
		public int ProductWayType { get; set; }
	}
}
