using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Kogel.Dapper.Extension.Expressions
{
	/// <summary>
	/// 实现表达式解析的基类
	/// </summary>
	public class BaseExpressionVisitor : ExpressionVisitor
	{
		/// <summary>
		/// 字段集合
		/// </summary>
		internal List<string> FieldList { get; set; }
		protected DynamicParameters Param { get; set; }
		protected IProviderOption providerOption { get; set; }
		public BaseExpressionVisitor(IProviderOption providerOption)
		{
			this.FieldList = new List<string>();
			this.Param = new DynamicParameters();
			this.providerOption = providerOption;
		}
		/// <summary>
		/// 有+ - * /需要拼接的对象
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitBinary(BinaryExpression node)
		{
			var binary = new BinaryExpressionVisitor(node, providerOption);
			GenerateField(binary.SpliceField.ToString());
			this.Param.AddDynamicParams(binary.Param);
			return node;
		}
		/// <summary>
		/// 值对象
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitConstant(ConstantExpression node)
		{
			//参数
			string paramName = $"{providerOption.ParameterPrefix}Member_Param_{Param.ParameterNames.Count()}";
			//值
			object nodeValue = node.ToConvertAndGetValue();
			//设置sql
			GenerateField(paramName);
			Param.Add(paramName, nodeValue);
			return node;
		}
		/// <summary>
		/// 成员对象
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitMember(MemberExpression node)
		{
			//需要计算的字段值
			var expTypeName = node.Expression?.GetType().FullName ?? "";
			if (expTypeName == "System.Linq.Expressions.TypedParameterExpression")
			{
				var member = EntityCache.QueryEntity(node.Expression.Type);
				string fieldName = member.FieldPairs[node.Member.Name];
				string field = (providerOption.IsAsName ? member.GetAsName(providerOption) : "") + providerOption.CombineFieldName(fieldName);
				GenerateField(field);
			}
			else
			{
				//参数
				string paramName = $"{providerOption.ParameterPrefix}Member_Param_{Param.ParameterNames.Count()}";
				//值
				object nodeValue = node.ToConvertAndGetValue();
				//设置sql
				GenerateField(paramName);
				Param.Add(paramName, nodeValue);
			}
			return node;
		}
		/// <summary>
		/// 待执行的方法对象
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			//保存导航查询的属性
			if (node.Method.ReturnType.FullName.Contains("System.Collections.Generic.List") || node.Method.ReturnType.BaseType.FullName.Contains("Kogel.Dapper.Extension.IBaseEntity"))
			{
				return node;
			}
			if (node.Method.DeclaringType.FullName.Contains("Convert"))//使用convert函数里待执行的sql数据
			{
				Visit(node.Arguments[0]);
			}
			else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
			{
				DynamicParameters parameters = new DynamicParameters();
				GenerateField("(" + node.MethodCallExpressionToSql(ref parameters) + ")");
				Param.AddDynamicParams(parameters);
			}
			else
			{
				GenerateField(GetFieldValue(node));
			}
			return node;
		}
		/// <summary>
		/// 生成字段
		/// </summary>
		/// <param name="field"></param>
		protected virtual void GenerateField(string field)
		{
			FieldList.Add(field);
		}
		/// <summary>
		/// 获取字段的值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string GetFieldValue(Expression expression)
		{
			object value = expression.ToConvertAndGetValue();
			if (value == null)
			{
				value = "NULL";
			}
			return value.ToString();
		}
	}
	public class WhereExpressionVisitor : BaseExpressionVisitor
	{
		private string FieldName { get; set; }//字段
		private string ParamName { get => (providerOption.ParameterPrefix + FieldName?.Replace(".", "_") + "_" + Param.ParameterNames.Count()); }//带参数标识的
		internal StringBuilder SpliceField { get; set; }
		internal new DynamicParameters Param { get; set; }
		public WhereExpressionVisitor(IProviderOption providerOption) : base(providerOption)
		{
			this.SpliceField = new StringBuilder();
			this.Param = new DynamicParameters();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			//使用convert函数里待执行的sql数据
			if (node.Method.DeclaringType.FullName.Contains("Convert"))
			{
				Visit(node.Arguments[0]);
			}
			else if (node.Method.DeclaringType.FullName.Equals("Kogel.Dapper.Extension.ExpressExpansion"))//自定义扩展方法
			{
				//Visit(node.Arguments[0]);
				Operation(node);
			}
			else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
			{
				base.VisitMethodCall(node);
				foreach (var item in base.FieldList)
				{
					SpliceField.Append(item);
				}
				this.Param.AddDynamicParams(base.Param);
			}
			else
			{
				Operation(node);
			}
			return node;
		}
		/// <summary>
		/// 处理判断字符
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.Not)
			{
				SpliceField.Append("NOT(");
				Visit(node.Operand);
				SpliceField.Append(")");
			}
			else
			{
				Visit(node.Operand);
			}
			return node;
		}
		/// <summary>
		/// 重写成员对象，得到字段名称
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitMember(MemberExpression node)
		{
			var expTypeName = node.Expression?.GetType().FullName ?? "";
			if (expTypeName == "System.Linq.Expressions.TypedParameterExpression")
			{
				var member = EntityCache.QueryEntity(node.Member.DeclaringType);
				string asName = string.Empty;
				if (providerOption.IsAsName)
				{
					this.FieldName = member.AsName + "." + member.FieldPairs[node.Member.Name];
					asName = member.GetAsName(providerOption);
				}
				else
				{
					this.FieldName = member.FieldPairs[node.Member.Name];
				}
				string fieldName = asName + providerOption.CombineFieldName(member.FieldPairs[node.Member.Name]);
				SpliceField.Append(fieldName);
			}
			else
			{
				SpliceField.Append(ParamName);
				object nodeValue = node.ToConvertAndGetValue();
				Param.Add(ParamName, nodeValue);
			}
			return node;
		}
		/// <summary>
		/// 重写值对象，记录参数
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (FieldName != null)
			{
				SpliceField.Append(ParamName);
				Param.Add(ParamName, node.ToConvertAndGetValue());
			}
			else
			{
				var nodeValue = node.ToConvertAndGetValue();
				if (nodeValue.Equals(true))
				{
					SpliceField.Append("1=1");
				}
				else if (nodeValue.Equals(false))
				{
					SpliceField.Append("1!=1");
				}
				else
				{
					SpliceField.Append(ParamName);
					Param.Add(ParamName, nodeValue);
				}
			}
			return node;
		}
		/// <summary>
		/// 解析函数
		/// </summary>
		/// <param name="node"></param>
		private void Operation(MethodCallExpression node)
		{
			switch (node.Method.Name)
			{
				case "Contains":
					{
						if (node.Arguments[0].Type.FullName == "System.String")
						{
							Visit(node.Object);
							this.SpliceField.Append(" LIKE ");
							Visit(node.Arguments);
						}
						else
						{
							if (node.Object != null)
							{
								Visit(node.Arguments[0]);
								this.SpliceField.Append(" IN ");
								Visit(node.Object);
							}
							else
							{
								Visit(node.Arguments[1]);
								this.SpliceField.Append($" IN {ParamName}");
								//这里只能手动记录参数
								var nodeValue = node.Arguments[0].ToConvertAndGetValue();
								Param.Add(ParamName, nodeValue);
							}
						}
					}
					break;
				case "Equals":
					{
						if (node.Object != null)
						{
							Visit(node.Object);
							this.SpliceField.Append($" = ");
							Visit(node.Arguments[0]);
						}
						else
						{
							Visit(node.Arguments[0]);
							this.SpliceField.Append($" = ");
							Visit(node.Arguments[1]);
						}
					}
					break;
				case "In":
					{
						Visit(node.Arguments[0]);
						this.SpliceField.Append($" IN {ParamName}");
						object value = node.Arguments[1].ToConvertAndGetValue();
						Param.Add(ParamName, value);
					}
					break;
				case "NotIn":
					{
						Visit(node.Arguments[0]);
						this.SpliceField.Append($" NOT IN {ParamName}");
						object value = node.Arguments[1].ToConvertAndGetValue();
						Param.Add(ParamName, value);
					}
					break;
				case "IsNull":
					{
						Visit(node.Arguments[0]);
						this.SpliceField.Append(" IS NULL");
					}
					break;
				case "IsNotNull":
					{
						Visit(node.Arguments[0]);
						this.SpliceField.Append(" IS NOT NULL");
					}
					break;
				case "Between":
					{
						if (node.Object != null)
						{
							Visit(node.Object);
							SpliceField.Append(" BETWEEN ");
							Visit(node.Arguments[0]);
							SpliceField.Append(" AND ");
							Visit(node.Arguments[1]);
						}
						else
						{
							Visit(node.Arguments[0]);
							SpliceField.Append(" BETWEEN ");
							Visit(node.Arguments[1]);
							SpliceField.Append(" AND ");
							Visit(node.Arguments[2]);
						}
					}
					break;
				#region Convert转换计算
				case "ToInt32":
				case "ToString":
				case "ToDecimal":
				case "ToDouble":
					{
						Visit(node.Object);
						if (node.Arguments.Count > 0)//Convert.ToInt32("xxx") or Convert.ToString("xxx")
						{
							this.SpliceField.AppendFormat(" {0}", ParamName);
							var argumentExpression = (ConstantExpression)node.Arguments[0];
							Param.Add(ParamName, argumentExpression.ToConvertAndGetValue());
						}
						else
						{
							//xxx.ToString
						}
					}
					break;
				#endregion
				#region 时间计算
				case "AddYears":
				case "AddMonths":
				case "AddDays":
				case "AddHours":
				case "AddMinutes":
				case "AddSeconds":
					{
						var dateOption = (DateOption)Enum.Parse(typeof(DateOption), node.Method.Name);
						providerOption.CombineDate(dateOption, SpliceField,
							() =>
							{
								Visit(node.Object);
							},
							() =>
							{
								Visit(node.Arguments);
							});
					}
					break;
				#endregion
				default:
					throw new DapperExtensionException("Kogel.Dapper.Extension不支持此功能");
			}
		}
	}
	/// <summary>
	/// 转用于解析二元表达式
	/// </summary>
	public class BinaryExpressionVisitor : WhereExpressionVisitor
	{
		public BinaryExpressionVisitor(BinaryExpression expression, IProviderOption providerOption) : base(providerOption)
		{
			SpliceField = new StringBuilder();
			Param = new DynamicParameters();
			SpliceField.Append("(");
			Visit(expression);
			SpliceField.Append(")");
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			SpliceField.Append("(");
			Visit(node.Left);
			SpliceField.Append(node.GetExpressionType());
			Visit(node.Right);
			SpliceField.Append(")");
			return node;
		}
	}
}
