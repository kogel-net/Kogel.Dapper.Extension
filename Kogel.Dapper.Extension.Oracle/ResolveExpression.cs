using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Oracle
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
			//查询字段解析
			StringBuilder selectSql = new StringBuilder(" SELECT ");
			//去重
			if (abstractSet.IsDistinct)
				selectSql.Append(" DISTINCT ");

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
						string fieldName = $"{entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}";
						selectSql = $@" SELECT NVL(SUM({fieldName}),0)  ";
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
						string fieldName = $"{entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}";
						selectSql = $" SELECT Max({fieldName})  ";
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
						string fieldName = $"{entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}";
						selectSql = $" SELECT Min({fieldName})  ";
					}
					break;
				case ExpressionType.MemberInit:
					throw new DapperExtensionException("不支持该表达式类型");
			}

			return selectSql;
		}

        public override string ResolveBulkInsert<T>(IEnumerable<T> entites, string[] excludeFields)
        {
            var sqlBuilder = new StringBuilder();
            DynamicParameters parameters = new DynamicParameters();
            //当前数据索引
            int index = 0;
            foreach (var item in entites)
            {
#if NET45 || NET451
                var resolveInsertParamsAndValues = ResolveInsertParamsAndValues(item, excludeFields, index++);
                var tableName = resolveInsertParamsAndValues.Item1;
                var fieldStr = resolveInsertParamsAndValues.Item2;
                var paramStr = resolveInsertParamsAndValues.Item3;
                var parameter = resolveInsertParamsAndValues.Item4;
#else
                var (tableName, fieldStr, paramStr, parameter) = ResolveInsertParamsAndValues(item, excludeFields, index++);
#endif

                //增加字段(只加一次)
                if (sqlBuilder.Length == 0)
                {
                    sqlBuilder.Append($"INSERT ALL ");
                }
                //增加参数
                sqlBuilder.Append($"INTO {tableName} ({fieldStr}) Values({paramStr}) ");
                parameters.AddDynamicParams(parameter);
            }
            sqlBuilder.Append("SELECT 1 FROM DUAL");
			provider.Params.AddDynamicParams(parameters);
            return sqlBuilder.ToString();
        }

        public override string ResolveWithNoLock(bool nolock)
		{
			return "";
		}
    }
}
