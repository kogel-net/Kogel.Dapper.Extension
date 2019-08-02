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

        private const string Prefix = "UPDATE_";

        /// <summary>
        /// sql指令
        /// </summary>
        public string SqlCmd => _sqlCmd.Length > 0 ? $" SET {_sqlCmd} " : "";

        private readonly IProviderOption _providerOption;

        private readonly char _parameterPrefix;

        public DynamicParameters Param { get; }

        #endregion

        #region 执行解析

        /// <inheritdoc />
        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateExpression(LambdaExpression expression, IProviderOption providerOption)
        {
            _sqlCmd = new StringBuilder(100);
            _providerOption = providerOption;
            _parameterPrefix = _providerOption.ParameterPrefix;
            Param = new DynamicParameters();

            Visit(expression);
        }

        #endregion

        protected override System.Linq.Expressions.Expression VisitMember(MemberExpression node)
        {
            var memberInitExpression = node;

            var entity = ((ConstantExpression) TrimExpression.Trim(memberInitExpression)).Value;

            EntityObject entityModel = EntityCache.QueryEntity(memberInitExpression.Type);

            foreach (var item in entityModel.Properties)
            {
                if (item.CustomAttributes.Any(b => b.AttributeType == typeof(Identity)))
                    continue;

                if (_sqlCmd.Length > 0)
                    _sqlCmd.Append(",");

                var paramName = entityModel.FieldPairs[item.Name];
                var value = item.GetValue(entity);
                var fieldName = _providerOption.CombineFieldName(paramName);
                SetParam(fieldName, paramName, value);
            }

            return node;
        }


        protected override System.Linq.Expressions.Expression VisitMemberInit(MemberInitExpression node)
        {
            var memberInitExpression = node;

            foreach (var item in memberInitExpression.Bindings)
            {
                var memberAssignment = (MemberAssignment) item;

                if (_sqlCmd.Length > 0)
                    _sqlCmd.Append(",");

                var entityModel = EntityCache.QueryEntity(memberAssignment.Member.DeclaringType);
                var paramName = entityModel.FieldPairs[memberAssignment.Member.GetColumnAttributeName()] ;
                var fieldName = _providerOption.CombineFieldName(paramName);
                var constantExpression = (ConstantExpression) memberAssignment.Expression;
                SetParam(fieldName, paramName, constantExpression.Value);
            }

            return node;
        }

        private void SetParam(string fieldName, string paramName, object value)
        {
            var n = $"{_parameterPrefix}{Prefix}{paramName}";
            _sqlCmd.AppendFormat(" {0}={1} ", fieldName, n);
            Param.Add(n, value);
        }
    }
}
