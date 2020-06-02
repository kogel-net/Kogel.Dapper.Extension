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
            GroupExpressionList = new List<LambdaExpression>();
            HavingExpressionList = new List<LambdaExpression>();
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
            GroupExpressionList = new List<LambdaExpression>();
            HavingExpressionList = new List<LambdaExpression>();
        }
        #region 基础函数
        public QuerySet<T> AsTableName(Type type, string tableName)
        {
            SqlProvider.AsTableNameDic.Add(type, tableName);
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
        /// <summary>
        /// 字段匹配[已弃用]
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public QuerySet<T> FieldMatch<TSource>()
        {
            return this;
        }
        #endregion
        #region 条件
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
        /// 动态化查讯(转换成表达式树集合)  注意，int参数不会判断为0的值
        /// </summary>
        /// <typeparam name="T">对应表</typeparam>
        /// <param name="dynamicTree"></param>
        /// <returns></returns>
        public QuerySet<T> Where(Dictionary<string, DynamicTree> dynamicTree)
        {
            if (dynamicTree != null)
            {
                foreach (var key in dynamicTree.Keys)
                {
                    DynamicTree tree = dynamicTree[key];
                    if (tree != null && !string.IsNullOrEmpty(tree.Value))
                    {
                        Type tableType = typeof(T);
                        if (!string.IsNullOrEmpty(tree.Table))
                        {
                            tableType = EntityCache.QueryEntity(tree.Table).Type;
                        }
                        //如果不存在对应表就使用默认表
                        ParameterExpression param = Expression.Parameter(tableType, "param");
                        object value = tree.Value;
                        if (value == null)
                        {
                            continue;
                        }
                        else if (tree.ValueType == DbType.DateTime)
                        {
                            value = Convert.ToDateTime(value);
                        }
                        else if (tree.ValueType == DbType.String)
                        {
                            value = Convert.ToString(value);
                            if ("" == value.ToString())
                            {
                                continue;
                            }
                        }
                        else if (tree.ValueType == DbType.Int32)
                        {
                            int number = Convert.ToInt32(value);
                            value = number;
                            if (0 == number)
                            {
                                continue;
                            }
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
                        WhereExpressionList.Add(Expression.Lambda(TrimExpression.Trim(whereExpress), param));
                    }
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
                Params.AddDynamicParams(param, true);
            }
            return this;
        }

        public QuerySet<T> Where<TWhere1, TWhere2>(Expression<Func<TWhere1, TWhere2, bool>> exp)
        {
            WhereExpressionList.Add(exp);
            return this;
        }

        /// <summary>
        /// 带前置条件的Where判断
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="where"></param>
        /// <param name="truePredicate"></param>
        /// <param name="falsePredicate"></param>
        /// <returns></returns>
        public QuerySet<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate)
        {
            if (where)
                WhereExpressionList.Add(truePredicate);
            else
                WhereExpressionList.Add(falsePredicate);
            return this;
        }
        /// <summary>
        /// 带前置条件的Where判断
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <param name="where"></param>
        /// <param name="truePredicate"></param>
        /// <param name="falsePredicate"></param>
        /// <returns></returns>
        public QuerySet<T> WhereIf<TWhere>(bool where, Expression<Func<TWhere, bool>> truePredicate, Expression<Func<TWhere, bool>> falsePredicate)
        {
            if (where)
                WhereExpressionList.Add(truePredicate);
            else
                WhereExpressionList.Add(falsePredicate);
            return this;
        }
        #endregion
        #region 连表
        /// <summary>
        /// 连表
        /// </summary>
        /// <typeparam name="TWhere">条件表</typeparam>
        /// <typeparam name="TInner">连接表</typeparam>
        /// <param name="rightField">主表关联键</param>
        /// <param name="leftField">外表关联键</param>
        /// <param name="joinMode">连接方式</param>
        /// <returns></returns>
        public QuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, object>> rightField, Expression<Func<TInner, object>> leftField, JoinMode joinMode = JoinMode.LEFT)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Default,
                JoinMode = joinMode,
                RightTabName = EntityCache.QueryEntity(typeof(TWhere)).AsName,
                RightAssName = rightField.GetCorrectPropertyName(),
                LeftTabName = EntityCache.QueryEntity(typeof(TInner)).AsName,
                LeftAssName = leftField.GetCorrectPropertyName(),
                TableType = typeof(TInner)
            });
            return this;
        }

        /// <summary>
        /// 连表
        /// </summary>
        /// <typeparam name="TInner">副表</typeparam>
        /// <param name="expression">条件</param>
        /// <param name="joinMode">连接类型</param>
        /// <param name="isDisField">是否需要显示表字段</param>
        /// <returns></returns>
        public QuerySet<T> Join<TInner>(LambdaExpression expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            var joinWhere = new WhereExpression(expression, $"{Params.ParameterNames.Count()}", SqlProvider);
            Regex whereRex = new Regex("AND");
            string tableName = SqlProvider.FormatTableName(false, true, typeof(TInner));
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Sql,
                JoinSql = $"{joinMode.ToString()} JOIN {tableName} ON {  whereRex.Replace(joinWhere.SqlCmd, "", 1)}",
                TableType = (isDisField ? typeof(TInner) : null)
            });
            if (joinWhere.Param != null)
            {
                Params.AddDynamicParams(joinWhere.Param, true);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="expression"></param>
        /// <param name="joinMode"></param>
        /// <param name="isDisField"></param>
        /// <returns></returns>
        public QuerySet<T>Join<TWhere,TInner>(Expression<Func<TWhere, TInner, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            Join<TInner>(expression, joinMode, isDisField);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TWhere2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="joinMode"></param>
        /// <param name="isDisField"></param>
        /// <returns></returns>
        public QuerySet<T> Join<TWhere, TInner,TWhere2>(Expression<Func<TWhere, TInner, TWhere2, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            Join<TInner>(expression, joinMode, isDisField);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TWhere2"></typeparam>
        /// <typeparam name="TWhere3"></typeparam>
        /// <param name="expression"></param>
        /// <param name="joinMode"></param>
        /// <param name="isDisField"></param>
        /// <returns></returns>
        public QuerySet<T> Join<TWhere, TInner, TWhere2, TWhere3>(Expression<Func<TWhere, TInner, TWhere2, TWhere3, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            Join<TInner>(expression, joinMode, isDisField);
            return this;
        }

        public QuerySet<T> Join(string SqlJoin)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Sql,
                JoinSql = SqlJoin,
                IsMapperField = false
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
                Action = JoinAction.Sql,
                JoinSql = SqlJoin,
                TableType = typeof(TInner),
            });
            return this;
        }
        #endregion
        #region 多表索引扩展
        public ISelectFrom<T, T1, T2> From<T1, T2>()
        {
            return new ISelectFrom<T, T1, T2>(this);
        }
        public ISelectFrom<T, T1, T2, T3> From<T1, T2, T3>()
        {
            return new ISelectFrom<T, T1, T2, T3>(this);
        }
        public ISelectFrom<T, T1, T2, T3, T4> From<T1, T2, T3, T4>()
        {
            return new ISelectFrom<T, T1, T2, T3, T4>(this);
        }
        #endregion

        #region 分组
        public QuerySet<T> GroupBy(Expression<Func<T, object>> groupByExp)
        {
            GroupExpressionList.Add(groupByExp);
            return this;
        }

        public QuerySet<T> GroupBy<TGroup>(Expression<Func<TGroup, object>> groupByExp)
        {
            GroupExpressionList.Add(groupByExp);
            return this;
        }
        public QuerySet<T> GroupByIf<TGroup>(bool where, Expression<Func<TGroup, object>> trueGroupByExp, Expression<Func<TGroup, object>> falseGroupByExp)
        {
            if (where)
                GroupExpressionList.Add(trueGroupByExp);
            else
                GroupExpressionList.Add(falseGroupByExp);
            return this;
        }

        /// <summary>
        /// 分组聚合条件
        /// </summary>
        /// <param name="havingExp"></param>
        /// <returns></returns>
        public QuerySet<T> Having(Expression<Func<T, object>> havingExp)
        {
            HavingExpressionList.Add(havingExp);
            return this;
        }

        /// <summary>
        /// 分组聚合条件(根据指定表)
        /// </summary>
        /// <typeparam name="THaving"></typeparam>
        /// <param name="havingExp"></param>
        /// <returns></returns>
        public QuerySet<T> Having<THaving>(Expression<Func<THaving, object>> havingExp)
        {
            HavingExpressionList.Add(havingExp);
            return this;
        }
        /// <summary>
        /// 分组聚合条件(带判断)
        /// </summary>
        /// <typeparam name="THaving"></typeparam>
        /// <param name="where"></param>
        /// <param name="trueHavingExp"></param>
        /// <param name="falseHavingExp"></param>
        /// <returns></returns>
        public QuerySet<T> HavingIf<THaving>(bool where, Expression<Func<THaving, object>> trueHavingExp, Expression<Func<THaving, object>> falseHavingExp)
        {
            if (where)
                HavingExpressionList.Add(trueHavingExp);
            else
                HavingExpressionList.Add(falseHavingExp);
            return this;
        }

        /// <summary>
        /// 是否去重
        /// </summary>
        /// <returns></returns>
        public QuerySet<T> Distinct()
        {
            IsDistinct = true;
            return this;
        }
        #endregion
    }
}
