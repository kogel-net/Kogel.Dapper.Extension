using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model
{
	/// <summary>
	/// 版 本 1.0
	/// Copyright (c) 2018 
	/// 创建人：LHS
	/// 日 期：2018年3月19日
	/// 描述：资源映射实体类
	/// </summary>
	public class ResourceMapping : IBaseEntity<ResourceMapping, long>
	{
		/// <summary>
		/// Id
		/// </summary>
	    [Identity]
		public override long Id { get; set; }
		/// <summary>
		/// 扩展名
		/// </summary>
		public string Extension { get; set; }
		/// <summary>
		/// 图片路径
		/// </summary>
		public string RPath { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 排序
		/// </summary>
		public int SortCode { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 资源类型 1=文章缩略图 2=用户头像 3=评论图
		/// </summary>
		public int Type { get; set; }
		/// <summary>
		/// 资源关联Id type=1时，则对应文章Id
		/// </summary>
		public int FKId { get; set; }
		/// <summary>
		/// 文件大小
		/// </summary>
		public int RSize { get; set; }
	}
}
