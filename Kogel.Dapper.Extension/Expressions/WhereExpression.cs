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
    /// <summary>
    /// 解析查询条件
    /// </summary>
    public sealed class WhereExpression :BaseExpressionVisitor
    {
        #region sql指令
        private readonly StringBuilder _sqlCmd;
        /// <summary>
        /// sql指令
        /// </summary>
        public string SqlCmd => _sqlCmd.Length > 0 ? $" AND {_sqlCmd} " : "";
        /// <summary>
        /// 参数
        /// </summary>
        public new DynamicParameters Param { get; }
        private IProviderOption providerOption;
        #endregion
        #region 当前解析的对象
        private EntityObject entity { get; }
        #endregion
        public WhereExpression(LambdaExpression expression, string prefix, IProviderOption providerOption)
        {
            this._sqlCmd = new StringBuilder(100);
            this.Param = new DynamicParameters();
            this.providerOption = providerOption;
            //当前定义的查询返回对象
            this.entity = EntityCache.QueryEntity(expression.Body.Type);
            //开始解析对象
            Visit(expression);
            //开始拼接成条件
            for (var i = 0; i < base.FieldList.Count; i++)
            {
                if (_sqlCmd.Length != 0)
                    _sqlCmd.Append(" AND ");
                _sqlCmd.Append(base.FieldList[i]);
            }
            this.Param.AddDynamicParams(base.Param);
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binaryWhere = new BinaryWhereExpressionVisitor(node, providerOption);
            this._sqlCmd.Append(binaryWhere.SpliceField);
            this.Param.AddDynamicParams(binaryWhere.Param);
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }
    }
    
    /// <summary>
    /// 专门处理条件的二元表达式
    /// </summary>
    internal class BinaryWhereExpressionVisitor : BinaryExpressionVisitor
    {
        private string FieldName { get; set; }//字段
        private string ParamName { get => (providerOption.ParameterPrefix + FieldName.Replace(".", "_") + "_" + Param.ParameterNames.Count()); }//带参数标识的
        public BinaryWhereExpressionVisitor(BinaryExpression expression, IProviderOption providerOption) : base(expression, providerOption)
        {
            
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //使用convert函数里待执行的sql数据
            if (node.Method.DeclaringType.FullName.Contains("Convert"))
            {
                Visit(node.Arguments[0]);
            }
            else if(node.Method.DeclaringType.FullName.Equals("Kogel.Dapper.Extension.ExpressExpansion"))//自定义扩展方法
            {
                Visit(node.Arguments[0]);
                Operation(node);
            }
            else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
            {
                base.VisitMethodCall(node);
            }
            else
            {
                Operation(node);
            }
            return node;
        }
        /// <summary>
        /// 重写成员对象，得到字段名称
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            var member = EntityCache.QueryEntity(node.Member.DeclaringType);
            string fieldName = member.FieldPairs[node.Member.Name];
            this.FieldName = member.Name + "." + fieldName;
            SpliceField.Append(this.FieldName);

            //时间类型有时会进入此处
            if (node.Type == typeof(DateTime))
            {
                SpliceField= SpliceField.Replace("DateTime.Now", base.providerOption.GetDate());
            }
            return node;
        }
        /// <summary>
        /// 重写值对象，记录参数
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            SpliceField.Append(ParamName);
            Param.Add(ParamName, node.ToConvertAndGetValue());
            return node;
        }

        private void Operation(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    {
                        Visit(node.Object);
                        base.SpliceField.Append($" LIKE {ParamName}");
                        var argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(ParamName, "%" + argumentExpression.ToConvertAndGetValue() + "%");
                    }
                    break;
                case "Equals":
                    {
                        Visit(node.Object);
                        base.SpliceField.Append($" = {ParamName}");
                        var argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(ParamName, argumentExpression.ToConvertAndGetValue());
                    }
                    break;
                case "In":
                    {
                        base.SpliceField.Append($" IN {ParamName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(ParamName, value);
                    }
                    break;
                case "NotIn":
                    {
                        base.SpliceField.Append($" NOT IN {ParamName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(ParamName, value);
                    }
                    break;
                case "IsNull":
                    {
                        base.SpliceField.Append(" IS NULL");
                    }
                    break;
                case "IsNotNull":
                    {
                        base.SpliceField.Append(" IS NOT NULL");
                    }
                    break;
                case "Between":
                    {
                        string fromParam = ParamName + "_From";
                        string toParam = ParamName + "_To";
                        base.SpliceField.Append($" BETWEEN {fromParam} AND {toParam}");
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
                            base.SpliceField.AppendFormat(" {0}", ParamName);
                            var argumentExpression = (ConstantExpression)node.Arguments[0];
                            Param.Add(ParamName, argumentExpression.ToConvertAndGetValue());
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
                            base.SpliceField.AppendFormat(" {0}", ParamName);
                            Param.Add(ParamName, time);
                        }
                        else//字段比对(类似CreateDate.AddDays)
                        {
                            var member = (MemberExpression)(node.Object);
                            var entity = EntityCache.QueryEntity(member.Member.DeclaringType);
                            var dateOption = (DateOption)Enum.Parse(typeof(DateOption), node.Method.Name);
                            string fieldName = entity.FieldPairs[member.Member.Name];
                            string sqlCombine = providerOption.CombineDate(dateOption, entity.Name, fieldName, node.Arguments[0].ToConvertAndGetValue().ToString());
                            base.SpliceField.Append(sqlCombine);
                            //重新计算参数名称
                            FieldName = entity.Name + "_" + fieldName;
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
