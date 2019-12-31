using Kogel.Dapper.Extension;
using Lige.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lige.ViewModel.APP.Shopping
{
	/// <summary>
	/// 订单列表返回
	/// </summary>
	public class OrderResDto
	{
		/// <summary>
		/// 
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// 订单号
		/// </summary>
		public string OrderNo { get; set; }
		/// <summary>
		/// 订单创建时间
		/// </summary>
		public DateTime OrderTime { get; set; }
		/// <summary>
		/// 订单状态
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 总金额
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// 总积分
		/// </summary>
		public decimal Point { get; set; }
		/// <summary>
		/// 是否存在明细
		/// </summary>
		public bool IsAnyOrderDetail { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public OrderDetail DetailList { get; set; }
		/// <summary>
		/// 订单明细
		/// </summary>
		public List<OrderDetailResDto> OrderDetailList { get; set; }
	}
	public class test<T> : IBaseEntity<T, int>
	{
		public override  int Id { get; set; }
	}
	/// <summary>
	/// 订单明细
	/// </summary>
	public class OrderDetailResDto: test<OrderDetailResDto>
	{
		/// <summary>
		/// 
		/// </summary>
		public override int Id { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 价钱
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 积分
		/// </summary>
		public decimal Point { get; set; }
		/// <summary>
		/// 件数
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 礼品图片
		/// </summary>
		public string ImgUrl { get; set; }
		/// <summary>
		/// 原价
		/// </summary>
		public decimal OriginalPrice { get; set; }
		/// <summary>
		/// 原积分
		/// </summary>
		public decimal OriginalPoint { get; set; }
	}
}
