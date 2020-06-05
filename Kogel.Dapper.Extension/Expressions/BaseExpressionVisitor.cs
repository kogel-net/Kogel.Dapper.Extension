using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kogel.Dapper.Extension.Expressions
{
    /// <summary>
    /// 实现表达式解析的基类
    /// </summary>
    public class BaseExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 字段sql
        /// </summary>
        internal StringBuilder SpliceField { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        protected DynamicParameters Param { get; set; }
        /// <summary>
        /// 解析提供方
        /// </summary>
        protected SqlProvider Provider { get; set; }
        /// <summary>
        /// 解析第n个下标
        /// </summary>
        protected int Index { get; set; }
        /// <summary>
        /// 提供方选项
        /// </summary>
        protected IProviderOption providerOption;

        public BaseExpressionVisitor(SqlProvider provider)
        {
            SpliceField = new StringBuilder();
            this.Param = new DynamicParameters();
            this.Provider = provider;
            this.providerOption = provider.ProviderOption;
        }
        /// <summary>
        /// 有+ - * /需要拼接的对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binary = new BinaryExpressionVisitor(node, Provider, Index);
            SpliceField.Append(binary.SpliceField);
            this.Param.AddDynamicParams(binary.Param);
            return node;
        }
        /// <summary>
        /// 值对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            //参数
            string paramName = $"{providerOption.ParameterPrefix}Member_Param_{Index}_{Param.ParameterNames.Count()}";
            //值
            object nodeValue = node.ToConvertAndGetValue();
            //设置sql
            SpliceField.Append(paramName);
            Param.Add(paramName, nodeValue);
            return node;
        }
        /// <summary>
        /// 成员对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            //需要计算的字段值
            var expTypeName = node.Expression?.GetType().FullName ?? "";
            if (expTypeName == "System.Linq.Expressions.TypedParameterExpression" || expTypeName == "System.Linq.Expressions.PropertyExpression")
            {
                var member = EntityCache.QueryEntity(node.Expression.Type);
                string fieldName = member.FieldPairs[node.Member.Name];
                string field = (providerOption.IsAsName ? member.GetAsName(providerOption) : "") + providerOption.CombineFieldName(fieldName);
                SpliceField.Append(field);
            }
            else
            {
                //参数
                string paramName = $"{providerOption.ParameterPrefix}Member_Param_{Index}_{Param.ParameterNames.Count()}";
                //值
                object nodeValue = node.ToConvertAndGetValue();
                //设置sql
                SpliceField.Append(paramName);
                Param.Add(paramName, nodeValue);
            }
            return node;
        }

        /// <summary>
        /// 待执行的方法对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension.Function"))//系统函数
            {
                Operation(node);
            }
            else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
            {
                DynamicParameters parameters = new DynamicParameters();
                SpliceField.Append("(" + node.MethodCallExpressionToSql(ref parameters) + ")");
                Param.AddDynamicParams(parameters);
            }
            else
            {
                Operation(node);
            }
            return node;
        }

        /// <summary>
        /// 解析函数
        /// </summary>
        /// <param name="node"></param>
        private void Operation(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                #region Convert转换计算
                case "ToInt32":
                case "ToString":
                case "ToDecimal":
                case "ToDouble":
                case "ToBoolean":
                case "ToDateTime":
                    {
                        var convertOption = (ConvertOption)Enum.Parse(typeof(ConvertOption), node.Method.Name);
                        providerOption.CombineConvert(convertOption, SpliceField, () =>
                        {
                            Visit(node.Object != null ? node.Object : node.Arguments[0]);
                        });
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
                        var dateOption = (DateOption)Enum.Parse(typeof(DateOption), node.Method.Name);
                        providerOption.CombineDate(dateOption, SpliceField,
                            () =>
                            {
                                Visit(node.Object);
                            },
                            () =>
                            {
                                Visit(node.Arguments);
                            });
                    }
                    break;
                #endregion
                #region 字符处理
                case "ToLower":
                    {
                        providerOption.ToLower(SpliceField,
                            () =>
                            {
                                if (node.Object != null)
                                    Visit(node.Object);
                                else
                                    Visit(node.Arguments);
                            });
                    }
                    break;
                case "ToUpper":
                    {
                        providerOption.ToUpper(SpliceField,
                            () =>
                            {
                                if (node.Object != null)
                                    Visit(node.Object);
                                else
                                    Visit(node.Arguments);
                            });
                    }
                    break;
                case "Replace":
                    {

                        SpliceField.Append("Replace(");
                        Visit(node.Object);
                        SpliceField.Append(",");
                        Visit(node.Arguments[0]);
                        SpliceField.Append(",");
                        Visit(node.Arguments[1]);
                        SpliceField.Append(")");
                    }
                    break;
                case "Trim":
                    {
                        SpliceField.Append("Trim(");
                        Visit(node.Object);
                        SpliceField.Append(")");
                    }
                    break;
                case "Concact":
                    {
                        SpliceField.Append("Concat(");
                        Visit(node.Arguments[0]);
                        SpliceField.Append(",");
                        Visit(node.Arguments[1]);
                        SpliceField.Append(")");
                    }
                    break;
                case "IfNull":
                    {
                        SpliceField.Append($"{providerOption.IfNull()}(");
                        Visit(node.Arguments[0]);
                        SpliceField.Append(",");
                        Visit(node.Arguments[1]);
                        SpliceField.Append(")");
                    }
                    break;
                #endregion
                #region 聚合函数
                case "Count":
                    {
                        providerOption.Count(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Sum":
                    {
                        providerOption.Sum(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Max":
                    {
                        providerOption.Max(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Min":
                    {
                        providerOption.Min(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Avg":
                    {
                        providerOption.Avg(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                #endregion
                #region 导航属性
                case "Select":
                    {
                        Visit(node.Arguments[1]);
                        break;
                    }
                #endregion
                default:
                    SpliceField.Append(node.ToConvertAndGetValue());
                    break;
            }
        }
    }

    /// <summary>
    /// 用于解析条件表达式
    /// </summary>
    public class WhereExpressionVisitor : BaseExpressionVisitor
    {
        private string FieldName { get; set; } = "";//字段
        private string ParamName { get => GetParamName(); }//带参数标识的
        internal new StringBuilder SpliceField { get; set; }
        internal new DynamicParameters Param { get; set; }
        public WhereExpressionVisitor(SqlProvider provider) : base(provider)
        {
            this.SpliceField = new StringBuilder();
            this.Param = new DynamicParameters();
        }

        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <returns></returns>
        private string GetParamName()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(providerOption.ParameterPrefix);
            if (!string.IsNullOrEmpty(FieldName))
                //builder.Append(FieldName.Replace(".", "_"));
                //builder.Append(FieldName.Substring(FieldName.IndexOf(".") + 1));
                builder.Append("Param");
            builder.Append($"_{Param.ParameterNames.Count()}{Index}");
            return builder.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            string callName = node.Method.DeclaringType.FullName;
            //使用convert函数里待执行的sql数据
            if (callName.Equals("Kogel.Dapper.Extension.ExpressExpansion"))//自定义扩展方法
            {
                Operation(node);
            }
            else if (callName.Contains("Kogel.Dapper.Extension.Function"))//系统函数
            {
                Operation(node);
            }
            else if (callName.Contains("Kogel.Dapper.Extension"))
            {
                base.VisitMethodCall(node);
                this.SpliceField.Append(base.SpliceField);
                this.Param.AddDynamicParams(base.Param);
            }
            else
            {
                Operation(node);
            }
            return node;
        }
        /// <summary>
        /// 处理判断字符
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                SpliceField.Append("NOT(");
                Visit(node.Operand);
                SpliceField.Append(")");
            }
            else
            {
                Visit(node.Operand);
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
            var expType = node.Expression?.GetType();
            var expTypeName = expType?.FullName ?? "";
            if (expTypeName == "System.Linq.Expressions.TypedParameterExpression" || expTypeName == "System.Linq.Expressions.PropertyExpression")
            {
                //验证是否是可空对象
                if (!node.Expression.Type.FullName.Contains("System.Nullable")) //(node.Expression.Type != typeof(Nullable))
                {
                    var member = EntityCache.QueryEntity(node.Expression.Type);
                    string asName = string.Empty;
                    if (providerOption.IsAsName)
                    {
                        this.FieldName = $"{member.AsName}.{member.FieldPairs[node.Member.Name]}";
                        asName = member.GetAsName(providerOption);
                    }
                    else
                        this.FieldName = member.FieldPairs[node.Member.Name];
                    SpliceField.Append($"{asName}{providerOption.CombineFieldName(member.FieldPairs[node.Member.Name])}");
                    //导航属性允许显示字段
                    if (expTypeName == "System.Linq.Expressions.PropertyExpression")
                    {
                        //导航属性有条件时设置查询该导航属性
                        var joinTable = Provider.JoinList.Find(x => x.TableType.IsTypeEquals(member.Type));
                        if (joinTable != null && joinTable.IsMapperField == false)
                        {
                            joinTable.IsMapperField = true;
                        }
                        else
                        {
                            //不存在第一层中，可能在后几层嵌套使用导航属性
                            //获取调用者表达式
                            var parentExpression = (node.Expression as MemberExpression).Expression;
                            var parentEntity = EntityCache.QueryEntity(parentExpression.Type);
                            joinTable = parentEntity.Navigations.Find(x => x.TableType == member.Type);
                            if (joinTable != null)
                            {
                                joinTable = (JoinAssTable)joinTable.Clone();
                                joinTable.IsMapperField = true;
                                //加入导航连表到提供方
                                Provider.JoinList.Add(joinTable);
                            }
                        }
                    }
                }
                else
                {
                    //可空函数
                    Visit(node.Expression);
                    switch (node.Member.Name)
                    {
                        case "HasValue":
                            {
                                this.SpliceField.Append(" IS NOT NULL");
                            }
                            break;
                    }
                }
            }
            else
            {
                SpliceField.Append(ParamName);
                object nodeValue = node.ToConvertAndGetValue();
                Param.Add(ParamName, nodeValue);
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
            if (FieldName != null)
            {
                SpliceField.Append(ParamName);
                Param.Add(ParamName, node.ToConvertAndGetValue());
            }
            else
            {
                var nodeValue = node.ToConvertAndGetValue();
                switch (nodeValue)
                {
                    case true:
                        SpliceField.Append("1=1");
                        break;
                    case false:
                        SpliceField.Append("1!=1");
                        break;
                    default:
                        SpliceField.Append(ParamName);
                        Param.Add(ParamName, nodeValue);
                        break;
                }
            }
            return node;
        }
        /// <summary>
        /// 解析函数
        /// </summary>
        /// <param name="node"></param>
        private void Operation(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    {
                        if (node.Object != null && !(node.Object.Type.FullName.Contains("System.Collections.Generic")))
                        {
                            Visit(node.Object);
                            var value = node.Arguments[0].ToConvertAndGetValue();
                            string param = ParamName;
                            value = providerOption.FuzzyEscaping(value, ref param);
                            this.SpliceField.Append($" LIKE {param}");
                            Param.Add(ParamName, value);
                        }
                        else
                        {
                            if (node.Object != null)
                            {
                                Visit(node.Arguments[0]);
                                this.SpliceField.Append(" IN ");
                                Visit(node.Object);
                            }
                            else
                            {
                                Visit(node.Arguments[1]);
                                this.SpliceField.Append($" IN {ParamName}");
                                //这里只能手动记录参数
                                var nodeValue = node.Arguments[0].ToConvertAndGetValue();
                                Param.Add(ParamName, nodeValue);
                            }
                        }
                    }
                    break;
                case "StartsWith":
                    {
                        Visit(node.Object);
                        var value = node.Arguments[0].ToConvertAndGetValue();
                        string param = ParamName;
                        value = providerOption.FuzzyEscaping(value, ref param, EFuzzyLocation.Right);
                        this.SpliceField.Append($" LIKE {param}");
                        Param.Add(ParamName, value);
                    }
                    break;
                case "EndsWith":
                    {
                        Visit(node.Object);
                        var value = node.Arguments[0].ToConvertAndGetValue();
                        string param = ParamName;
                        value = providerOption.FuzzyEscaping(value, ref param, EFuzzyLocation.Left);
                        this.SpliceField.Append($" LIKE {param}");
                        Param.Add(ParamName, value);
                    }
                    break;
                case "Equals":
                    {
                        if (node.Object != null)
                        {
                            Visit(node.Object);
                            this.SpliceField.Append($" = ");
                            Visit(node.Arguments[0]);
                        }
                        else
                        {
                            Visit(node.Arguments[0]);
                            this.SpliceField.Append($" = ");
                            Visit(node.Arguments[1]);
                        }
                    }
                    break;
                case "In":
                    {
                        Visit(node.Arguments[0]);
                        this.SpliceField.Append($" IN {ParamName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(ParamName, value);
                    }
                    break;
                case "NotIn":
                    {
                        Visit(node.Arguments[0]);
                        this.SpliceField.Append($" NOT IN {ParamName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(ParamName, value);
                    }
                    break;
                case "IsNull":
                    {
                        Visit(node.Arguments[0]);
                        this.SpliceField.Append(" IS NULL");
                    }
                    break;
                case "IsNotNull":
                    {
                        Visit(node.Arguments[0]);
                        this.SpliceField.Append(" IS NOT NULL");
                    }
                    break;
                case "IsNullOrEmpty":
                    {
                        this.SpliceField.Append("(");
                        Visit(node.Arguments[0]);
                        this.SpliceField.Append(" IS NULL AND ");
                        Visit(node.Arguments[0]);
                        this.SpliceField.Append(" !=''");
                        this.SpliceField.Append(")");
                    }
                    break;
                case "Between":
                    {
                        if (node.Object != null)
                        {
                            Visit(node.Object);
                            SpliceField.Append(" BETWEEN ");
                            Visit(node.Arguments[0]);
                            SpliceField.Append(" AND ");
                            Visit(node.Arguments[1]);
                        }
                        else
                        {
                            Visit(node.Arguments[0]);
                            SpliceField.Append(" BETWEEN ");
                            Visit(node.Arguments[1]);
                            SpliceField.Append(" AND ");
                            Visit(node.Arguments[2]);
                        }
                    }
                    break;
                case "Any":
                    {
                        Type entityType;
                        if (ExpressionExtension.IsAnyBaseEntity(node.Arguments[0].Type, out entityType))
                        {
                            //导航属性有条件时设置查询该导航属性
                            var navigationTable = Provider.JoinList.Find(x => x.TableType.IsTypeEquals(entityType));
                            if (navigationTable != null)
                                navigationTable.IsMapperField = true;
                            else
                            {
                                //不存在第一层中，可能在后几层嵌套使用导航属性
                                //获取调用者表达式
                                var parentExpression = (node.Arguments[0] as MemberExpression).Expression;
                                var parentEntity = EntityCache.QueryEntity(parentExpression.Type);
                                navigationTable = parentEntity.Navigations.Find(x => x.TableType == entityType);
                                if (navigationTable != null)
                                {
                                    navigationTable = (JoinAssTable)navigationTable.Clone();
                                    navigationTable.IsMapperField = true;
                                    //加入导航连表到提供方
                                    Provider.JoinList.Add(navigationTable);
                                }
                            }
                            //解析导航属性条件
                            var navigationExpression = new WhereExpression(node.Arguments[1] as LambdaExpression, $"_Navi_{navigationTable.PropertyInfo.Name}", Provider);
                            //添加sql和参数
                            this.SpliceField.Append($" 1=1 {navigationExpression.SqlCmd}");
                            this.Param.AddDynamicParams(navigationExpression.Param);
                        }
                        else
                        {
                            throw new DapperExtensionException("导航属性类需要继承IBaseEntity");
                        }
                    }
                    break;
                #region Convert转换计算
                case "ToInt32":
                case "ToString":
                case "ToDecimal":
                case "ToDouble":
                case "ToBoolean":
                case "ToDateTime":
                    {
                        var convertOption = (ConvertOption)Enum.Parse(typeof(ConvertOption), node.Method.Name);
                        providerOption.CombineConvert(convertOption, SpliceField, () =>
                        {
                            Visit(node.Object != null ? node.Object : node.Arguments[0]);
                        });
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
                        var dateOption = (DateOption)Enum.Parse(typeof(DateOption), node.Method.Name);
                        providerOption.CombineDate(dateOption, SpliceField,
                            () =>
                            {
                                Visit(node.Object);
                            },
                            () =>
                            {
                                Visit(node.Arguments);
                            });
                    }
                    break;
                #endregion
                #region 字符处理
                case "ToLower":
                    {
                        providerOption.ToLower(SpliceField,
                            () =>
                            {
                                if (node.Object != null)
                                    Visit(node.Object);
                                else
                                    Visit(node.Arguments);
                            });
                    }
                    break;
                case "ToUpper":
                    {
                        providerOption.ToUpper(SpliceField,
                            () =>
                            {
                                if (node.Object != null)
                                    Visit(node.Object);
                                else
                                    Visit(node.Arguments);
                            });
                    }
                    break;
                case "Replace":
                    {

                        SpliceField.Append("Replace(");
                        Visit(node.Object);
                        SpliceField.Append(",");
                        Visit(node.Arguments[0]);
                        SpliceField.Append(",");
                        Visit(node.Arguments[1]);
                        SpliceField.Append(")");
                    }
                    break;
                case "Trim":
                    {
                        SpliceField.Append("Trim(");
                        Visit(node.Object);
                        SpliceField.Append(")");
                    }
                    break;
                case "Concact":
                    {
                        SpliceField.Append("Concat(");
                        Visit(node.Arguments[0]);
                        SpliceField.Append(",");
                        Visit(node.Arguments[1]);
                        SpliceField.Append(")");
                    }
                    break;
                case "IfNull":
                    {
                        SpliceField.Append($"{providerOption.IfNull()}(");
                        Visit(node.Arguments[0]);
                        SpliceField.Append(",");
                        Visit(node.Arguments[1]);
                        SpliceField.Append(")");
                    }
                    break;
                #endregion
                #region 聚合函数
                case "Count":
                    {
                        providerOption.Count(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Sum":
                    {
                        providerOption.Sum(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Max":
                    {
                        providerOption.Max(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Min":
                    {
                        providerOption.Min(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Avg":
                    {
                        providerOption.Avg(SpliceField, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                #endregion
                #region lambda函数
                case "FirstOrDefault":
                    {
                        string paramName = ParamName;
                        this.SpliceField.Append(paramName);
                        Param.Add(paramName, node.ToConvertAndGetValue());
                    }
                    break;
                #endregion
                default:
                    {
                        if (node.Object != null)
                            Visit(node.Object);
                        else
                            Visit(node.Arguments);
                    }
                    break;
            }
        }
    }
    /// <summary>
    /// 用于解析二元表达式
    /// </summary>
    public class BinaryExpressionVisitor : WhereExpressionVisitor
    {
        public BinaryExpressionVisitor(BinaryExpression expression, SqlProvider provider, int index = 0) : base(provider)
        {
            SpliceField = new StringBuilder();
            Param = new DynamicParameters();
            base.Index = index;
            SpliceField.Append("(");
            Visit(expression);
            SpliceField.Append(")");
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            SpliceField.Append("(");
            Visit(node.Left);
            var expressionType = node.GetExpressionType();
            SpliceField.Append(expressionType);
            if (expressionType == " AND " || expressionType == " OR ")
            {
                switch (node.Right.ToString())
                {
                    case "True":
                        SpliceField.Append("1=1");
                        break;
                    case "False":
                        SpliceField.Append("1!=1");
                        break;
                    default:
                        Visit(node.Right);
                        break;
                }
            }
            else
            {
                Visit(node.Right);
            }
            SpliceField.Append(")");
            return node;
        }
    }
}
