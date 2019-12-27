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
		/// 字段sql
		/// </summary>
		internal StringBuilder SpliceField { get; set; }
		/// <summary>
		/// 参数
		/// </summary>
		protected DynamicParameters Param { get; set; }
		/// <summary>
		/// 解析提供方
		/// </summary>
		protected SqlProvider Provider { get; set; }
		/// <summary>
		/// 解析第n个下标
		/// </summary>
		protected int Index { get; set; }
		/// <summary>
		/// 提供方选项
		/// </summary>
		protected IProviderOption providerOption;

		public BaseExpressionVisitor(SqlProvider provider)
		{
			SpliceField = new StringBuilder();
			this.Param = new DynamicParameters();
			this.Provider = provider;
			this.providerOption = provider.ProviderOption;
		}
		/// <summary>
		/// 有+ - * /需要拼接的对象
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitBinary(BinaryExpression node)
		{
			var binary = new BinaryExpressionVisitor(node, Provider);
			SpliceField.Append(binary.SpliceField);
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
			string paramName = $"{providerOption.ParameterPrefix}Member_Param_{Index}_{Param.ParameterNames.Count()}";
			//值
			object nodeValue = node.ToConvertAndGetValue();
			//设置sql
			SpliceField.Append(paramName);
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
			if (expTypeName == "System.Linq.Expressions.TypedParameterExpression" || expTypeName == "System.Linq.Expressions.PropertyExpression")
			{
				var member = EntityCache.QueryEntity(node.Expression.Type);
				string fieldName = member.FieldPairs[node.Member.Name];
				string field = (providerOption.IsAsName ? member.GetAsName(providerOption) : "") + providerOption.CombineFieldName(fieldName);
				SpliceField.Append(field);
			}
			else
			{
				//参数
				string paramName = $"{providerOption.ParameterPrefix}Member_Param_{Param.ParameterNames.Count()}";
				//值
				object nodeValue = node.ToConvertAndGetValue();
				//设置sql
				SpliceField.Append(paramName);
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
			else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension.Function"))//系统函数
			{
				Operation(node);
			}
			else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
			{
				DynamicParameters parameters = new DynamicParameters();
				SpliceField.Append("(" + node.MethodCallExpressionToSql(ref parameters) + ")");
				Param.AddDynamicParams(parameters);
			}
			else
			{
				Operation(node);
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
				#region Convert转换计算
				case "ToInt32":
				case "ToString":
				case "ToDecimal":
				case "ToDouble":
				case "ToBoolean":
				case "ToDateTime":
					{
						var convertOption = (ConvertOption)Enum.Parse(typeof(ConvertOption), node.Method.Name);
						providerOption.CombineConvert(convertOption, SpliceField, () =>
						{
							Visit(node.Object != null ? node.Object : node.Arguments[0]);
						});
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
				#region 字符处理
				case "ToLower":
					{
						providerOption.ToLower(SpliceField,
							() =>
							{
								if (node.Object != null)
									Visit(node.Object);
								else
									Visit(node.Arguments);
							});
					}
					break;
				case "ToUpper":
					{
						providerOption.ToUpper(SpliceField,
							() =>
							{
								if (node.Object != null)
									Visit(node.Object);
								else
									Visit(node.Arguments);
							});
					}
					break;
				case "Replace":
					{

						SpliceField.Append("Replace(");
						Visit(node.Object);
						SpliceField.Append(",");
						Visit(node.Arguments[0]);
						SpliceField.Append(",");
						Visit(node.Arguments[1]);
						SpliceField.Append(")");
					}
					break;
				case "Trim":
					{
						SpliceField.Append("Trim(");
						Visit(node.Object);
						SpliceField.Append(")");
					}
					break;
				#endregion
				#region 聚合函数
				case "Count":
					{
						providerOption.Count(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Sum":
					{
						providerOption.Sum(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Max":
					{
						providerOption.Max(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Min":
					{
						providerOption.Min(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Avg":
					{
						providerOption.Avg(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				#endregion
				default:
					SpliceField.Append(node.ToConvertAndGetValue());
					break;
			}
		}
	}
	/// <summary>
	/// 用于解析条件表达式
	/// </summary>
	public class WhereExpressionVisitor : BaseExpressionVisitor
	{
		private string FieldName { get; set; }//字段
		private string ParamName { get => ($"{providerOption.ParameterPrefix}{FieldName?.Replace(".", "_")}_{Param.ParameterNames.Count()}"); }//带参数标识的
		internal new StringBuilder SpliceField { get; set; }
		internal new DynamicParameters Param { get; set; }
		public WhereExpressionVisitor(SqlProvider provider) : base(provider)
		{
			this.SpliceField = new StringBuilder();
			this.Param = new DynamicParameters();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			//使用convert函数里待执行的sql数据
			if (node.Method.DeclaringType.FullName.Equals("Kogel.Dapper.Extension.ExpressExpansion"))//自定义扩展方法
			{
				Operation(node);
			}
			else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension.Function"))//系统函数
			{
				Operation(node);
			}
			else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
			{
				base.VisitMethodCall(node);
				this.SpliceField.Append(base.SpliceField);
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
			if (expTypeName == "System.Linq.Expressions.TypedParameterExpression" || expTypeName == "System.Linq.Expressions.PropertyExpression")
			{
				var member = EntityCache.QueryEntity(node.Expression.Type);
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
						if (node.Object != null && !(node.Object.Type.FullName.Contains("System.Collections.Generic")))
						{
							Visit(node.Object);
							var value = node.Arguments[0].ToConvertAndGetValue();
							string param = ParamName;
							value = providerOption.FuzzyEscaping(value, ref param);
							this.SpliceField.Append($" LIKE {param}");
							Param.Add(ParamName, value);
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
				case "IsNullOrEmpty":
					{
						this.SpliceField.Append("(");
						Visit(node.Arguments[0]);
						this.SpliceField.Append(" IS NULL AND ");
						Visit(node.Arguments[0]);
						this.SpliceField.Append(" !=''");
						this.SpliceField.Append(")");
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
				case "Any":
					{
						Type entityType;
						if (ExpressionExtension.IsAnyBaseEntity(node.Arguments[0].Type, out entityType))
						{
							//导航属性有条件时设置查询该导航属性
							var joinTable = Provider.JoinList.Find(x => x.TableType == entityType);
							joinTable.IsMapperField = true;
							//解析导航属性条件
							var binaryExp = (node.Arguments[1] as LambdaExpression)?.Body as BinaryExpression;
							if (binaryExp != null)
							{
								var binaryVisitor = new BinaryExpressionVisitor(binaryExp, Provider);
								this.SpliceField.Append(binaryVisitor.SpliceField.Replace("=", "In").Replace("!", "Not "));
								//添加参数
								foreach (string paramName in binaryVisitor.Param.ParameterNames)
								{
									var objectParam = new object[] { binaryVisitor.Param.Get<object>(paramName) };
									this.Param.Add(paramName, objectParam);
								}
							}
						}
						else
						{
							throw new DapperExtensionException("导航属性类需要继承IBaseEntity");
						}
					}
					break;
				#region Convert转换计算
				case "ToInt32":
				case "ToString":
				case "ToDecimal":
				case "ToDouble":
				case "ToBoolean":
				case "ToDateTime":
					{
						var convertOption = (ConvertOption)Enum.Parse(typeof(ConvertOption), node.Method.Name);
						providerOption.CombineConvert(convertOption, SpliceField, () =>
						{
							Visit(node.Object != null ? node.Object : node.Arguments[0]);
						});
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
				#region 字符处理
				case "ToLower":
					{
						providerOption.ToLower(SpliceField,
							() =>
							{
								if (node.Object != null)
									Visit(node.Object);
								else
									Visit(node.Arguments);
							});
					}
					break;
				case "ToUpper":
					{
						providerOption.ToUpper(SpliceField,
							() =>
							{
								if (node.Object != null)
									Visit(node.Object);
								else
									Visit(node.Arguments);
							});
					}
					break;
				case "Replace":
					{

						SpliceField.Append("Replace(");
						Visit(node.Object);
						SpliceField.Append(",");
						Visit(node.Arguments[0]);
						SpliceField.Append(",");
						Visit(node.Arguments[1]);
						SpliceField.Append(")");
					}
					break;
				case "Trim":
					{
						SpliceField.Append("Trim(");
						Visit(node.Object);
						SpliceField.Append(")");
					}
					break;
				#endregion
				#region 聚合函数
				case "Count":
					{
						providerOption.Count(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Sum":
					{
						providerOption.Sum(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Max":
					{
						providerOption.Max(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Min":
					{
						providerOption.Min(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				case "Avg":
					{
						providerOption.Avg(SpliceField, () =>
						{
							Visit(node.Arguments);
						});
					}
					break;
				#endregion
				default:
					{
						if (node.Object != null)
							Visit(node.Object);
						else
							Visit(node.Arguments);
					}
					break;
			}
		}
	}
	/// <summary>
	/// 用于解析二元表达式
	/// </summary>
	public class BinaryExpressionVisitor : WhereExpressionVisitor
	{
		public BinaryExpressionVisitor(BinaryExpression expression, SqlProvider provider) : base(provider)
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
