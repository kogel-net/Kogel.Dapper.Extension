using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Entites;
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
        internal StringBuilder SqlBuilder { get; set; }

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
        protected int ExpIndex { get; set; }

        /// <summary>
        /// 提供方选项
        /// </summary>
        protected IProviderOption ProviderOption;

        public BaseExpressionVisitor(SqlProvider provider)
        {
            SqlBuilder = new StringBuilder();
            this.Param = new DynamicParameters();
            this.Provider = provider;
            this.ProviderOption = provider.ProviderOption;
        }

        /// <summary>
        /// 有+ - * /需要拼接的对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binary = new BinaryExpressionVisitor(node, Provider, ExpIndex);
            SqlBuilder.Append(binary._sqlBuilder);
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
            string paramName = $"{ProviderOption.ParameterPrefix}Member_Param_{ExpIndex}_{Param.ParameterNames.Count()}";
            //值
            object nodeValue = node.ToConvertAndGetValue();
            //设置sql
            SqlBuilder.Append(paramName);
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
                //验证是否是可空对象
                if (!node.Expression.Type.FullName.Contains("System.Nullable")) //(node.Expression.Type != typeof(Nullable))
                {
                    //是否是成员值对象
                    if (expTypeName == "System.Linq.Expressions.PropertyExpression" && node.IsConstantExpression())
                    {
                        //参数
                        string paramName = $"{ProviderOption.ParameterPrefix}Member_Param_{ExpIndex}_{Param.ParameterNames.Count()}";
                        //值
                        object nodeValue = node.ToConvertAndGetValue();
                        //设置sql
                        SqlBuilder.Append(paramName);
                        Param.Add(paramName, nodeValue);
                        return node;
                    }
                    var member = EntityCache.QueryEntity(node.Expression.Type);
                    string fieldName = member.FieldPairs[node.Member.Name];
                    //字段全称
                    string fieldStr = Provider.IsAppendAsName ? $"{member.AsName}.{ProviderOption.CombineFieldName(fieldName)}"
                        : ProviderOption.CombineFieldName(member.FieldPairs[node.Member.Name]);
                    SqlBuilder.Append(fieldStr);

                    //string field = $"{member.AsName}.{ProviderOption.CombineFieldName(fieldName)}";
                    //_sqlBuilder.Append(field);
                }
                else
                {
                    //可空函数
                    Visit(node.Expression);
                    switch (node.Member.Name)
                    {
                        case "HasValue":
                            {
                                this.SqlBuilder.Append(" IS NOT NULL");
                            }
                            break;
                    }
                }
            }
            else
            {
                //参数
                string paramName = $"{ProviderOption.ParameterPrefix}Member_Param_{ExpIndex}_{Param.ParameterNames.Count()}";
                //值
                object nodeValue = node.ToConvertAndGetValue();
                //设置sql
                SqlBuilder.Append(paramName);
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
                SqlBuilder.Append($"({node.MethodCallExpressionToSql(ref parameters, ExpIndex)})");
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
                        ProviderOption.CombineConvert(convertOption, SqlBuilder, () =>
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
                        ProviderOption.CombineDate(dateOption, SqlBuilder,
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
                        ProviderOption.ToLower(SqlBuilder,
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
                        ProviderOption.ToUpper(SqlBuilder,
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

                        SqlBuilder.Append("Replace(");
                        Visit(node.Object);
                        SqlBuilder.Append(",");
                        Visit(node.Arguments[0]);
                        SqlBuilder.Append(",");
                        Visit(node.Arguments[1]);
                        SqlBuilder.Append(")");
                    }
                    break;
                case "Trim":
                    {
                        SqlBuilder.Append("Trim(");
                        Visit(node.Object);
                        SqlBuilder.Append(")");
                    }
                    break;
                case "Concat":
                    {
                        SqlBuilder.Append("Concat(");
                        Visit(node.Arguments[0]);
                        SqlBuilder.Append(",");
                        Visit(node.Arguments[1]);
                        SqlBuilder.Append(")");
                    }
                    break;
                case "IfNull":
                    {
                        SqlBuilder.Append($"{ProviderOption.IfNull()}(");
                        Visit(node.Arguments[0]);
                        SqlBuilder.Append(",");
                        Visit(node.Arguments[1]);
                        SqlBuilder.Append(")");
                    }
                    break;
                case "ConcatSql":
                    {
                        SqlBuilder.Append(node.Arguments[0].ToConvertAndGetValue());
                        // Param
                        if (node.Arguments.Count > 1)
                        {
                            Param.AddDynamicParams(node.Arguments[1].ToConvertAndGetValue());
                        }
                    }
                    break;
                #endregion
                #region 聚合函数
                case "Count":
                    {
                        ProviderOption.Count(SqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Sum":
                    {
                        ProviderOption.Sum(SqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Max":
                    {
                        ProviderOption.Max(SqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Min":
                    {
                        ProviderOption.Min(SqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Avg":
                    {
                        ProviderOption.Avg(SqlBuilder, () =>
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
                    SqlBuilder.Append(node.ToConvertAndGetValue());
                    break;
            }
        }
    }

    /// <summary>
    /// 用于解析条件表达式
    /// </summary>
    public class WhereExpressionVisitor : BaseExpressionVisitor
    {
        /// <summary>
        /// 参数标记
        /// </summary>
        internal string Prefix { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        private string _fieldName { get; set; } = string.Empty;

        /// <summary>
        /// 带参数标识的参数名称
        /// </summary>
        private string _paramName => $"{GetParamName()}{Prefix}";

        /// <summary>
        /// 拼接sql
        /// </summary>
        internal new StringBuilder _sqlBuilder { get; set; }

        /// <summary>
        /// 参数目录
        /// </summary>
        internal new DynamicParameters Param { get; set; }


        public WhereExpressionVisitor(SqlProvider provider) : base(provider)
        {
            this._sqlBuilder = new StringBuilder();
            this.Param = new DynamicParameters();
        }

        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <returns></returns>
        private string GetParamName()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ProviderOption.ParameterPrefix);
            if (!string.IsNullOrEmpty(_fieldName))
                builder.Append("Param");
            builder.Append($"_{Param.ParameterNames.Count()}{ExpIndex}");
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
                this._sqlBuilder.Append(base.SqlBuilder);
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
                _sqlBuilder.Append("NOT(");
                Visit(node.Operand);
                _sqlBuilder.Append(")");
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
                    //是否是成员值对象
                    if (expTypeName == "System.Linq.Expressions.PropertyExpression" && node.IsConstantExpression())
                    {
                        _sqlBuilder.Append(_paramName);
                        object nodeValue = node.ToConvertAndGetValue();
                        Param.Add(_paramName, nodeValue);
                        return node;
                    }
                    var member = EntityCache.QueryEntity(node.Expression.Type);
                    this._fieldName = member.FieldPairs[node.Member.Name];
                    //字段全称
                    string fieldStr = Provider.IsAppendAsName ? $"{member.AsName}.{ProviderOption.CombineFieldName(member.FieldPairs[node.Member.Name])}"
                        : ProviderOption.CombineFieldName(member.FieldPairs[node.Member.Name]);
                    _sqlBuilder.Append(fieldStr);
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
                                this._sqlBuilder.Append(" IS NOT NULL");
                            }
                            break;
                    }
                }
            }
            else
            {
                _sqlBuilder.Append(_paramName);
                object nodeValue = node.ToConvertAndGetValue();
                Param.Add(_paramName, nodeValue);
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
            if (!string.IsNullOrEmpty(_fieldName))
            {
                _sqlBuilder.Append(_paramName);
                Param.Add(_paramName, node.ToConvertAndGetValue());
            }
            else
            {
                var nodeValue = node.ToConvertAndGetValue();
                switch (nodeValue)
                {
                    case true:
                        _sqlBuilder.Append("1=1");
                        break;
                    case false:
                        _sqlBuilder.Append("1!=1");
                        break;
                    default:
                        _sqlBuilder.Append(_paramName);
                        Param.Add(_paramName, nodeValue);
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
                            string param = _paramName;
                            value = ProviderOption.FuzzyEscaping(value, ref param);
                            this._sqlBuilder.Append($" LIKE {param}");
                            Param.Add(_paramName, value);
                        }
                        else
                        {
                            if (node.Object != null)
                            {
                                Visit(node.Arguments[0]);
                                this._sqlBuilder.Append(" IN ");
                                Visit(node.Object);
                            }
                            else
                            {
                                Visit(node.Arguments[1]);
                                this._sqlBuilder.Append($" IN {_paramName}");
                                //这里只能手动记录参数
                                var nodeValue = node.Arguments[0].ToConvertAndGetValue();
                                Param.Add(_paramName, nodeValue);
                            }
                        }
                    }
                    break;
                case "StartsWith":
                    {
                        Visit(node.Object);
                        var value = node.Arguments[0].ToConvertAndGetValue();
                        string param = _paramName;
                        value = ProviderOption.FuzzyEscaping(value, ref param, EFuzzyLocation.Right);
                        this._sqlBuilder.Append($" LIKE {param}");
                        Param.Add(_paramName, value);
                    }
                    break;
                case "EndsWith":
                    {
                        Visit(node.Object);
                        var value = node.Arguments[0].ToConvertAndGetValue();
                        string param = _paramName;
                        value = ProviderOption.FuzzyEscaping(value, ref param, EFuzzyLocation.Left);
                        this._sqlBuilder.Append($" LIKE {param}");
                        Param.Add(_paramName, value);
                    }
                    break;
                case "Equals":
                    {
                        if (node.Object != null)
                        {
                            Visit(node.Object);
                            this._sqlBuilder.Append($" = ");
                            Visit(node.Arguments[0]);
                        }
                        else
                        {
                            Visit(node.Arguments[0]);
                            this._sqlBuilder.Append($" = ");
                            Visit(node.Arguments[1]);
                        }
                    }
                    break;
                case "In":
                    {
                        Visit(node.Arguments[0]);
                        this._sqlBuilder.Append($" IN {_paramName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(_paramName, value);
                    }
                    break;
                case "NotIn":
                    {
                        Visit(node.Arguments[0]);
                        this._sqlBuilder.Append($" NOT IN {_paramName}");
                        object value = node.Arguments[1].ToConvertAndGetValue();
                        Param.Add(_paramName, value);
                    }
                    break;
                case "IsNull":
                    {
                        Visit(node.Arguments[0]);
                        this._sqlBuilder.Append(" IS NULL");
                    }
                    break;
                case "IsNotNull":
                    {
                        Visit(node.Arguments[0]);
                        this._sqlBuilder.Append(" IS NOT NULL");
                    }
                    break;
                case "IsNullOrEmpty":
                    {
                        this._sqlBuilder.Append("(");
                        Visit(node.Arguments[0]);
                        this._sqlBuilder.Append(" IS NULL OR ");
                        Visit(node.Arguments[0]);
                        this._sqlBuilder.Append(" =''");
                        this._sqlBuilder.Append(")");
                    }
                    break;
                case "Between":
                    {
                        if (node.Object != null)
                        {
                            Visit(node.Object);
                            _sqlBuilder.Append(" BETWEEN ");
                            Visit(node.Arguments[0]);
                            _sqlBuilder.Append(" AND ");
                            Visit(node.Arguments[1]);
                        }
                        else
                        {
                            Visit(node.Arguments[0]);
                            _sqlBuilder.Append(" BETWEEN ");
                            Visit(node.Arguments[1]);
                            _sqlBuilder.Append(" AND ");
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
                            this._sqlBuilder.Append($" 1=1 {navigationExpression.SqlCmd}");
                            foreach (var paramName in navigationExpression.Param.ParameterNames)
                            {
                                //相同的key会直接顶掉
                                this.Param.Add(paramName, navigationExpression.Param.Get<object>(paramName));
                            }
                            //this.Param.AddDynamicParams(navigationExpression.Param);
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
                        ProviderOption.CombineConvert(convertOption, _sqlBuilder, () =>
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
                        ProviderOption.CombineDate(dateOption, _sqlBuilder,
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
                        ProviderOption.ToLower(_sqlBuilder,
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
                        ProviderOption.ToUpper(_sqlBuilder,
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

                        _sqlBuilder.Append("Replace(");
                        Visit(node.Object);
                        _sqlBuilder.Append(",");
                        Visit(node.Arguments[0]);
                        _sqlBuilder.Append(",");
                        Visit(node.Arguments[1]);
                        _sqlBuilder.Append(")");
                    }
                    break;
                case "Trim":
                    {
                        _sqlBuilder.Append("Trim(");
                        Visit(node.Object);
                        _sqlBuilder.Append(")");
                    }
                    break;
                case "Concat":
                    {
                        _sqlBuilder.Append("Concat(");
                        Visit(node.Arguments[0]);
                        _sqlBuilder.Append(",");
                        Visit(node.Arguments[1]);
                        _sqlBuilder.Append(")");
                    }
                    break;
                case "IfNull":
                    {
                        _sqlBuilder.Append($"{ProviderOption.IfNull()}(");
                        Visit(node.Arguments[0]);
                        _sqlBuilder.Append(",");
                        Visit(node.Arguments[1]);
                        _sqlBuilder.Append(")");
                    }
                    break;
                case "ConcatSql":
                    {
                        _sqlBuilder.Append(node.Arguments[0].ToConvertAndGetValue());
                        // Param
                        if (node.Arguments.Count > 1)
                        {
                            Param.AddDynamicParams(node.Arguments[1].ToConvertAndGetValue());
                        }
                    }
                    break;
                #endregion
                #region 聚合函数
                case "Count":
                    {
                        ProviderOption.Count(_sqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Sum":
                    {
                        ProviderOption.Sum(_sqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Max":
                    {
                        ProviderOption.Max(_sqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Min":
                    {
                        ProviderOption.Min(_sqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                case "Avg":
                    {
                        ProviderOption.Avg(_sqlBuilder, () =>
                        {
                            Visit(node.Arguments);
                        });
                    }
                    break;
                #endregion
                #region lambda函数
                case "FirstOrDefault":
                    {
                        string paramName = _paramName;
                        this._sqlBuilder.Append(paramName);
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
        public BinaryExpressionVisitor(BinaryExpression expression, SqlProvider provider, int index = 0, string prefix = null) : base(provider)
        {
            _sqlBuilder = new StringBuilder();
            Param = new DynamicParameters();
            base.ExpIndex = index;
            base.Prefix = prefix;
            _sqlBuilder.Append("(");
            Visit(expression);
            _sqlBuilder.Append(")");
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sqlBuilder.Append("(");
            Visit(node.Left);
            var expressionType = node.GetExpressionType();
            _sqlBuilder.Append(expressionType);
            if (expressionType == " AND " || expressionType == " OR ")
            {
                switch (node.Right.ToString())
                {
                    case "True":
                        _sqlBuilder.Append("1=1");
                        break;
                    case "False":
                        _sqlBuilder.Append("1!=1");
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
            _sqlBuilder.Append(")");
            return node;
        }
    }
}
