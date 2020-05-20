using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension
{
	internal class ResolveExpression : IResolveExpression
	{
		public ResolveExpression(SqlProvider provider) : base(provider)
		{

		}
		/// <summary>
		/// 解析查询字段
		/// </summary>
		/// <param name="topNum"></param>
		/// <returns></returns>
		public override string ResolveSelect(int? topNum)
		{
			//添加需要连接的导航表
			var masterEntity = EntityCache.QueryEntity(abstractSet.TableType);
			var navigationList = masterEntity.Navigations;
			if (navigationList.Any())
			{
				provider.JoinList.AddRange(ExpressionExtension.Clone(navigationList));
			}
			//查询字段解析
			StringBuilder selectSql = new StringBuilder(" SELECT ");
			////去重
			//if (abstractSet.IsDistinct)
			//	selectSql.Append(" DISTINCT ");
			//top
			if (topNum.HasValue)
				selectSql.Append($" TOP {topNum} ");
			//else
			//	//设置返回百分比(为了兼容更多语法)
			//	selectSql.Append(" TOP (100) PERCENT ");
			//不是自定义返回视图则显示所有字段
			if (provider.Context.Set.SelectExpression == null)
			{
				var propertyBuilder = GetTableField(masterEntity);
				selectSql.Append(propertyBuilder);
			}
			else//自定义查询字段
			{
				var selectExp = new SelectExpression(provider.Context.Set.SelectExpression, "", provider);
				selectSql.Append(selectExp.SqlCmd);
				provider.Params.AddDynamicParams(selectExp.Param);
			}
			return selectSql.ToString();
		}
		/// <summary>
		/// 解析查询总和
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public override string ResolveSum(LambdaExpression selector)
		{
			if (selector == null)
				throw new ArgumentException("selector");
			var selectSql = "";

			switch (selector.NodeType)
			{
				case ExpressionType.Lambda:
				case ExpressionType.MemberAccess:
					{
						EntityObject entityObject = EntityCache.QueryEntity(selector.Parameters[0].Type);
						var memberName = selector.Body.GetCorrectPropertyName();
						selectSql = $" SELECT ISNULL(SUM({entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}),0)  ";
					}
					break;
				case ExpressionType.MemberInit:
					throw new DapperExtensionException("不支持该表达式类型");
			}

			return selectSql;
		}

		/// <summary>
		/// 解析查询最大值
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public override string ResolveMax(LambdaExpression selector)
		{
			if (selector == null)
				throw new ArgumentException("selector");
			var selectSql = "";

			switch (selector.NodeType)
			{
				case ExpressionType.Lambda:
				case ExpressionType.MemberAccess:
					{
						EntityObject entityObject = EntityCache.QueryEntity(selector.Parameters[0].Type);
						var memberName = selector.Body.GetCorrectPropertyName();
						selectSql = $" SELECT Max({entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])})  ";
					}
					break;
				case ExpressionType.MemberInit:
					throw new DapperExtensionException("不支持该表达式类型");
			}

			return selectSql;
		}

		/// <summary>
		/// 解析查询最小值
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public override string ResolveMin(LambdaExpression selector)
		{
			if (selector == null)
				throw new ArgumentException("selector");
			var selectSql = "";

			switch (selector.NodeType)
			{
				case ExpressionType.Lambda:
				case ExpressionType.MemberAccess:
					{
						EntityObject entityObject = EntityCache.QueryEntity(selector.Parameters[0].Type);
						var memberName = selector.Body.GetCorrectPropertyName();
						selectSql = $" SELECT Min({entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])})  ";
					}
					break;
				case ExpressionType.MemberInit:
					throw new DapperExtensionException("不支持该表达式类型");
			}

			return selectSql;
		}
	}
}
