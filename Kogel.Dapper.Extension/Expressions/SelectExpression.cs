using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Kogel.Dapper.Extension.Expressions
{
    public class SelectExpression : ExpressionVisitor
    {
        #region sql指令
        private readonly StringBuilder _sqlCmd;
        /// <summary>
        /// sql指令
        /// </summary>
        public string SqlCmd => _sqlCmd.ToString();

        public DynamicParameters Param { get; }

        private readonly string _prefix;

        private readonly char _parameterPrefix;

        private readonly string _closeQuote;

        private readonly string _openQuote;

        #endregion
        #region 当前解析的对象
        private EntityObject entity { get; }
        private int protiesIndex;
        private string[] fieldArr { get; }//字段数组
        #endregion
        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="prefix">字段前缀</param>
        /// <param name="providerOption"></param>
        public SelectExpression(LambdaExpression expression, string prefix, IProviderOption providerOption)
        {
            _sqlCmd = new StringBuilder(100);
            Param = new DynamicParameters();

            _prefix = prefix;
            _parameterPrefix = providerOption.ParameterPrefix;
            _openQuote = providerOption.OpenQuote;
            _closeQuote = providerOption.CloseQuote;
            //当前定义的查询返回对象
            entity = EntityCache.QueryEntity(expression.Body.Type);
            protiesIndex = 0;
            //判断是不是实体类
            if (expression.Body is MemberInitExpression)
            {
                fieldArr = ((MemberInitExpression)expression.Body).Bindings.AsList().Select(x => entity.FieldPairs[x.Member.Name]).ToArray();
            }
            else//匿名类
            {
                fieldArr = entity.Properties.Select(x => x.Name).ToArray();
            }
            Visit(expression);
        }

        /// <summary>
        /// 解析绑定字段
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            var member = EntityCache.QueryEntity(node.Member.DeclaringType);
            if (_sqlCmd.Length != 0)
                _sqlCmd.Append(",");
            //设置值对象
            _sqlCmd.Append(_openQuote + member.Name + _closeQuote + ".");
            string fieldName = member.FieldPairs[node.Member.Name];
            _sqlCmd.Append(_openQuote + fieldName + _closeQuote);
            //设置字段对象
            _sqlCmd.Append($" as { _openQuote + fieldArr[protiesIndex++] + _closeQuote} ");
            return node;
        }
        /// <summary>
        /// 解析绑定固定数据
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (_sqlCmd.Length != 0)
                _sqlCmd.Append(",");
            //设置值对象
            _sqlCmd.Append(node.Value);

            //设置字段对象
            _sqlCmd.Append($" as { _openQuote + fieldArr[protiesIndex++] + _closeQuote} ");
            return node;
        }
        /// <summary>
        /// 解析字段的值(表达式)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType.FullName.Contains("Convert"))//使用convert函数里待执行的sql数据
            {
                Visit(node.Arguments[0]);
            }
            else//使用系统自带需要计算的函数
            {
                if (_sqlCmd.Length != 0)
                    _sqlCmd.Append(",");

                //设置值对象
                _sqlCmd.Append(node.ToConvertAndGetValue());

                //设置字段对象
                _sqlCmd.Append($" as { _openQuote + fieldArr[protiesIndex++] + _closeQuote} ");
            }
            return node;
        }
    }
}
