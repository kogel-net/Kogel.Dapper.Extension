using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.MySql
{
    internal static class ResolveExpression
    {
        public static IProviderOption ProviderOption;

        public static void InitOption(IProviderOption providerOption)
        {
            ProviderOption = providerOption;
        }

        public static string ResolveOrderBy(Dictionary<LambdaExpression, EOrderBy> orderbyExpressionDic)
        {
            var orderByList = orderbyExpressionDic.Select(a =>
            {
                var entity = EntityCache.QueryEntity(a.Key.Type.GenericTypeArguments[0]);
                var columnName = a.Key.Body.GetCorrectPropertyName();
                return $"{ProviderOption.CombineFieldName(entity.Name)}." + ProviderOption.CombineFieldName(columnName) + (a.Value == EOrderBy.Asc ? " ASC " : " DESC ");
            });
            if (!orderByList.Any())
                return "";

            return "ORDER BY " + string.Join(",", orderByList);
        }
        /// <summary>
        /// 条件查询集合
        /// </summary>
        /// <param name="lambdaExpressionList"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static List<WhereExpression> ResolveWhereList(AbstractSet abstractSet, ref string whereSql, DynamicParameters Params, string prefix = null)
        {
            //添加Linq生成的sql条件和参数
            List<LambdaExpression> lambdaExpressionList = abstractSet.WhereExpressionList;
            StringBuilder builder = new StringBuilder("WHERE 1=1 ");
            List<WhereExpression> whereExpressionList = new List<WhereExpression>();
            for (int i = 0; i < lambdaExpressionList.Count; i++)
            {
                var whereParam = new WhereExpression(lambdaExpressionList[i], prefix, ProviderOption);
                whereExpressionList.Add(whereParam);
                builder.Append(whereParam.SqlCmd);
                //参数
                foreach (var paramKey in whereParam.Param.ParameterNames)
                {
                    Params.Add(paramKey, whereParam.Param.Get<object>(paramKey));
                }
            }
            //添加自定义sql生成的条件和参数
            if ((abstractSet.WhereBuilder != null && abstractSet.WhereBuilder.Length != 0) || (abstractSet.Params != null && abstractSet.Params.ParameterNames.Count() != 0))
            {
                //添加自定义条件sql
                builder.Append(abstractSet.WhereBuilder);
                Params.AddDynamicParams(abstractSet.Params);
            }
            whereSql = builder.ToString();
            return whereExpressionList;
        }
        public static UpdateEntityWhereExpression ResolveWhere(object obj)
        {
            var where = new UpdateEntityWhereExpression(obj, ProviderOption);
            where.Resolve();
            return where;
        }
        /// <summary>
        /// 根据反射对象获取表字段
        /// </summary>
        /// <param name="propertyInfos"></param>
        /// <returns></returns>
        public static string GetTableField(EntityObject entityObject)
        {
            var propertyInfos = entityObject.Properties;
            string tableName = entityObject.Name;
            string property = string.Join(",", entityObject.FieldPairs.Select(field => $"{ProviderOption.CombineFieldName(tableName)}.{ProviderOption.CombineFieldName(field.Value) }"));
            return property;
        }
        public static string ResolveSelect(EntityObject entityObject, LambdaExpression selector)
        {
            var selectFormat = " SELECT {0} ";
            var selectSql = "";

            if (selector == null)
            {
                var propertyBuilder = GetTableField(entityObject);
                selectSql = string.Format(selectFormat, propertyBuilder);
            }
            else
            {
                var selectExp = new SelectExpression(selector, "", ProviderOption);
                selectSql = string.Format(selectFormat, selectExp.SqlCmd);
            }

            return selectSql;
        }

        public static string ResolveSelectOfUpdate(EntityObject entityObject, LambdaExpression selector)
        {
            var selectSql = "";

            if (selector == null)
            {
                var propertyBuilder = new StringBuilder();
                foreach (var propertyInfo in entityObject.Properties)
                {
                    if (propertyBuilder.Length > 0)
                        propertyBuilder.Append(",");
                    propertyBuilder.AppendFormat($"INSERTED.{ ProviderOption.CombineFieldName(propertyInfo.GetColumnAttributeName())} { ProviderOption.CombineFieldName(propertyInfo.Name)}");
                }
                selectSql = propertyBuilder.ToString();
            }
            else
            {
                var nodeType = selector.Body.NodeType;
                if (nodeType == ExpressionType.MemberAccess)
                {
                    var columnName = ((MemberExpression)selector.Body).Member.GetColumnAttributeName();
                    selectSql = "INSERTED." + ProviderOption.CombineFieldName(columnName);
                }
                else if (nodeType == ExpressionType.MemberInit)
                {
                    var memberInitExpression = (MemberInitExpression)selector.Body;
                    selectSql = string.Join(",", memberInitExpression.Bindings.Select(a => "INSERTED." + ProviderOption.CombineFieldName(a.Member.GetColumnAttributeName())));
                }
            }

            return "OUTPUT " + selectSql;
        }

        public static string ResolveSum<T>(Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentException("selector");
            var selectSql = "";

            switch (selector.NodeType)
            {
                case ExpressionType.Lambda:
                case ExpressionType.MemberAccess:
                    {
                        var memberName = selector.GetCorrectPropertyName();
                        EntityObject entityObject = EntityCache.QueryEntity(typeof(T));
                        selectSql = $" SELECT ISNULL(SUM({entityObject.Name}.{ProviderOption.CombineFieldName(entityObject.FieldPairs[memberName])}),0)  ";
                    }
                    break;
                case ExpressionType.MemberInit:
                    throw new DapperExtensionException("不支持该表达式类型");
            }

            return selectSql;
        }

        public static UpdateExpression ResolveUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            return new UpdateExpression(updateExpression, ProviderOption);
        }

        public static string ResolveWithNoLock(bool nolock)
        {
            //return nolock ? "(NOLOCK)" : "";
            return "";
        }

        public static string ResolveJoinSql(List<JoinAssTable> joinAssTables, ref string sql, LambdaExpression selectExp)
        {
            StringBuilder builder = new StringBuilder(Environment.NewLine);
            StringBuilder sqlBuilder = new StringBuilder();
            //循环拼接连表对象
            for (var i = 0; i < joinAssTables.Count; i++)
            {
                var item = joinAssTables[i];
                if (item.Action == JoinAction.defaults)//默认连表
                {
                    builder.Append($" {item.JoinMode.ToString()}  JOIN {item.LeftTabName} ON {item.LeftTabName}.{item.LeftAssName}={item.RightTabName}.{item.RightAssName} " + Environment.NewLine);
                }
                else//sql连表
                {
                    builder.Append(" " + item.JoinSql);
                    //判断是否需要显示连表的字段
                    if (item.TableType == null)
                    {
                        continue;
                    }
                }
                sqlBuilder.Append(GetTableField(EntityCache.QueryEntity(item.TableType)) + (i != joinAssTables.Count - 1 ? "," : ""));
            }
            if (sqlBuilder.Length != 0 && selectExp == null)//不是自定义返回视图则显示所有字段
            {
                sql += "," + sqlBuilder.ToString();
                //判断是否有前表命名冲突的字段
                var sqlArray = sql.Split(',').ToList();
                var changeArray = sqlArray.Select(x =>
                  new
                  {
                      oldValue = x,
                      newValue = x.Substring(x.IndexOf(".") + 1)
                  }).GroupBy(x => x.newValue);
                foreach (var fieldArr in changeArray)
                {
                    if (fieldArr.Count() > 1)
                    {
                        var fieldList = fieldArr.ToList();
                        for (var i = 1; i < fieldArr.Count(); i++)
                        {
                            var index = sqlArray.IndexOf(fieldList[i].oldValue);
                            if (index != -1)
                            {
                                sqlArray[index] += $" AS {fieldList[i].newValue}_{i}";
                            }
                        }
                    }
                }
                sql = string.Join(",", sqlArray);
            }
            return builder.ToString();
        }
    }
}
