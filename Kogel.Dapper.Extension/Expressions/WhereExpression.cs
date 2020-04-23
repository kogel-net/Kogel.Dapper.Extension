using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using System.Linq;

namespace Kogel.Dapper.Extension.Expressions
{
	/// <summary>
	/// 解析查询条件
	/// </summary>
	public sealed class WhereExpression : WhereExpressionVisitor
	{
		#region sql指令
		private readonly StringBuilder _sqlCmd;
		/// <summary>
		/// sql指令
		/// </summary>
		public string SqlCmd { get; }
		/// <summary>
		/// 参数
		/// </summary>
		public new DynamicParameters Param { get; }
		#endregion
		/// <summary>
		/// 解析条件对象
		/// </summary>
		/// <param name="expression">表达式</param>
		/// <param name="prefix">参数标记</param>
		/// <param name="providerOption"></param>
		public WhereExpression(LambdaExpression expression, string prefix, SqlProvider provider) : base(provider)
		{
			this._sqlCmd = new StringBuilder(100);
			this.Param = new DynamicParameters();
			this.providerOption = provider.ProviderOption;
			//开始解析对象
			Visit(expression);
			//开始拼接成条件
			this._sqlCmd.Append(base.SpliceField);
			this.SqlCmd = " AND " + this._sqlCmd.ToString();
			if (string.IsNullOrEmpty(prefix))
			{
				this.Param.AddDynamicParams(base.Param);
			}
			else
			{
				//加上参数标记
				foreach (var paramName in base.Param.ParameterNames.Reverse())
				{
					string newName = (paramName + prefix).Replace("_", "__");
					object value = base.Param.Get<object>(paramName);
					this.SqlCmd = this.SqlCmd.Replace(paramName, newName);
					this.Param.Add(newName, value);
				}
			}
		}

		/// <summary>
		/// 解析二元表达式
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitBinary(BinaryExpression node)
		{
			var binaryWhere = new BinaryExpressionVisitor(node, base.Provider);
			this._sqlCmd.Append(binaryWhere.SpliceField);
			base.Param.AddDynamicParams(binaryWhere.Param);
			return node;
		}

		/// <summary>
		/// 解析!不等于(!里只能包含一个条件)
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.Not)
			{
				this._sqlCmd.Append(" Not (");
				Visit(node.Operand);
				this._sqlCmd.Append(") ");
			}
			else
			{
				Visit(node.Operand);
			}
			return node;
		}
	}
}
