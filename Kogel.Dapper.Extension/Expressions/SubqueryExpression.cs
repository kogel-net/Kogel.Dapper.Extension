using Dapper;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System;
using Kogel.Dapper.Extension.Core.SetQ;
using System.Collections.ObjectModel;

namespace Kogel.Dapper.Extension.Expressions
{
	/// <summary>
	/// 专门处理子查询的的表达式树扩展类
	/// </summary>
	public class SubqueryExpression : ExpressionVisitor
	{
		private MethodCallExpression expression;
		private List<ParameterExpression> parameterExpressions;
		private readonly StringBuilder _sqlCmd;
		/// <summary>
		/// sql指令
		/// </summary>
		public string SqlCmd => _sqlCmd.ToString();
		/// <summary>
		/// 参数
		/// </summary>
		public DynamicParameters Param;
		/// <summary>
		/// 返回类型
		/// </summary>
		public Type ReturnType { get; set; }
		#region Kogel对象
		/// <summary>
		/// 查询对象
		/// </summary>
		public object QuerySet { get; set; }
		/// <summary>
		/// 条件表达式
		/// </summary>
		public List<LambdaExpression> WhereExpression { get; set; }
		#endregion

		public SubqueryExpression(MethodCallExpression methodCallExpression)
		{
			this.expression = methodCallExpression;
			this._sqlCmd = new StringBuilder();
			this.Param = new DynamicParameters();
			this.WhereExpression = new List<LambdaExpression>();
			this.AnalysisExpression();
		}
		/// <summary>
		/// 解析表达式
		/// </summary>
		public void AnalysisExpression()
		{
			AnalysisKogelQuerySet(expression);
			AnalysisKogelExpression(expression);
			//动态执行，得到T类型
			typeof(SubqueryExpression)
						.GetMethod("FormatSend")
						.MakeGenericMethod(QuerySet.GetType().GenericTypeArguments[0])
						.Invoke(this, new object[] { QuerySet, this.expression.Method.Name });
		}
		/// <summary>
		/// 递归得到QuerySet
		/// </summary>
		/// <param name="methodCallExpression"></param>
		public void AnalysisKogelQuerySet(MethodCallExpression methodCallExpression)
		{
			switch (methodCallExpression.Method.Name)
			{
				case "QuerySet":
					{
						this.QuerySet = methodCallExpression.ToConvertAndGetValue();
						break;
					}
			}
			if (methodCallExpression.Object != null)
			{
				if (methodCallExpression.Object is MethodCallExpression)
				{
					var objectCallExpression = methodCallExpression.Object as MethodCallExpression;
					AnalysisKogelQuerySet(objectCallExpression);
				}
				else if (methodCallExpression.Object.Type.FullName.Contains("QuerySet"))
				{
					this.QuerySet = methodCallExpression.Object.ToConvertAndGetValue();
				}
			}
		}
		/// <summary>
		/// 递归解析导航查询表达式
		/// </summary>
		/// <param name="methodCallExpression"></param>
		/// <returns></returns>
		public void AnalysisKogelExpression(MethodCallExpression methodCallExpression)
		{
			switch (methodCallExpression.Method.Name)
			{
				case "Where":
					{
						foreach (UnaryExpression exp in methodCallExpression.Arguments)
						{
							this.parameterExpressions = new List<ParameterExpression>();
							Visit(exp);
							var lambda = Expression.Lambda(exp, parameterExpressions.ToList());
							this.WhereExpression.Add(lambda);
						}
						break;
					}
				case "WhereIf":
					{
						this.parameterExpressions = new List<ParameterExpression>();
						if (methodCallExpression.Arguments[0].ToConvertAndGetValue().Equals(true))
						{
							var trueExp = methodCallExpression.Arguments[1];
							Visit(trueExp);
							var lambda = Expression.Lambda(trueExp, parameterExpressions.ToList());
							this.WhereExpression.Add(lambda);
						}
						else
						{
							var falseExp = methodCallExpression.Arguments[2];
							Visit(falseExp);
							var lambda = Expression.Lambda(falseExp, parameterExpressions.ToList());
							this.WhereExpression.Add(lambda);
						}
						break;
					}
				default:
					{
						if ((methodCallExpression.Method.Name == "OrderBy" || methodCallExpression.Method.Name == "OrderByDescing"))
						{
							//设置参数和参数类型
							var method = methodCallExpression.Method;
							if (methodCallExpression.Arguments.Count != 0)
							{
								var paramType = (methodCallExpression.Arguments.First() as UnaryExpression).Operand.Type.GenericTypeArguments[0];
								if (paramType != typeof(string))
								{
									var lambda = methodCallExpression.Arguments[0].GetLambdaExpression();
									typeof(SubqueryExpression)
										.GetMethod("FormatSendOrder")
										.MakeGenericMethod(new Type[] { QuerySet.GetType().GenericTypeArguments[0] })
										.Invoke(this, new object[] { QuerySet, lambda, methodCallExpression.Method.Name });
									break;
								}
							}
						}
						//正常方法
						string[] methodArr = new string[] { "Count", "Sum", "Min", "Max", "Get", "ToList", "PageList" };
						if (!methodArr.Contains(methodCallExpression.Method.Name))
						{
							object[] parameters = methodCallExpression.Arguments.Select(x => x.ToConvertAndGetValue()).ToArray();
							methodCallExpression.Method.Invoke(this.QuerySet, parameters);
						}
						break;
					}
			}
			if (methodCallExpression.Object != null)
			{
				if (methodCallExpression.Object is MethodCallExpression)
				{
					var objectCallExpression = methodCallExpression.Object as MethodCallExpression;
					AnalysisKogelExpression(objectCallExpression);
				}
			}
		}
		/// <summary>
		/// 解析参数
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (!parameterExpressions.Exists(x => x.Name == node.Name))
			{
				ParameterExpression param = Expression.Parameter(node.Type, node.Name);
				parameterExpressions.Add(param);
			}
			return node;
		}
		/// <summary>
		/// 替换成新的参数名，防止命名冲突
		/// </summary>
		/// <param name="param"></param>
		/// <param name="sql"></param>
		/// <returns></returns>
		private DynamicParameters ToSubqueryParam(DynamicParameters param, ref string sql)
		{
			DynamicParameters newParam = new DynamicParameters();
			foreach (var paramName in param.ParameterNames)
			{
				string newName = paramName + "_Subquery";
				object value = param.Get<object>(paramName);
				newParam.Add(newName, value);
				sql = sql.Replace(paramName, newName);
			}
			return newParam;
		}
		/// <summary>
		/// 反射执行需要指向T类型的函数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sqlProvider"></param>
		/// <param name="methodName"></param>
		public void FormatSend<T>(QuerySet<T> querySet, string methodName)
		{
			SqlProvider sqlProvider = querySet.SqlProvider;
			//写入重新生成后的条件
			if (WhereExpression != null && WhereExpression.Any())
			{
				querySet.WhereExpressionList.AddRange(WhereExpression);
			}
			//因为表达式的原因，递归获取连表默认会倒序
			if (sqlProvider.JoinList.Any())
			{
				sqlProvider.JoinList.Reverse();
			}
			switch (methodName)
			{
				case "Count":
					{
						sqlProvider.FormatCount();
					}
					break;
				case "Sum":
					{
						var lambda = this.expression.Arguments[0].GetLambdaExpression();
						sqlProvider.FormatSum(lambda);
					}
					break;
				case "Min":
					{
						var lambda = this.expression.Arguments[0].GetLambdaExpression();
						sqlProvider.FormatMin(lambda);
					}
					break;
				case "Max":
					{
						var lambda = this.expression.Arguments[0].GetLambdaExpression();
						sqlProvider.FormatMax(lambda);
					}
					break;
				case "Get":
					{
						LambdaExpression lambda = default(LambdaExpression);
						if (this.expression.Arguments.Count == 1)
						{
							lambda = this.expression.Arguments[0].GetLambdaExpression();
							this.ReturnType = lambda.ReturnType;
						}
						else if (this.expression.Arguments.Count == 0)//无自定义列表返回
						{
							lambda = null;
							this.ReturnType = this.expression.Method.ReturnType;
						}
						else
						{
							//带if判断
							if (this.expression.Arguments[0].ToConvertAndGetValue().Equals(true))
							{
								lambda = this.expression.Arguments[1].GetLambdaExpression();
							}
							else
							{
								lambda = this.expression.Arguments[2].GetLambdaExpression();
							}
							this.ReturnType = lambda.ReturnType;
						}
						sqlProvider.Context.Set.SelectExpression = lambda;
						sqlProvider.FormatGet<T>();
					}
					break;
				case "ToList":
					{
						LambdaExpression lambda = default(LambdaExpression);
						if (this.expression.Arguments.Count == 1)
						{
							lambda = this.expression.Arguments[0].GetLambdaExpression();
							this.ReturnType = lambda.ReturnType;
						}
						else if (this.expression.Arguments.Count == 0)//无自定义列表返回
						{
							lambda = null;
							this.ReturnType = this.expression.Method.ReturnType.GenericTypeArguments[0];
						}
						else
						{
							//带if判断
							if (this.expression.Arguments[0].ToConvertAndGetValue().Equals(true))
							{
								lambda = this.expression.Arguments[1].GetLambdaExpression();
							}
							else
							{
								lambda = this.expression.Arguments[2].GetLambdaExpression();
							}
							this.ReturnType = lambda.ReturnType;
						}
						sqlProvider.Context.Set.SelectExpression = lambda;
						sqlProvider.FormatToList<T>();
					}
					break;
				default:
					throw new DapperExtensionException("Kogel.Dapper.Extension中子查询不支持的扩展函数");
			}
			//得到解析的sql和param对象
			string sql = sqlProvider.SqlString;
			var param = ToSubqueryParam(sqlProvider.Params, ref sql);
			_sqlCmd.Append(sql);
			this.Param.AddDynamicParams(param);
		}

		/// <summary>
		/// 反射执行多表联查的排序
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="querySet"></param>
		/// <param name="orderExpression"></param>
		/// <param name="methodName"></param>
		public void FormatSendOrder<T>(QuerySet<T> querySet, LambdaExpression orderExpression, string methodName)
		{
			if (methodName == "OrderBy")
				querySet.OrderbyExpressionList.Add(orderExpression, Model.EOrderBy.Asc);
			else
				querySet.OrderbyExpressionList.Add(orderExpression, Model.EOrderBy.Desc);
		}
	}
}
