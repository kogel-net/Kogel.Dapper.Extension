using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dapper;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
	public interface IQuerySet<T> : IAggregation<T>, IOrder<T>, IQuery<T>
	{
		/// <summary>
		/// 查询条件
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		QuerySet<T> Where(Expression<Func<T, bool>> predicate);

		/// <summary>
		/// 查询条件
		/// </summary>
		/// <typeparam name="TWhere"></typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		QuerySet<T> Where<TWhere>(Expression<Func<TWhere, bool>> predicate);

		/// <summary>
		/// 查询条件
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		QuerySet<T> Where(T model);

		/// <summary>
		/// 查询条件
		/// </summary>
		/// <param name="dynamicTree"></param>
		/// <returns></returns>
		QuerySet<T> Where(Dictionary<string, DynamicTree> dynamicTree);

		/// <summary>
		/// 查询条件
		/// </summary>
		/// <param name="sqlWhere"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		QuerySet<T> Where(string sqlWhere, object param = null);

		/// <summary>
		/// 带前置条件的Where判断
		/// </summary>
		/// <typeparam name="TWhere"></typeparam>
		/// <param name="where"></param>
		/// <param name="truePredicate"></param>
		/// <param name="falsePredicate"></param>
		/// <returns></returns>
		QuerySet<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate);

		/// <summary>
		/// 带前置条件的Where判断
		/// </summary>
		/// <typeparam name="TWhere"></typeparam>
		/// <param name="where"></param>
		/// <param name="truePredicate"></param>
		/// <param name="falsePredicate"></param>
		/// <returns></returns>
		QuerySet<T> WhereIf<TWhere>(bool where, Expression<Func<TWhere, bool>> truePredicate, Expression<Func<TWhere, bool>> falsePredicate);

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
		QuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, object>> rightField, Expression<Func<TInner, object>> leftField, JoinMode joinMode = JoinMode.LEFT);

		/// <summary>
		/// 连表查询(任意查)
		/// </summary>
		/// <typeparam name="TOuter">主表</typeparam>
		/// <typeparam name="TInner">副表</typeparam>
		/// <param name="exp"></param>
		/// <param name="joinMode">连表方式</param>
		/// <param name="IsDisField">是否显示字段</param>
		/// <returns></returns>
		QuerySet<T> Join<TWhere, TInner>(Expression<Func<TWhere, TInner, bool>> exp, JoinMode joinMode = JoinMode.LEFT, bool IsDisField = true);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TWhere"></typeparam>
		/// <typeparam name="TInner"></typeparam>
		/// <param name="expression"></param>
		/// <param name="joinMode"></param>
		/// <param name="isDisField"></param>
		/// <returns></returns>
		QuerySet<T> Join<TWhere, TInner, TWhere2>(Expression<Func<TWhere, TInner, TWhere2, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true);

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
		QuerySet<T> Join<TWhere, TInner, TWhere2, TWhere3>(Expression<Func<TWhere, TInner, TWhere2, TWhere3, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true);

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

		/// <summary>
		/// 根据类型重名表名
		/// </summary>
		/// <param name="type"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		QuerySet<T> AsTableName(Type type, string tableName);

		/// <summary>
		/// 分组
		/// </summary>
		/// <param name="groupByExp"></param>
		/// <returns></returns>
		QuerySet<T> GroupBy(Expression<Func<T, object>> groupByExp);

		/// <summary>
		/// 分组(根据指定表)
		/// </summary>
		/// <typeparam name="TGroup"></typeparam>
		/// <param name="groupByExp"></param>
		/// <returns></returns>
		QuerySet<T> GroupBy<TGroup>(Expression<Func<TGroup, object>> groupByExp);

		/// <summary>
		/// 分组(带判断)
		/// </summary>
		/// <typeparam name="TGroup"></typeparam>
		/// <param name="where"></param>
		/// <param name="trueGroupByExp"></param>
		/// <param name="falseGroupByExp"></param>
		/// <returns></returns>
		QuerySet<T> GroupByIf<TGroup>(bool where, Expression<Func<TGroup, object>> trueGroupByExp, Expression<Func<TGroup, object>> falseGroupByExp);

		/// <summary>
		/// 分组聚合条件
		/// </summary>
		/// <param name="havingExp"></param>
		/// <returns></returns>
		QuerySet<T> Having(Expression<Func<T, object>> havingExp);

		/// <summary>
		/// 分组聚合条件(根据指定表)
		/// </summary>
		/// <typeparam name="THaving"></typeparam>
		/// <param name="havingExp"></param>
		/// <returns></returns>
		QuerySet<T> Having<THaving>(Expression<Func<THaving, object>> havingExp);

		/// <summary>
		/// 分组聚合条件(带判断)
		/// </summary>
		/// <typeparam name="THaving"></typeparam>
		/// <param name="where"></param>
		/// <param name="trueHavingExp"></param>
		/// <param name="falseHavingExp"></param>
		/// <returns></returns>
		QuerySet<T> HavingIf<THaving>(bool where, Expression<Func<THaving, object>> trueHavingExp, Expression<Func<THaving, object>> falseHavingExp);

		/// <summary>
		/// 是否去重
		/// </summary>
		/// <returns></returns>
		QuerySet<T> Distinct();
	}
}
