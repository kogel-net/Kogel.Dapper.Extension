using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Entites;
using System.Linq;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension;
using System.Text;
using static Kogel.Dapper.Extension.SqlMapper;
using Kogel.Dapper.Extension.Expressions;
using System.Text.RegularExpressions;
using Kogel.Dapper.Extension.Extension.From;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 查询集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QuerySet<T> : Aggregation<T>, IQuerySet<T>
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
            OrderbyExpressionList = new Dictionary<LambdaExpression, EOrderBy>();
            OrderbyBuilder = new StringBuilder();
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
            OrderbyExpressionList = new Dictionary<LambdaExpression, EOrderBy>();
            OrderbyBuilder = new StringBuilder();
        }

        #region 基础函数
        public IQuerySet<T> ResetTableName(Type type, string tableName)
        {
            SqlProvider.AsTableNameDic.Add(type, tableName);
            return this;
        }

        /// <summary>
        /// 不锁表查询(此方法只支持Mssql)
        /// </summary>
        /// <returns></returns>
        public IQuerySet<T> WithNoLock()
        {
            NoLock = true;
            return this;
        }

        /// <summary>
        /// 字段匹配[已弃用]
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public IQuerySet<T> FieldMatch<TSource>()
        {
            return this;
        }

        /// <summary>
        /// 返回对应行数数据
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQuerySet<T> Top(int num)
        {
            TopNum = num;
            return this;
        }

        /// <summary>
        /// 是否去重
        /// </summary>
        /// <returns></returns>
        public IQuerySet<T> Distinct()
        {
            IsDistinct = true;
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
        public IQuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, object>> rightField, Expression<Func<TInner, object>> leftField, JoinMode joinMode = JoinMode.LEFT)
        {
            var tWhere = EntityCache.QueryEntity(typeof(TWhere));
            var tInner = EntityCache.QueryEntity(typeof(TInner));
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Default,
                JoinMode = joinMode,
                RightTabName = tWhere.AsName,
                RightAssName = tWhere.FieldPairs[rightField.GetCorrectPropertyName()],
                LeftTabName = tInner.AsName,
                LeftAssName = tInner.FieldPairs[leftField.GetCorrectPropertyName()],
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
        public IQuerySet<T> Join<TInner>(LambdaExpression expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            var joinWhere = new WhereExpression(expression, $"{Params.ParameterNames.Count()}", SqlProvider);
            Regex whereRex = new Regex("AND");
            string tableName = SqlProvider.FormatTableName(false, true, typeof(TInner));
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Sql,
                JoinSql = $"{joinMode} JOIN {tableName} ON {  whereRex.Replace(joinWhere.SqlCmd, "", 1)}",
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
        public IQuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, TInner, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
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
        public IQuerySet<T> Join<TWhere, TInner, TWhere2>(Expression<Func<TWhere, TInner, TWhere2, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
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
        public IQuerySet<T> Join<TWhere, TInner, TWhere2, TWhere3>(Expression<Func<TWhere, TInner, TWhere2, TWhere3, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            Join<TInner>(expression, joinMode, isDisField);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlJoin"></param>
        /// <returns></returns>
        public IQuerySet<T> Join(string sqlJoin)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Sql,
                JoinSql = sqlJoin,
                IsMapperField = false
            });
            return this;
        }

        /// <summary>
        /// 连接(通过sql连接，不指定表实体默认为不增加该表显示字段)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="sqlJoin"></param>
        /// <returns></returns>
        public IQuerySet<T> Join<TInner>(string sqlJoin)
        {
            SqlProvider.JoinList.Add(new JoinAssTable()
            {
                Action = JoinAction.Sql,
                JoinSql = sqlJoin,
                TableType = typeof(TInner),
            });
            return this;
        }
        #endregion

        #region 分组
        public IQuerySet<T> GroupBy(Expression<Func<T, object>> groupByExp)
        {
            GroupExpressionList.Add(groupByExp);
            return this;
        }

        public IQuerySet<T> GroupBy<TGroup>(Expression<Func<TGroup, object>> groupByExp)
        {
            GroupExpressionList.Add(groupByExp);
            return this;
        }
        public IQuerySet<T> GroupByIf<TGroup>(bool where, Expression<Func<TGroup, object>> trueGroupByExp, Expression<Func<TGroup, object>> falseGroupByExp)
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
        public IQuerySet<T> Having(Expression<Func<T, object>> havingExp)
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
        public IQuerySet<T> Having<THaving>(Expression<Func<THaving, object>> havingExp)
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
        public IQuerySet<T> HavingIf<THaving>(bool where, Expression<Func<THaving, object>> trueHavingExp, Expression<Func<THaving, object>> falseHavingExp)
        {
            if (where)
                HavingExpressionList.Add(trueHavingExp);
            else
                HavingExpressionList.Add(falseHavingExp);
            return this;
        }

        #endregion
    }
}
