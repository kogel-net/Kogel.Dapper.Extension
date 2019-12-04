using System;
using System.Collections.Generic;
using System.Text;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	/// <summary>
	/// 礼品详情
	/// </summary>
	[Display(Rename = "Lige_GiftDetail")]
	public class GiftDetail : IBaseEntity<GiftDetail>
	{
		/// <summary>
		/// 
		/// </summary>
		[Identity]
		public int Id { get; set; }
		/// <summary>
		/// 礼品代码
		/// </summary>
		public string ProductCode { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Introduction_CN { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Introduction_EN { get; set; }
		/// <summary>
		/// 说明
		/// </summary>
		public string Description_CN { get; set; }
		/// <summary>
		/// 说明
		/// </summary>
		public string Description_EN { get; set; }
		/// <summary>
		/// 中文图片
		/// </summary>
		public string ImgUrl_CN { get; set; }
		/// <summary>
		/// 英文图片
		/// </summary>
		public string ImgUrl_EN { get; set; }
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
