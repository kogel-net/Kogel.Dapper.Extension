using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dapper;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface IQuerySet<T>
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        QuerySet<T> Where(Expression<Func<T, bool>> predicate);
        QuerySet<T> Where<TWhere>(Expression<Func<TWhere, bool>> predicate);
        QuerySet<T> Where(T model);
        QuerySet<T> Where(Dictionary<string, DynamicTree> dynamicTree);
        QuerySet<T> Where(string sqlWhere, object param = null);
        /// <summary>
        /// 不锁表查询(此方法只支持Mssql)
        /// </summary>
        /// <returns></returns>
        QuerySet<T> WithNoLock();
        /// <summary>
        /// 连表查询
        /// </summary>
        /// <typeparam name="TOuter">主表</typeparam>
        /// <typeparam name="TInner">副表</typeparam>
        /// <param name="rightField"></param>
        /// <param name="leftField"></param>
        /// <returns></returns>
        QuerySet<T> Join<TOuter, TInner>(Expression<Func<TOuter, object>> rightField, Expression<Func<TInner, object>> leftField, JoinMode joinMode = JoinMode.LEFT);
        /// <summary>
        /// 连表查询(任意查)
        /// </summary>
        /// <typeparam name="TOuter">主表</typeparam>
        /// <typeparam name="TInner">副表</typeparam>
        /// <param name="exp"></param>
        /// <param name="joinMode">连表方式</param>
        /// <param name="IsDisField">是否显示字段</param>
        /// <returns></returns>
        QuerySet<T> Join<TOuter, TInner>(Expression<Func<TOuter, TInner, bool>> exp, JoinMode joinMode = JoinMode.LEFT, bool IsDisField = true);
        /// <summary>
        /// 连接(通过sql连接，不指定表实体默认为不增加该表显示字段)
        /// </summary>
        /// <param name="SqlJoin"></param>
        /// <returns></returns>
        QuerySet<T> Join(string SqlJoin);
        /// <summary>
        /// 连接(通过sql连接，不指定表实体默认为不增加该表显示字段)
        /// </summary>
        /// <typeparam name="TInner"></typeparam>
        /// <param name="sqlJoin"></param>
        /// <returns></returns>
        QuerySet<T> Join<TInner>(string SqlJoin);
        /// <summary>
        /// 字段匹配(适用于实体类字段和数据库字段不一致时,返回值为Dynamic类型时不适用)
        /// </summary>
        /// <returns></returns>
        QuerySet<T> FieldMatch<TSource>();

        #region 多表索引扩展


        #endregion
    }
}
