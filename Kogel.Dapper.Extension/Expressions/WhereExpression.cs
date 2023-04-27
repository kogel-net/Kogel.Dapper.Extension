using System.Linq.Expressions;
using System.Text;
using Kogel.Dapper.Extension;
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
            this.ProviderOption = provider.ProviderOption;
            base.Prefix = prefix;
            //开始解析对象
            Visit(expression);
            //开始拼接成条件
            this._sqlCmd.Append(base._sqlBuilder);
            this.SqlCmd = " AND " + this._sqlCmd.ToString();
            this.Param.AddDynamicParams(base.Param);
        }

        /// <summary>
        /// 解析二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binaryWhere = new BinaryExpressionVisitor(node, base.Provider, 0, base.Prefix);
            this._sqlCmd.Append(binaryWhere._sqlBuilder);
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
                base._sqlBuilder.Append(" Not (");
                Visit(node.Operand);
                base._sqlBuilder.Append(") ");
            }
            else
            {
                Visit(node.Operand);
            }
            return node;
        }
    }
}
