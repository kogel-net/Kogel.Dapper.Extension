using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Core.Test.Models.Model
{
	public class Order
	{
		[Identity]
		public int Id { get; set; }
		/// <summary>
		/// 订单编号
		/// 订单号生成规则：201808281431452356+20+236
		/// 时间yyyyMMddHHmmssffff+appUserId+三位随机数
		/// </summary>
		public string OrderNo { get; set; }
		/// <summary>
		/// 街市Id
		/// </summary>
		public int MarketId { get; set; }
		/// <summary>
		/// 街市名称
		/// </summary>
		public string MarketName { get; set; }
		/// <summary>
		/// 数量
		/// </summary>
		public int Quantity { get; set; }
		/// <summary>
		/// 会有卡号
		/// </summary>
		public string CardNo { get; set; }
		/// <summary>
		/// 金额
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// 积分
		/// </summary>
		public int Points { get; set; }
		/// <summary>
		/// 订单状态
		/// </summary>
		public int OrderStatus { get; set; }
		/// <summary>
		/// 订单备注
		/// </summary>
		public string OrderRemark { get; set; }
		/// <summary>
		/// 用户Id
		/// </summary>
		public int AppUserId { get; set; }
		/// <summary>
		/// 提货时间
		/// </summary>
		public DateTime PickUpDoodsTime { get; set; }
		/// <summary>
		/// 支付方式
		/// </summary>
		public string PayMent { get; set; }
		/// <summary>
		/// 订单类型
		/// 餐饮，零售
		/// </summary>
		public int OrderType { get; set; }
		/// <summary>
		/// 备货推送
		/// 0：未推送
		/// 1：已推送
		/// </summary>
		public int SIMPush { get; set; }
	}
}
