using Dapper;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
   public abstract class IResolveExpression
    {
        protected IProviderOption providerOption;
        public IResolveExpression(IProviderOption providerOption)
        {
            this.providerOption = providerOption;
        }
        /// <summary>
        /// 根据反射对象获取表字段
        /// </summary>
        /// <param name="propertyInfos"></param>
        /// <returns></returns>
        public virtual string GetTableField(EntityObject entityObject)
        {
            var propertyInfos = entityObject.Properties;
			string asName = entityObject.Name == entityObject.AsName ? providerOption.CombineFieldName(entityObject.AsName) : entityObject.AsName;
			string property = string.Join(",", entityObject.FieldPairs.Select(field => $"{asName}.{providerOption.CombineFieldName(field.Value) }"));
            return property;
        }
        /// <summary>
        /// 解析查询字段
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="selector"></param>
        /// <param name="topNum"></param>
        /// <param name="Param"></param>
        /// <returns></returns>
        public abstract string ResolveSelect(EntityObject entityObject, LambdaExpression selector, int? topNum, DynamicParameters Param);
        /// <summary>
        /// 解析查询条件
        /// </summary>
        /// <param name="abstractSet"></param>
        /// <param name="whereSql"></param>
        /// <param name="Params"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public virtual List<WhereExpression> ResolveWhereList(AbstractSet abstractSet, ref string whereSql, DynamicParameters Params, string prefix = null)
        {
            //添加Linq生成的sql条件和参数
            List<LambdaExpression> lambdaExpressionList = abstractSet.WhereExpressionList;
            StringBuilder builder = new StringBuilder("WHERE 1=1 ");
            List<WhereExpression> whereExpressionList = new List<WhereExpression>();
            for (int i = 0; i < lambdaExpressionList.Count; i++)
            {
				var whereParam = new WhereExpression(lambdaExpressionList[i], $"{prefix}_{i}", providerOption);
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
				//参数
				foreach (var paramKey in abstractSet.Params.ParameterNames)
				{
					if (!Params.ParameterNames.Contains(paramKey))
						Params.Add(paramKey, abstractSet.Params.Get<object>(paramKey));
				}
			}
            whereSql = builder.ToString();
            return whereExpressionList;
        }
        /// <summary>
        /// 解析排序
        /// </summary>
        /// <param name="abstractSet"></param>
        /// <returns></returns>
        public virtual string ResolveOrderBy(AbstractSet abstractSet)
        {
            var orderByList = abstractSet?.OrderbyExpressionList.Select(a =>
            {
                var entity = EntityCache.QueryEntity(a.Key.Type.GenericTypeArguments[0]);
                var columnName = a.Key.Body.GetCorrectPropertyName();
                return $"{entity.GetAsName(providerOption)}" + providerOption.CombineFieldName(columnName) + (a.Value == EOrderBy.Asc ? " ASC " : " DESC ");
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
        /// 解析查询更新
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual string ResolveSelectOfUpdate(EntityObject entityObject, LambdaExpression selector)
        {
            var selectSql = "";
            if (selector == null)
            {
                var propertyBuilder = new StringBuilder();
                foreach (var propertyInfo in entityObject.Properties)
                {
                    if (propertyBuilder.Length > 0)
                        propertyBuilder.Append(",");
                    propertyBuilder.AppendFormat($"INSERTED.{ providerOption.CombineFieldName(propertyInfo.GetColumnAttributeName())} { providerOption.CombineFieldName(propertyInfo.Name)}");
                }
                selectSql = propertyBuilder.ToString();
            }
            else
            {
                var nodeType = selector.Body.NodeType;
                if (nodeType == ExpressionType.MemberAccess)
                {
                    var columnName = ((MemberExpression)selector.Body).Member.GetColumnAttributeName();
                    selectSql = "INSERTED." + providerOption.CombineFieldName(columnName);
                }
                else if (nodeType == ExpressionType.MemberInit)
                {
                    var memberInitExpression = (MemberInitExpression)selector.Body;
                    selectSql = string.Join(",", memberInitExpression.Bindings.Select(a => "INSERTED." + providerOption.CombineFieldName(a.Member.GetColumnAttributeName())));
                }
            }
            return "OUTPUT " + selectSql;
        }
        /// <summary>
        /// 解析更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        public virtual UpdateExpression ResolveUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            return new UpdateExpression(updateExpression, providerOption);
        }
		/// <summary>
		/// 解析更新语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="Param"></param>
		/// <param name=""></param>
		/// <param name="excludeFields"></param>
		/// <returns></returns>
		public virtual string ResolveUpdates<T>(T t, DynamicParameters Param, string[] excludeFields)
		{
			var entity = EntityCache.QueryEntity(t.GetType());
			var properties = entity.Properties;
			StringBuilder builder = new StringBuilder();
			foreach (var propertiy in properties)
			{
				//是否是排除字段
				if (excludeFields != null && excludeFields.Contains(propertiy.Name))
				{
					continue;
				}
				//主键标识
				var typeAttribute = propertiy.GetCustomAttributess(true).FirstOrDefault(x => x.GetType().Equals(typeof(Identity)));
				if (typeAttribute != null)
				{
					var identity = typeAttribute as Identity;
					//是否自增
					if (identity.IsIncrease)
					{
						continue;
					}
				}
				object value = propertiy.GetValue(t);
				string name = entity.FieldPairs[propertiy.Name];
				if (builder.Length != 0)
				{
					builder.Append(",");
				}
				builder.Append($"{providerOption.CombineFieldName(name)}={providerOption.ParameterPrefix}Update_{name}");
				Param.Add($"{providerOption.ParameterPrefix}Update_{name}", value);
			}
			builder.Insert(0, " SET ");
			return builder.ToString();
		}
        public virtual string ResolveWithNoLock(bool nolock)
        {
            return nolock ? "(NOLOCK)" : "";
        }
        /// <summary>
        /// 解析连表查询
        /// </summary>
        /// <param name="joinAssTables"></param>
        /// <param name="sql"></param>
        /// <param name="selectExp"></param>
        /// <returns></returns>
        public virtual string ResolveJoinSql(List<JoinAssTable> joinAssTables, ref string sql, LambdaExpression selectExp)
        {
            StringBuilder builder = new StringBuilder(Environment.NewLine);
            if (joinAssTables.Count != 0)
            {
                StringBuilder sqlBuilder = new StringBuilder();
                //循环拼接连表对象
                for (var i = 0; i < joinAssTables.Count; i++)
                {
                    var item = joinAssTables[i];
                    if (item.Action == JoinAction.defaults)//默认连表
                    {
                        builder.Append($@" {item.JoinMode.ToString()} JOIN 
                                       {providerOption.CombineFieldName(item.LeftTabName)} ON {providerOption.CombineFieldName(item.LeftTabName)}
                                      .{providerOption.CombineFieldName(item.LeftAssName)} = {providerOption.CombineFieldName(item.RightTabName)}
                                      .{providerOption.CombineFieldName(item.RightAssName)} " + Environment.NewLine);
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
                                    sqlArray[index] += $" AS {fieldList[i].newValue.Substring(0, fieldList[i].newValue.Length - 1)}_{i + providerOption.CloseQuote}";
                                }
                            }
                        }
                    }
                    sql = string.Join(",", sqlArray);
                }
            }
            return builder.ToString();
        }
    }
}
