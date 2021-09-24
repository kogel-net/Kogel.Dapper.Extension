using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Extension.From;
using Kogel.Dapper.Extension.Entites;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public partial interface IQuerySet<T> : IAggregation<T>, IQuery<T>
    {
        /// <summary>
        /// 根据类型重命名表名
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IQuerySet<T> ResetTableName(Type type, string tableName);

        /// <summary>
        /// 不锁表查询(此方法只支持Mssql)
        /// </summary>
        /// <returns></returns>
        IQuerySet<T> WithNoLock();

        /// <summary>
        /// 字段匹配(适用于实体类字段和数据库字段不一致时,返回值为Dynamic类型时不适用)
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        IQuerySet<T> FieldMatch<TSource>();

        /// <summary>
        /// 返回对应行数数据
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        IQuerySet<T> Top(int num);

        /// <summary>
        /// 去重
        /// </summary>
        /// <returns></returns>
        IQuerySet<T> Distinct();

        /// <summary>
        /// 连表查询
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="rightField"></param>
        /// <param name="leftField"></param>
        /// <param name="joinMode"></param>
        /// <returns></returns>
        IQuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, object>> rightField, Expression<Func<TInner, object>> leftField, JoinMode joinMode = JoinMode.LEFT);

        /// <summary>
        /// 连表查询(任意查)
        /// </summary>
        /// <typeparam name="TWhere">主表</typeparam>
        /// <typeparam name="TInner">副表</typeparam>
        /// <param name="exp"></param>
        /// <param name="joinMode">连表方式</param>
        /// <param name="IsDisField">是否显示字段</param>
        /// <returns></returns>
        IQuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, TInner, bool>> exp, JoinMode joinMode = JoinMode.LEFT, bool IsDisField = true);

        /// <summary>
        /// 连表查询
        /// </summary>
        /// <typeparam name="TWhere"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="expression"></param>
        /// <param name="joinMode"></param>
        /// <param name="isDisField"></param>
        /// <returns></returns>
        IQuerySet<T> Join<TWhere, TInner, TWhere2>(Expression<Func<TWhere, TInner, TWhere2, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true);

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
        IQuerySet<T> Join<TWhere, TInner, TWhere2, TWhere3>(Expression<Func<TWhere, TInner, TWhere2, TWhere3, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true);

        /// <summary>
        /// 连表查询(通过sql连接，不指定表实体不增加该表显示字段)
        /// </summary>
        /// <param name="SqlJoin"></param>
        /// <returns></returns>
        IQuerySet<T> Join(string sqlJoin);

        /// <summary>
        /// 连接(通过sql连接，指定表实体增加该表显示字段)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="sqlJoin"></param>
        /// <returns></returns>
        IQuerySet<T> Join<TInner>(string sqlJoin);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="groupByExp"></param>
        /// <returns></returns>
        IQuerySet<T> GroupBy(Expression<Func<T, object>> groupByExp);

        /// <summary>
        /// 分组(根据指定表)
        /// </summary>
        /// <typeparam name="TGroup"></typeparam>
        /// <param name="groupByExp"></param>
        /// <returns></returns>
        IQuerySet<T> GroupBy<TGroup>(Expression<Func<TGroup, object>> groupByExp);

        /// <summary>
        /// 分组(带判断)
        /// </summary>
        /// <typeparam name="TGroup"></typeparam>
        /// <param name="where"></param>
        /// <param name="trueGroupByExp"></param>
        /// <param name="falseGroupByExp"></param>
        /// <returns></returns>
        IQuerySet<T> GroupByIf<TGroup>(bool where, Expression<Func<TGroup, object>> trueGroupByExp, Expression<Func<TGroup, object>> falseGroupByExp);

        /// <summary>
        /// 分组聚合条件
        /// </summary>
        /// <param name="havingExp"></param>
        /// <returns></returns>
        IQuerySet<T> Having(Expression<Func<T, object>> havingExp);

        /// <summary>
        /// 分组聚合条件(根据指定表)
        /// </summary>
        /// <typeparam name="THaving"></typeparam>
        /// <param name="havingExp"></param>
        /// <returns></returns>
        IQuerySet<T> Having<THaving>(Expression<Func<THaving, object>> havingExp);

        /// <summary>
        /// 分组聚合条件(带判断)
        /// </summary>
        /// <typeparam name="THaving"></typeparam>
        /// <param name="where"></param>
        /// <param name="trueHavingExp"></param>
        /// <param name="falseHavingExp"></param>
        /// <returns></returns>
        IQuerySet<T> HavingIf<THaving>(bool where, Expression<Func<THaving, object>> trueHavingExp, Expression<Func<THaving, object>> falseHavingExp);

    }
}
