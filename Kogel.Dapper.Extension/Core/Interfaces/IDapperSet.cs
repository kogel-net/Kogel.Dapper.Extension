using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Model;
using Dapper;

namespace Kogel.Dapper.Extension
{
	public abstract class AbstractSet
	{
		/// <summary>
		/// 数据解析提供方
		/// </summary>
		public SqlProvider SqlProvider;

		/// <summary>
		/// 表类型
		/// </summary>
		public Type TableType { get; set; }

		/// <summary>
		/// [已弃用]只用来生成对象
		/// </summary>
		internal LambdaExpression WhereExpression { get; set; }

		/// <summary>
		/// 条件表达式对象
		/// </summary>
		internal List<LambdaExpression> WhereExpressionList { get; set; }

		/// <summary>
		/// 表达式排序集合
		/// </summary>
		internal Dictionary<LambdaExpression, EOrderBy> OrderbyExpressionList { get;  set; }

		/// <summary>
		/// 字符串排序
		/// </summary>
		internal StringBuilder OrderbyBuilder { get; set; }

		/// <summary>
		/// 字段查询对象
		/// </summary>
		public LambdaExpression SelectExpression { get;  set; }

		/// <summary>
		/// 是否锁表（with(nolock)）
		/// </summary>
		public bool NoLock { get;  set; }

		/// <summary>
		/// sql字符串对象
		/// </summary>
		internal StringBuilder WhereBuilder { get; set; }

		/// <summary>
		/// sql参数对象
		/// </summary>
		internal DynamicParameters Params { get => SqlProvider.Params; set => SqlProvider.Params.AddDynamicParams(value); }

		/// <summary>
		/// 分组表达式对象
		/// </summary>
		internal List<LambdaExpression> GroupExpressionList { get; set; }

		/// <summary>
		/// 分组聚合条件
		/// </summary>
		internal List<LambdaExpression> HavingExpressionList { get; set; }

		/// <summary>
		/// 是否去重
		/// </summary>
		public bool IsDistinct { get; set; } = false;
	}
}
