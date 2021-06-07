using Dapper;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public abstract class IResolveExpression
    {
        protected SqlProvider provider;
        protected IProviderOption providerOption;
        protected AbstractSet abstractSet => provider.Context.Set;

        public IResolveExpression(SqlProvider provider)
        {
            this.provider = provider;
            this.providerOption = provider.ProviderOption;
        }

        /// <summary>
        /// 字段列列表
        /// </summary>
        private static Hashtable _tableFieldMap = new Hashtable();

        /// <summary>
        /// 根据反射对象获取表字段
        /// </summary>
        /// <param name="propertyInfos"></param>
        /// <returns></returns>
        public virtual string GetTableField(EntityObject entityObject)
        {
            lock (_tableFieldMap)
            {
                string fieldBuild = (string)_tableFieldMap[entityObject];
                if (fieldBuild == null)
                {
                    var propertyInfos = entityObject.Properties;
                    string asName = entityObject.Name == entityObject.AsName ? providerOption.CombineFieldName(entityObject.AsName) : entityObject.AsName;
                    fieldBuild = string.Join(",", entityObject.FieldPairs.Select(field => $"{asName}.{providerOption.CombineFieldName(field.Value) }"));
                    _tableFieldMap.Add(entityObject, fieldBuild);
                }
                return fieldBuild;
            }
        }

        /// <summary>
        /// 解析查询字段
        /// </summary>
        /// <param name="topNum"></param>
        /// <returns></returns>
        public abstract string ResolveSelect(int? topNum);

        /// <summary>
        /// 解析查询条件
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public virtual string ResolveWhereList(string prefix = null)
        {
            //添加Linq生成的sql条件和参数
            List<LambdaExpression> lambdaExpressionList = abstractSet.WhereExpressionList;
            StringBuilder builder = new StringBuilder("WHERE 1=1 ");
            for (int i = 0; i < lambdaExpressionList.Count; i++)
            {
                prefix = $"{prefix}{(i)}_";
                var whereParam = new WhereExpression(lambdaExpressionList[i], prefix, provider);
                builder.Append(whereParam.SqlCmd);
                //参数
                foreach (var paramKey in whereParam.Param.ParameterNames)
                {
                    abstractSet.Params.Add(paramKey, whereParam.Param.Get<object>(paramKey));
                }
            }
            //添加自定义sql生成的条件和参数
            if (abstractSet.WhereBuilder != null && abstractSet.WhereBuilder.Length != 0)
            {
                //添加自定义条件sql
                builder.Append(abstractSet.WhereBuilder);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 解析分组
        /// </summary>
        /// <returns></returns>
        public virtual string ResolveGroupBy()
        {
            StringBuilder builder = new StringBuilder();
            var groupExpression = abstractSet.GroupExpressionList;
            if (groupExpression != null && groupExpression.Any())
            {
                for (int i = 0; i < groupExpression.Count; i++)
                {
                    var groupParam = new GroupExpression(groupExpression[i], $"Group_{i}", provider);
                    if (builder.Length != 0)
                        builder.Append(",");
                    builder.Append(groupParam.SqlCmd);
                }
                builder.Insert(0, " GROUP BY ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 解析分组聚合条件
        /// </summary>
        /// <returns></returns>
        public virtual string ResolveHaving()
        {
            StringBuilder builder = new StringBuilder();
            var havingExpression = abstractSet.HavingExpressionList;
            if (havingExpression != null && havingExpression.Any())
            {
                for (int i = 0; i < havingExpression.Count; i++)
                {
                    var whereParam = new WhereExpression(havingExpression[i], $"Having_{i}", provider);
                    builder.Append(whereParam.SqlCmd);
                    //参数
                    foreach (var paramKey in whereParam.Param.ParameterNames)
                    {
                        abstractSet.Params.Add(paramKey, whereParam.Param.Get<object>(paramKey));
                    }
                }
                builder.Insert(0, " Having 1=1 ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 解析排序
        /// </summary>
        /// <returns></returns>
        public virtual string ResolveOrderBy()
        {
            var orderByList = abstractSet?.OrderbyExpressionList.Select(a =>
            {
                var entity = EntityCache.QueryEntity(a.Key.Type.GenericTypeArguments[0]);
                var columnName = entity.FieldPairs[a.Key.Body.GetCorrectPropertyName()];
                string orderBySql = $"{entity.AsName}.{providerOption.CombineFieldName(columnName)}{(a.Value == EOrderBy.Asc ? " ASC " : " DESC ")}";
                return orderBySql;
            }) ?? new List<string>();
            if (!orderByList.Any() && (abstractSet.OrderbyBuilder == null || abstractSet.OrderbyBuilder.Length == 0))
                return "";

            return $"ORDER BY {string.Join(",", orderByList)} {abstractSet.OrderbyBuilder}";
        }

        /// <summary>
        /// 解析查询总和
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public abstract string ResolveSum(LambdaExpression selector);

        /// <summary>
        /// 解析查询最小值
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public abstract string ResolveMax(LambdaExpression selector);

        /// <summary>
        /// 解析查询最大值
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public abstract string ResolveMin(LambdaExpression selector);

        /// <summary>
        /// 解析更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        public virtual UpdateExpression<T> ResolveUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            return new UpdateExpression<T>(updateExpression, provider);
        }

        /// <summary>
        /// 解析更新语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="param"></param>
        /// <param name="excludeFields"></param>
        /// <returns></returns>
        public virtual string ResolveUpdate<T>(T entity, DynamicParameters param, string[] excludeFields)
        {
            var entityObject = EntityCache.QueryEntity(typeof(T));
            StringBuilder builder = new StringBuilder();
            foreach (var entityField in entityObject.EntityFieldList)
            {
                string name = entityField.FieldName;
                //是否是排除字段
                if (excludeFields != null && (excludeFields.Contains(entityField.PropertyInfo.Name) || excludeFields.Contains(name)))
                {
                    continue;
                }
                //var customAttributes = entityField.PropertyInfo.GetCustomAttributess(true);
                ////导航属性排除
                //if (customAttributes.Any(x => x.GetType().Equals(typeof(ForeignKey))))
                //{
                //    continue;
                //}
                //是否自增
                if (entityField.IsIncrease)
                {
                    continue;
                }
                object value = entityField.PropertyInfo.GetValue(entity);
                if (builder.Length != 0)
                {
                    builder.Append(",");
                }
                string paramName = $"{providerOption.ParameterPrefix}Update_{name}_{param.ParameterNames.Count()}";
                builder.Append($"{providerOption.CombineFieldName(name)}={paramName}");
                param.Add($"{paramName}", value);
            }
            builder.Insert(0, " SET ");
            return builder.ToString();
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entites"></param>
        /// <param name="param"></param>
        /// <param name="excludeFields"></param>
        /// <returns></returns>
        public virtual string ResolveBulkUpdate<T>(IEnumerable<T> entites, DynamicParameters param, string[] excludeFields)
        {
            var entityObject = EntityCache.QueryEntity(typeof(T));
            StringBuilder builder = new StringBuilder();
            foreach (var entityField in entityObject.EntityFieldList)
            {
                string name = entityField.FieldName;
                //是否是排除字段
                if (excludeFields != null && (excludeFields.Contains(entityField.PropertyInfo.Name) || excludeFields.Contains(name)))
                {
                    continue;
                }
                //var customAttributes = entityField.PropertyInfo.GetCustomAttributess(true);
                ////导航属性排除
                //if (customAttributes.Any(x => x.GetType().Equals(typeof(ForeignKey))))
                //{
                //    continue;
                //}
                //是否自增
                if (entityField.IsIncrease)
                {
                    continue;
                }
                if (builder.Length != 0)
                {
                    builder.Append(",");
                }
                string paramName = $"{providerOption.ParameterPrefix}{name}";
                builder.Append($"{providerOption.CombineFieldName(name)}={paramName}");
                param.Add(paramName);
            }
            builder.Insert(0, " SET ");
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nolock"></param>
        /// <returns></returns>
        public virtual string ResolveWithNoLock(bool nolock)
        {
            return nolock ? "(NOLOCK)" : "";
        }

        /// <summary>
        /// 解析连表查询
        /// </summary>
        /// <param name="joinAssTables"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual string ResolveJoinSql(List<JoinAssTable> joinAssTables, ref string sql)
        {
            StringBuilder builder = new StringBuilder(Environment.NewLine);
            if (joinAssTables.Count != 0)
            {
                sql = sql.TrimEnd();
                //循环拼接连表对象
                for (int i = 0; i < joinAssTables.Count; i++)
                {
                    //当前连表对象
                    var item = joinAssTables[i];
                    if (item.IsMapperField == false)
                    {
                        continue;
                    }
                    item.MapperList.Clear();
                    if (item.TableType != null)
                    {
                        //连表实体
                        EntityObject leftEntity = EntityCache.QueryEntity(item.TableType);
                        //默认连表
                        if (item.Action == JoinAction.Default || item.Action == JoinAction.Navigation)
                        {
                            string leftTable = providerOption.CombineFieldName(item.LeftTabName);
                            builder.Append($@" {item.JoinMode} JOIN 
                                       {leftTable} {leftEntity.AsName} ON {leftEntity.AsName}
                                      .{providerOption.CombineFieldName(item.LeftAssName)} = {providerOption.CombineFieldName(item.RightTabName)}
                                      .{providerOption.CombineFieldName(item.RightAssName)} " + Environment.NewLine);
                        }
                        else//sql连表
                        {
                            builder.Append(" " + item.JoinSql);
                            //判断是否需要显示连表的字段
                            if (!item.IsMapperField)
                            {
                                continue;
                            }
                        }
                        //自定义返回
                        if (provider.Context.Set.SelectExpression != null)
                        {
                            continue;
                        }
                        FieldDetailWith(ref sql, item, leftEntity);
                    }
                    else
                    {
                        if (item.Action == JoinAction.Sql)
                        {
                            builder.Append(" " + item.JoinSql);
                            //判断是否需要显示连表的字段
                            if (!item.IsMapperField)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 字段处理
        /// </summary>
        /// <param name="masterSql"></param>
        /// <param name="joinAssTable"></param>
        /// <param name="joinEntity"></param>
        /// <returns></returns>
        private string FieldDetailWith(ref string masterSql, JoinAssTable joinAssTable, EntityObject joinEntity)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            //表名称
            string joinTableName = joinEntity.AsName == joinEntity.Name ? providerOption.CombineFieldName(joinEntity.Name) : joinEntity.AsName;
            //查询的字段
            var fieldPairs = joinAssTable.SelectFieldPairs != null && joinAssTable.SelectFieldPairs.Any() ? joinAssTable.SelectFieldPairs : joinEntity.FieldPairs;
            foreach (string fieldValue in fieldPairs.Values)
            {
                if (masterSql.LastIndexOf(',') == masterSql.Length - 1 && sqlBuilder.Length == 0)
                    sqlBuilder.Append($"{joinTableName}.");
                else
                    //首先添加表名称
                    sqlBuilder.Append($",{joinTableName}.");
                //字段
                string field = providerOption.CombineFieldName(fieldValue);
                //字符出现的次数
                int repeatCount = masterSql.Split(new string[] { field }, StringSplitOptions.None).Length - 1;
                //添加字段
                sqlBuilder.Append(field);
                if (repeatCount > 0)
                {
                    sqlBuilder.Append($" AS {fieldValue}_{repeatCount}");
                    joinAssTable.MapperList.Add(fieldValue, $"{fieldValue}_{repeatCount}");
                }
                else
                {
                    joinAssTable.MapperList.Add(fieldValue, fieldValue);
                }
            }
            //导航属性目前有点问题
            //var joinEntityType = joinAssTable.IsDto == false ? joinEntity.Type : joinAssTable.DtoType;
            ////重新注册实体映射
            //SqlMapper.SetTypeMap(joinEntityType, new CustomPropertyTypeMap(joinEntityType,
            //		(type, column) =>
            //		type.GetPropertys(joinAssTable.MapperList.FirstOrDefault(x => x.Value.Equals(column)).Key)
            //		), true);
            //设置sql字段
            masterSql += sqlBuilder;
            return masterSql;
        }

        /// <summary>
        /// 解析批量新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="excludeFields"></param>
        /// <returns></returns>
        public virtual string ResolveBulkInsert<T>(IEnumerable<T> entitys, string[] excludeFields)
        {
            var sqlBuilder = new StringBuilder();
            DynamicParameters parameters = new DynamicParameters();
            //当前数据索引
            int index = 0;
            foreach (var item in entitys)
            {
#if NET45 || NET451
                var resolveInsertParamsAndValues = ResolveInsertParamsAndValues(item, excludeFields, index++);
                var tableName = resolveInsertParamsAndValues.Item1;
                var fieldStr = resolveInsertParamsAndValues.Item2;
                var paramStr = resolveInsertParamsAndValues.Item3;
                var parameter = resolveInsertParamsAndValues.Item4;
#else
                var (tableName, fieldStr, paramStr, parameter) = ResolveInsertParamsAndValues(item, excludeFields, index++);
#endif

                //增加字段(只加一次)
                if (sqlBuilder.Length == 0)
                {
                    sqlBuilder.Append($"INSERT INTO {tableName}");
                    sqlBuilder.Append($"({fieldStr})");
                }
                //增加参数
                if (index == 1)
                    sqlBuilder.Append($"Values({paramStr})");
                else
                    sqlBuilder.Append($",({paramStr})");
                parameters.AddDynamicParams(parameter);
            }
            provider.Params.AddDynamicParams(parameters);
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tValue></param>
        /// <param name="excludeFields">排除字段</param>
        /// <param name="index">当前数据索引</param>
        /// <returns></returns>
        protected Tuple<string, StringBuilder, StringBuilder, DynamicParameters> ResolveInsertParamsAndValues<T>(T tValue, string[] excludeFields = null, int index = 0)
        {
            var fieldBuilder = new StringBuilder(64);
            var paramBuilder = new StringBuilder(64);
            DynamicParameters parameters = new DynamicParameters();

            var entityProperties = EntityCache.QueryEntity(typeof(T));
            var tableName = provider.FormatTableName(false, false); // entityProperties.Name;
            var isAppend = false;

            foreach (var entityField in entityProperties.EntityFieldList)
            {
                string fieldName = entityField.FieldName;
                //是否是排除字段
                if (excludeFields != null && (excludeFields.Contains(fieldName) || excludeFields.Contains(entityField.PropertyInfo.Name)))
                {
                    continue;
                }
                //是否自增
                if (entityField.IsIncrease)
                {
                    continue;
                }
                if (isAppend)
                {
                    fieldBuilder.Append(",");
                    paramBuilder.Append(",");
                }
                //字段添加
                fieldBuilder.Append($"{fieldName}");
                //参数添加
                paramBuilder.Append($"{provider.ProviderOption.ParameterPrefix}{fieldName}{index}");
                parameters.Add($"{provider.ProviderOption.ParameterPrefix}{fieldName}{index}", entityField.PropertyInfo.GetValue(tValue));

                isAppend = true;
            }
            return new Tuple<string, StringBuilder, StringBuilder, DynamicParameters>(tableName, fieldBuilder, paramBuilder, parameters);
        }
    }
}
