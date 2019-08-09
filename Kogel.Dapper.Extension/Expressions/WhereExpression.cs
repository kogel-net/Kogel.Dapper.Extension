using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Expressions
{
    public sealed class WhereExpression : ExpressionVisitor
    {
        #region sql指令

        private readonly StringBuilder _sqlCmd;

        /// <summary>
        /// sql指令
        /// </summary>
        public string SqlCmd => _sqlCmd.Length > 0 ? $" AND {_sqlCmd} " : "";

        public DynamicParameters Param { get; }

        private string _tempFieldName;

        private string TempFieldName
        {
            get => _prefix + _tempFieldName+ "_" + Param.ParameterNames.Count();
            set => _tempFieldName = value;
        }

        private string ParamName => providerOption.ParameterPrefix + TempFieldName;

        private readonly string _prefix;

        private readonly IProviderOption providerOption;

        private string _closeQuote => providerOption.CloseQuote;

        private string _openQuote => providerOption.OpenQuote;

        #endregion

        #region 执行解析

        /// <inheritdoc />
        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="prefix">字段前缀</param>
        /// <param name="providerOption"></param>
        /// <returns></returns>
        public WhereExpression(LambdaExpression expression, string prefix, IProviderOption providerOption)
        {
            _sqlCmd = new StringBuilder(100);
            Param = new DynamicParameters();

            _prefix = prefix;
            this.providerOption = providerOption;
            var exp = TrimExpression.Trim(expression);
            Visit(exp);
        }
        #endregion

        #region 访问成员表达式

        /// <inheritdoc />
        /// <summary>
        /// 访问成员表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            EntityObject entity = EntityCache.QueryEntity(node.Member.DeclaringType);
            _sqlCmd.Append(entity.Name+".");
            string fieldName = entity.FieldPairs[node.Member.Name];
            _sqlCmd.Append(_openQuote + fieldName + _closeQuote);
            TempFieldName = entity.Name + "_" + fieldName;
            return node;
        }

        #endregion

        #region 访问二元表达式
        /// <inheritdoc />
        /// <summary>
        /// 访问二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sqlCmd.Append("(");
            Visit(node.Left);

            _sqlCmd.Append(node.GetExpressionType());

            Visit(node.Right);
            _sqlCmd.Append(")");

            return node;
        }
        #endregion

        #region 访问常量表达式
        /// <inheritdoc />
        /// <summary>
        /// 访问常量表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.ToConvertAndGetValue();
            if (value != null)
            {
                _sqlCmd.Append(ParamName);
                Param.Add(TempFieldName, value);
            }
            else
            {
                _sqlCmd.Append("NULL");
            }
            return node;
        }
        #endregion

        #region 访问方法表达式
        /// <inheritdoc />
        /// <summary>
        /// 访问方法表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Operation(node);
            return node;
        }

        #endregion

        private void Operation(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    {
                        Visit(node.Object);
                        _sqlCmd.Append($" LIKE {ParamName}");
                        var argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(TempFieldName, "%" + argumentExpression.ToConvertAndGetValue() + "%");
                    }
                    break;
                case "Equals":
                    {
                        Visit(node.Object);
                        _sqlCmd.Append($" = {ParamName}");
                        var argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(TempFieldName, argumentExpression.ToConvertAndGetValue());
                    }
                    break;
                case "In":
                    {
                        Visit(node.Arguments[0]);
                        _sqlCmd.Append($" IN {ParamName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(TempFieldName, value);
                    }
                    break;
                case "NotIn":
                    {
                        Visit(node.Arguments[0]);
                        _sqlCmd.Append($" NOT IN {ParamName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(TempFieldName, value);
                    }
                    break;
                case "IsNull":
                    {
                        Visit(node.Arguments[0]);
                        _sqlCmd.Append(" IS NULL");
                    }
                    break;
                case "IsNotNull":
                    {
                        Visit(node.Arguments[0]);
                        _sqlCmd.Append(" IS NOT NULL");
                    }
                    break;
                case "Between":
                    {
                        Visit(node.Arguments[0]);
                        string fromParam = ParamName + "_From";
                        string toParam = ParamName + "_To";
                        _sqlCmd.Append($" BETWEEN {fromParam} AND {toParam}");
                        Param.Add(fromParam, node.Arguments[1].ToConvertAndGetValue());
                        Param.Add(toParam, node.Arguments[2].ToConvertAndGetValue());
                    }
                    break;
                #region Convert转换计算
                case "ToInt32":
                case "ToString":
                case "ToDecimal":
                case "ToDouble":
                    {
                        Visit(node.Object);
                        if (node.Arguments.Count > 0)//Convert.ToInt32("xxx") or Convert.ToString("xxx")
                        {
                            _sqlCmd.AppendFormat(" {0}", ParamName);
                            var argumentExpression = (ConstantExpression)node.Arguments[0];
                            Param.Add(TempFieldName, argumentExpression.ToConvertAndGetValue());
                        }
                        else
                        {
                            //xxx.ToString
                        }
                    }
                    break;
                #endregion
                #region 时间计算
                case "AddYears":
                case "AddMonths":
                case "AddDays":
                case "AddHours":
                case "AddMinutes":
                case "AddSeconds":
                    {
                        if (!(node.Object is MemberExpression))//数值比对（DateTime.Now.AddDays）
                        {
                            object time = node.ToConvertAndGetValue();
                            _sqlCmd.AppendFormat(" {0}", ParamName);
                            Param.Add(TempFieldName, time);
                        }
                        else//字段比对(类似CreateDate.AddDays)
                        {
                            var member = (MemberExpression)(node.Object);
                            var entity = EntityCache.QueryEntity(member.Member.DeclaringType);
                            var dateOption = (DateOption)Enum.Parse(typeof(DateOption), node.Method.Name);
                            string fieldName = entity.FieldPairs[member.Member.Name];
                            string sqlCombine = providerOption.CombineDate(dateOption, entity.Name, fieldName, node.Arguments[0].ToConvertAndGetValue().ToString());
                            _sqlCmd.Append(sqlCombine);
                            //重新计算参数名称
                            TempFieldName = entity.Name + "_" + fieldName;
                        }
                    }
                    break;
                #endregion
                default:
                    throw new DapperExtensionException("the expression is no support this function");
            }
        }
    }
}
