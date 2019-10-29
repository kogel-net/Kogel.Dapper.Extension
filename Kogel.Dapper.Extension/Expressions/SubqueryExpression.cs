using Dapper;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using System;

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
		/// <summary>
		/// 条件表达式
		/// </summary>
		public List<LambdaExpression> WhereExpression { get; set; }

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
			MethodCallExpression methodCall = (MethodCallExpression)(expression.Object);
			//获取queryset对象
			var querySet = methodCall.Object.ToConvertAndGetValue();
            //获取queryset的类型
            var querySetEntity = EntityCache.QueryEntity(methodCall.Object.Type).Properties;
            //获取paramerer对象
            foreach (UnaryExpression exp in methodCall.Arguments)
            {
                this.parameterExpressions = new List<ParameterExpression>();
                Visit(exp);
                var lambda = Expression.Lambda(exp, parameterExpressions.ToList());
				WhereExpression.Add(lambda);
            }
            //写入条件集合
            var WhereExpressionList = querySetEntity.FirstOrDefault(x => x.Name.Equals("WhereExpressionList"));
            WhereExpressionList.SetValue(querySet, WhereExpression);
            dynamic querySetDynamic = querySet;
            //执行指定函数
            var newExpression = ((NewExpression)methodCall.Object).Arguments[1];
            var sqlProvider = (SqlProvider)(querySetDynamic.SqlProvider);
			//动态执行，得到T类型
			typeof(SubqueryExpression)
						.GetMethod("FormatSend")
						.MakeGenericMethod(sqlProvider.Context.Set.TableType)
						.Invoke(this, new object[] { sqlProvider, this.expression.Method.Name });
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
		public void FormatSend<T>(SqlProvider sqlProvider,string methodName)
		{
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
				case "ToList":
					{
						//加上自定义实体返回
						var lambda = this.expression.Arguments[0].GetLambdaExpression();
						this.ReturnType = lambda.ReturnType;
						sqlProvider.Context.Set.SelectExpression = lambda;
						sqlProvider.FormatToList<T>();
					}
					break;
				default:
					throw new DapperExtensionException("the expression is no support this function");
			}
			//得到解析的sql和param对象
			string sql = sqlProvider.SqlString;
			var param = ToSubqueryParam(sqlProvider.Params, ref sql);
			_sqlCmd.Append(sql);
			this.Param.AddDynamicParams(param);
		}
	}
}
