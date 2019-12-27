using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Kogel.Dapper.Extension.Helper;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Exception;
using System;

namespace Kogel.Dapper.Extension.Expressions
{
	/// <summary>
	/// 解析自定义查询字段
	/// </summary>
	public class SelectExpression : BaseExpressionVisitor
	{
		#region sql指令
		private readonly StringBuilder _sqlCmd;
		/// <summary>
		/// sql指令
		/// </summary>
		public string SqlCmd => _sqlCmd.ToString();
		/// <summary>
		/// 参数
		/// </summary>
		public new DynamicParameters Param;
		#endregion
		/// <summary>
		/// 执行解析
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="prefix">字段前缀</param>
		/// <param name="providerOption"></param>
		public SelectExpression(LambdaExpression expression, string prefix, SqlProvider provider) : base(provider)
		{
			this._sqlCmd = new StringBuilder();
			this.Param = new DynamicParameters();
			//判断是不是实体类
			if (expression.Body is MemberInitExpression)
			{
				var memberInitExpression = expression.Body as MemberInitExpression;
				foreach (MemberAssignment memberInit in memberInitExpression.Bindings)
				{
					base.SpliceField.Clear();
					base.Param = new DynamicParameters();
					if (_sqlCmd.Length != 0)
						_sqlCmd.Append(",");
					//实体类型
					Type entityType;
					//验证是实体类或者是泛型
					if (ExpressionExtension.IsAnyBaseEntity(memberInit.Expression.Type, out entityType))
					{
						var itemJoin = provider.JoinList.FirstOrDefault(x => x.TableType == entityType);
						if (itemJoin != null)
						{
							itemJoin.IsMapperField = true;
							//当前定义的查询返回对象
							EntityObject entity = EntityCache.QueryEntity(entityType);
							itemJoin.SelectFieldPairs = entity.FieldPairs;
						}
					}
					else
					{
						//值对象
						Visit(memberInit.Expression);
						_sqlCmd.Append($"{base.SpliceField} AS {memberInit.Member.Name}");
						Param.AddDynamicParams(base.Param);
					}
					base.Index++;
				}
			}
			else//匿名类
			{
				//当前定义的查询返回对象
				EntityObject entity = EntityCache.QueryEntity(expression.Body.Type);
				var newExpression = expression.Body as NewExpression;
				if (newExpression != null)
				{
					foreach (var argument in newExpression.Arguments)
					{
						base.SpliceField.Clear();
						base.Param = new DynamicParameters();
						if (_sqlCmd.Length != 0)
							_sqlCmd.Append(",");
						//返回类型
						var returnProperty = entity.Properties[base.Index];
						//实体类型
						Type entityType;
						//验证是实体类或者是泛型
						if (ExpressionExtension.IsAnyBaseEntity(returnProperty.PropertyType, out entityType))
						{
							throw new DapperExtensionException("匿名类型不支持导航属性!");
						}
						else
						{
							//值对象
							Visit(argument);
							_sqlCmd.Append($" {base.SpliceField} AS {returnProperty.Name} ");
							Param.AddDynamicParams(base.Param);
						}
						base.Index++;
					}
				}
				else
				{
					//单个属性，例如 .Get(x => x.Id);
					Visit(expression.Body);
					_sqlCmd.Append(base.SpliceField);
					Param.AddDynamicParams(base.Param);
				}
			}
		}
	}
}
