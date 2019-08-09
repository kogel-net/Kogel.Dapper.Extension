using Dapper;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace Kogel.Dapper.Extension.Expressions
{
    /// <summary>
    /// 专门处理子查询的的表达式树扩展类
    /// </summary>
    public class SubqueryExpression : ExpressionVisitor
    {
        private MethodCallExpression expression;
        private List<ParameterExpression> parameterExpressions;
        #region sql指令
        private readonly StringBuilder _sqlCmd;
        /// <summary>
        /// sql指令
        /// </summary>
        public string SqlCmd => _sqlCmd.ToString();
        /// <summary>
        /// 参数
        /// </summary>
        public DynamicParameters Param;
        #endregion
        public SubqueryExpression(MethodCallExpression methodCallExpression)
        {
            this.expression = methodCallExpression;
            this._sqlCmd = new StringBuilder();
            this.Param = new DynamicParameters();
            this.AnalysisExpression();
        }
        /// <summary>
        /// 解析表达式
        /// </summary>
        public void AnalysisExpression()
        {
            MethodCallExpression methodCall = (MethodCallExpression)(expression.Object);
            //获取queryset对象
            var querySet = methodCall.Object.ToConvertAndGetValue();
            //获取queryset的类型
            var querySetEntity = EntityCache.QueryEntity(methodCall.Object.Type).Properties;
            //获取paramerer对象
            List<LambdaExpression> lambdaList = new List<LambdaExpression>();
            foreach (UnaryExpression exp in methodCall.Arguments)
            {
                this.parameterExpressions = new List<ParameterExpression>();
                Visit(exp);
                var lambda = Expression.Lambda(exp, parameterExpressions.ToList());
                lambdaList.Add(lambda);
            }
            //写入条件集合
            var WhereExpressionList = querySetEntity.FirstOrDefault(x => x.Name.Equals("WhereExpressionList"));
            WhereExpressionList.SetValue(querySet, lambdaList);
            dynamic querySetDynamic = querySet;
            //执行指定函数
            var newExpression = ((NewExpression)methodCall.Object).Arguments[1];
            var sqlProvider = (SqlProvider)(querySetDynamic.SqlProvider);
            switch (this.expression.Method.Name)
            {
                case "Count":
                    {
                        sqlProvider.FormatCount();
                        string Sql = sqlProvider.SqlString;
                        var param = ToSubqueryParam(sqlProvider.Params, ref Sql);
                        _sqlCmd.Append(Sql);
                        this.Param.AddDynamicParams(param);
                    }
                    break;
                case "Sum":
                    {
                        var lambda = (LambdaExpression)(((UnaryExpression)(this.expression.Arguments[0])).Operand);
                        sqlProvider.FormatSum(lambda);
                        string Sql = sqlProvider.SqlString;
                        var param = ToSubqueryParam(sqlProvider.Params, ref Sql);
                        _sqlCmd.Append(Sql);
                        this.Param.AddDynamicParams(param);
                    }
                    break;
                default:
                    throw new DapperExtensionException("the expression is no support this function");
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
    }
}
