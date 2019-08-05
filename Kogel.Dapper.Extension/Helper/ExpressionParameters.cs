using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Helper
{
    /// <summary>
    /// 专门处理lambda参数的帮助类
    /// </summary>
    public class ExpressionParameters : ExpressionVisitor
    {
        private Expression expression;
        private List<ParameterExpression> parameterExpressions;
        public ExpressionParameters(Expression expression)
        {
            this.expression = expression; 
        }
        public Expression ConverParametersExpression()
        {
            List<Expression> unaryList = new List<Expression>();
            MethodCallExpression methodCall = (MethodCallExpression)expression;
            foreach (UnaryExpression exp in methodCall.Arguments)
            {
                this.parameterExpressions = new List<ParameterExpression>();
                Visit(expression);
                var lambda = Expression.Lambda(exp, parameterExpressions.ToList());
                UnaryExpression unary = exp.Update(lambda);
                unaryList.Add(unary);
            }
            var newMethodCall = methodCall.Update(methodCall.Object, unaryList);
            return methodCall;

        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!parameterExpressions.Exists(x => x.Name == node.Name))
            {
                ParameterExpression param = Expression.Parameter(node.Type, node.Name);
                parameterExpressions.Add(param);
            }
            return node;
        }
    }
}
