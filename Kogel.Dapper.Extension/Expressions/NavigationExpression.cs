using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Expressions
{
	public class NavigationExpression: ExpressionVisitor
	{
		/// <summary>
		/// sql指令
		/// </summary>
		public string SqlCmd { get; set; }
		/// <summary>
		/// 参数
		/// </summary>
		public DynamicParameters Param { get; set; }
		/// <summary>
		/// 返回类型
		/// </summary>
		public Type ReturnType { get; set; }
		/// <summary>
		/// 条件表达式
		/// </summary>
		public List<LambdaExpression> WhereExpression { get; set; }
		public NavigationExpression(Expression expression)
		{
			Visit(expression);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var subquery = new SubqueryExpression(node.Arguments[0] as MethodCallExpression ?? node);
			this.SqlCmd = subquery.SqlCmd;
			this.Param = subquery.Param;
			this.ReturnType = subquery.ReturnType;
			this.WhereExpression = subquery.WhereExpression;
			return node;
		}
	}
}
