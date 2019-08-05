using Kogel.Dapper.Extension.Attributes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Core.Interfaces;

namespace Kogel.Dapper.Extension.Expressions
{
    public sealed class UpdateExpression : ExpressionVisitor
    {
        #region sql指令
        private readonly StringBuilder _sqlCmd;
        /// <summary>
        /// sql指令
        /// </summary>
        public string SqlCmd => _sqlCmd.ToString();

        public DynamicParameters Param { get; }

        private IProviderOption providerOption { get; set; }

        #endregion
        #region 当前解析的对象
        private EntityObject entity { get; }
        private int protiesIndex;
        private string[] fieldArr { get; }//字段数组
        #endregion
        /// <inheritdoc />
        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateExpression(LambdaExpression expression, IProviderOption providerOption)
        {
            _sqlCmd = new StringBuilder(100);
            Param = new DynamicParameters();
            this.providerOption = providerOption;
            //当前定义的查询返回对象
            entity = EntityCache.QueryEntity(expression.Body.Type);
            protiesIndex = 0;
            fieldArr = ((MemberInitExpression)expression.Body).Bindings.AsList().Select(x => entity.FieldPairs[x.Member.Name]).ToArray();

            Visit(expression);
            _sqlCmd.Insert(0, " SET ");
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
            //设置字段对象
            string field = fieldArr[protiesIndex++];
            _sqlCmd.Append($"{ providerOption.CombineFieldName(field)}={providerOption.ParameterPrefix + "UPDATE_" + field}");
            //设置值对象
            Param.Add("UPDATE_" + field, node.Value);
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

                //设置字段对象
                string field = fieldArr[protiesIndex++];
                _sqlCmd.Append($"{ providerOption.CombineFieldName(field)}={providerOption.ParameterPrefix + "UPDATE_" + field}");
                //设置值对象
                Param.Add("UPDATE_" + field, node.ToConvertAndGetValue());
            }
            return node;
        }
    }
}
