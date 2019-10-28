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
		public string SqlCmd;
		/// <summary>
		/// 参数
		/// </summary>
		public DynamicParameters Param;
		/// <summary>
		/// 返回类型
		/// </summary>
		public Type ReturnType { get; set; }
		public NavigationExpression(Expression expression)
		{
			Visit(expression);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var subquery = new SubqueryExpression(node.Arguments[0] as MethodCallExpression);
			this.SqlCmd = subquery.SqlCmd;
			this.Param = subquery.Param;
			this.ReturnType = subquery.ReturnType;
			return node;
		}
	}
}
