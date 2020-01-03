using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

		public override string ResolveSelect(int? topNum)
		{
			//添加需要连接的导航表
			var masterEntity = EntityCache.QueryEntity(abstractSet.TableType);
			var navigationList = masterEntity.Navigations;
			if (navigationList.Any())
			{
				provider.JoinList.AddRange(ExpressionExtension.Clone(navigationList));
			}

			var selectFormat = " SELECT {0} ";
			var selectSql = "";
			if (provider.Context.Set.SelectExpression == null)
			{
				var propertyBuilder = GetTableField(masterEntity);
				selectSql = string.Format(selectFormat, propertyBuilder);
			}
			else
			{
				var selectExp = new SelectExpression(provider.Context.Set.SelectExpression, "", provider);
				selectSql = string.Format(selectFormat, selectExp.SqlCmd);
				provider.Params.AddDynamicParams(selectExp.Param);
			}
			return selectSql;
		}

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
						selectSql = $" SELECT IFNULL(SUM({entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}),0)  ";
					}
					break;
				case ExpressionType.MemberInit:
					throw new DapperExtensionException("不支持该表达式类型");
			}

			return selectSql;
		}

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



		public override string ResolveWithNoLock(bool nolock)
		{
			return "";
		}
	}
}
