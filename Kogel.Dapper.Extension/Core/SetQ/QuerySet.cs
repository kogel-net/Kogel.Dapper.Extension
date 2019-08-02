using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Model;
using System.Linq;
using Kogel.Dapper.Extension.Extension;
using Dapper;
using System.Text;
using static Dapper.SqlMapper;
using Kogel.Dapper.Extension.Expressions;
using System.Text.RegularExpressions;
using Kogel.Dapper.Extension.Extension.From;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 查询集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuerySet<T> : Aggregation<T>, IQuerySet<T>
    {
        public QuerySet(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {
            TableType = typeof(T);
            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Query
            };
            sqlProvider.Context = SetContext;
            WhereExpressionList = new List<LambdaExpression>();
            WhereBuilder = new StringBuilder();
            Params = new DynamicParameters();
        }

        public QuerySet(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {
            TableType = typeof(T);
            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Query
            };
            sqlProvider.Context = SetContext;
            WhereExpressionList = new List<LambdaExpression>();
            WhereBuilder = new StringBuilder();
            Params = new DynamicParameters();
        }

        internal QuerySet(IDbConnection conn, SqlProvider sqlProvider, Type tableType, LambdaExpression whereExpression, LambdaExpression selectExpression, int? topNum, Dictionary<LambdaExpression, EOrderBy> orderbyExpressionList, IDbTransaction dbTransaction, bool noLock) : base(conn, sqlProvider, dbTransaction)
        {
            TableType = tableType;
            WhereExpression = whereExpression;
            SelectExpression = selectExpression;
            TopNum = topNum;
            OrderbyExpressionList = orderbyExpressionList;
            NoLock = noLock;

            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Query
            };

            sqlProvider.Context = SetContext;
        }

        public QuerySet<T> Where(Expression<Func<T, bool>> predicate)
        {
            WhereExpressionList.Add(predicate);
            return this;
        }
        public QuerySet<T> Where<TWhere>(Expression<Func<TWhere, bool>> predicate)
        {
            WhereExpressionList.Add(predicate);
            return this;
        }
        /// <summary>
        /// 不锁表查询(此方法只支持Mssql)
        /// </summary>
        /// <returns></returns>
        public QuerySet<T> WithNoLock()
        {
            NoLock = true;
            return this;
        }

        public QuerySet<T> Where(T model)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "parameter");
            var entityType = EntityCache.QueryEntity(typeof(T));
            foreach (var item in entityType.Properties)
            {
                string name = item.Name;
                object value = item.GetValue(model, null);
                if (value != null)
                {
                    if (item.PropertyType == typeof(int) || item.PropertyType == typeof(double) || item.PropertyType == typeof(decimal) || item.PropertyType == typeof(long))
                    {
                        if (Convert.ToDecimal(value) == 0)
                        {
                            continue;
                        }
                    }
                    else if (item.PropertyType == typeof(DateTime))
                    {
                        var time = Convert.ToDateTime(value);
                        if (time == DateTime.MinValue)
                        {
                            continue;
                        }
                    }
                    var whereExpress = Expression.Equal(Expression.Property(parameter, name), Expression.Constant(value));
                    WhereExpressionList.Add(Expression.Lambda<Func<T, bool>>(TrimExpression.Trim(whereExpress), parameter));
                }
            }
            return this;
        }

        /// <summary>
        /// 动态化查讯(转换成表达式树集合)
        /// </summary>
        /// <typeparam name="T">对应表</typeparam>
        /// <param name="dynamicTree"></param>
        /// <returns></returns>
        public QuerySet<T> Where(Dictionary<string, DynamicTree> dynamicTree)
        {
            foreach (var key in dynamicTree.Keys)
            {
                DynamicTree tree = dynamicTree[key];
                if (!string.IsNullOrEmpty(tree.Value))
                {
                    //如果不存在对应表就使用默认表
                    var paramType = EntityCache.QueryEntity(typeof(T)).Type;
                    ParameterExpression param = Expression.Parameter(paramType, "param");
                    object value = tree.Value;
                    if (tree.ValueType == DbType.DateTime)
                    {
                        value = Convert.ToDateTime(value);
                    }
                    Expression whereExpress = null;
                    switch (tree.Operators)
                    {
                        case ExpressionType.Equal://等于
                            whereExpress = Expression.Equal(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                        case ExpressionType.GreaterThanOrEqual://大于等于
                            whereExpress = Expression.GreaterThanOrEqual(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                        case ExpressionType.LessThanOrEqual://小于等于
                            whereExpress = Expression.LessThanOrEqual(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                        case ExpressionType.Call://模糊查询
                            var method = typeof(string).GetMethodss().FirstOrDefault(x => x.Name.Equals("Contains"));
                            whereExpress = Expression.Call(Expression.Property(param, tree.Field), method, new Expression[] { Expression.Constant(value) });
                            break;
                        default:
                            whereExpress = Expression.Equal(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                    }
                    WhereExpressionList.Add(Expression.Lambda<Func<T, bool>>(TrimExpression.Trim(whereExpress), param));
                }
            }
            return this;
        }
        /// <summary>
        /// 使用sql查询条件
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public QuerySet<T> Where(string sqlWhere, object param = null)
        {
            WhereBuilder.Append(" AND " + sqlWhere);
            if (param != null)
            {
                Params.AddDynamicParams(param);
            }
            return this;
        }
        public QuerySet<T> Where<TWhere1, TWhere2>(Expression<Func<TWhere1, TWhere2, bool>> exp)
        {
            var sqlWhere = new WhereExpression(exp, $"Where_{Params.ParameterNames.Count()}_", SqlProvider.ProviderOption);
            WhereBuilder.Append(sqlWhere.SqlCmd);
            if (sqlWhere.Param != null)
            {
                Params.AddDynamicParams(sqlWhere.Param);
            }
            return this;
        }
        /// <summary>
        /// 连表
        /// </summary>
        /// <typeparam name="TOuter">主表</typeparam>
        /// <typeparam name="TInner">外表</typeparam>
        /// <param name="rightField">主表关联键</param>
        /// <param name="leftField">外表关联键</param>
        /// <param name="joinMode">连接方式</param>
        /// <returns></returns>
        public QuerySet<T> Join<TOuter, TInner>(Expression<Func<TOuter, object>> rightField, Expression<Func<TInner, object>> leftField, JoinMode joinMode = JoinMode.LEFT)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.defaults,
                JoinMode = joinMode,
                RightTabName = EntityCache.QueryEntity(typeof(TOuter)).Name,
                RightAssName = rightField.GetCorrectPropertyName(),
                LeftTabName = EntityCache.QueryEntity(typeof(TInner)).Name,
                LeftAssName = leftField.GetCorrectPropertyName(),
                TableType = typeof(TInner)
            });
            return this;
        }
        /// <summary>
        /// 连表
        /// </summary>
        /// <typeparam name="TOuter">主表</typeparam>
        /// <typeparam name="TInner">副表</typeparam>
        /// <param name="exp">条件</param>
        /// <param name="joinMode">连接类型</param>
        /// <param name="IsDisField">是否需要显示表字段</param>
        /// <returns></returns>
        public QuerySet<T> Join<TOuter, TInner>(Expression<Func<TOuter, TInner, bool>> exp, JoinMode joinMode = JoinMode.LEFT, bool IsDisField = true)
        {
            var joinWhere = new WhereExpression(exp, $"Where_{Params.ParameterNames.Count()}_", SqlProvider.ProviderOption);
            Regex whereRex = new Regex("AND");
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.sqlJoin,
                JoinSql = $"{joinMode.ToString()} JOIN {EntityCache.QueryEntity(typeof(TInner)).Name} ON {  whereRex.Replace(joinWhere.SqlCmd, "", 1)}",
                TableType = (IsDisField ? typeof(TInner) : null)
            });
            if (joinWhere.Param != null)
            {
                Params.AddDynamicParams(joinWhere.Param);
            }
            return this;
        }
        public QuerySet<T> Join(string SqlJoin)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.sqlJoin,
                JoinSql = SqlJoin,
            });
            return this;
        }
        /// <summary>
        /// 连接(通过sql连接，不指定表实体默认为不增加该表显示字段)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="SqlJoin"></param>
        /// <returns></returns>
        public QuerySet<T> Join<TInner>(string SqlJoin)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.sqlJoin,
                JoinSql = SqlJoin,
                TableType = typeof(TInner)
            });
            return this;
        }
        /// <summary>
        /// 字段匹配(适用于实体类字段和数据库字段不一致时,返回值为Dynamic类型时不适用)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public QuerySet<T> FieldMatch<TSource>()
        {
            EntityObject entity = EntityCache.QueryEntity(typeof(TSource));
            SqlMapper.SetTypeMap(entity.Type, new CustomPropertyTypeMap(entity.Type, (type, column) => type.GetPropertys(entity.FieldPairs.FirstOrDefault(x => x.Value.Equals(column)).Key)));
            return this;
        }

        #region 多表索引扩展
        public ISelectFrom<T, T1, T2> From<T1, T2>()
        {
            return new ISelectFrom<T, T1, T2>(this);
        }
        public ISelectFrom<T, T1, T2, T3> From<T1, T2, T3>()
        {
            return new ISelectFrom<T, T1, T2, T3>(this);
        }
        #endregion
    }
}
