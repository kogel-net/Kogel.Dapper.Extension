using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Expressions;
using System;
using System.Linq.Expressions;
using System.Linq;
using Kogel.Dapper.Extension.Model;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Dapper;

namespace Kogel.Dapper.Extension.Extension.From
{
    /// <summary>
    /// 多表索引扩展
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ISelect<T>
    {
        private QuerySet<T> querySet { get; }
        public ISelect(QuerySet<T> querySet)
        {
            this.querySet = querySet;
        }
        public QuerySet<T> GetQuerySet()
        {
            return querySet;
        }
        public QuerySet<T> Where(LambdaExpression exp)
        {
            querySet.WhereExpressionList.Add(exp);
            return querySet;
        }
        public TReturn Get<TReturn>(LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatGet<T>();
            return querySet.DbCon.QueryFirst_1<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
        }
        public IEnumerable<TReturn> ToList<TReturn>(LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatToList<T>();
            return querySet.DbCon.Query_1<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
        }
        public ISelect<T> OrderBy<TProperty>(Expression<Func<TProperty, object>> field)
        {
            if (field != null)
                querySet.OrderbyExpressionList.Add(field, EOrderBy.Asc);
            return this;
        }
        public ISelect<T> OrderBy(string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
                querySet.OrderbyBuilder.Append(orderBy);
            return this;
        }
        public ISelect<T> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field)
        {
            if (field != null)
                querySet.OrderbyExpressionList.Add(field, EOrderBy.Desc);
            return this;
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            //查询总行数
            querySet.SqlProvider.FormatCount();
            var pageTotal = querySet.DbCon.QuerySingle<int>(querySet.SqlProvider.SqlString, querySet.SqlProvider.Params);
            //查询数据
            querySet.SqlProvider.Params.Clear();
            querySet.SqlProvider.ProviderOption.MappingList.Clear();
            querySet.SqlProvider.FormatToPageList<T>(pageIndex, pageSize);
            var itemList = querySet.DbCon.Query_1<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
            return new PageList<TReturn>(pageIndex, pageSize, pageTotal, itemList);
        }
    }
    public class ISelectFrom<T, T1, T2> : ISelect<T>
    {
        public ISelectFrom(QuerySet<T> querySet) : base(querySet)
        {

        }
        public ISelectFrom<T, T1, T2> Where(Expression<Func<T1, T2, bool>> exp)
        {
            base.Where(exp);
            return this;
        }
        public ISelectFrom<T, T1, T2> WhereIf(bool where, Expression<Func<T1, T2, bool>> trueExp, Expression<Func<T1, T2, bool>> falseExp)
        {
            if (where)
                base.Where(trueExp);
            else
                base.Where(falseExp);
            return this;
        }
        public ISelectFrom<T, T1, T2> Where(bool where, Expression<Func<T1, T2, bool>> trueExp, Expression<Func<T1, T2, bool>> falseExp)
        {
            if (where)
                base.Where(trueExp);
            else
                base.Where(falseExp);
            return this;
        }
        public TReturn Get<TReturn>(Expression<Func<T1, T2, TReturn>> select)
        {
            return base.Get<TReturn>(select);
        }
        public TReturn Get<TReturn>(bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect)
        {
            if (where)
                return base.Get<TReturn>(trueSelect);
            else
                return base.Get<TReturn>(falseSelect);
        }
        public List<TReturn> ToList<TReturn>(Expression<Func<T1, T2, TReturn>> select)
        {
            return base.ToList<TReturn>(select).ToList();
        }
        public List<TReturn> ToList<TReturn>(bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect)
        {
            if (where)
                return base.ToList<TReturn>(trueSelect).ToList();
            else
                return base.ToList<TReturn>(falseSelect).ToList();
        }
        public new ISelectFrom<T, T1, T2> OrderBy<TProperty>(Expression<Func<TProperty, object>> field)
        {
            base.OrderBy(field);
            return this;
        }
        public new ISelectFrom<T, T1, T2> OrderBy(string orderBy)
        {
            base.OrderBy(orderBy);
            return this;
        }
        public new ISelectFrom<T, T1, T2> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field)
        {
            base.OrderByDescing(field);
            return this;
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T1, T2, TReturn>> select)
        {
            return base.PageList<TReturn>(pageIndex, pageSize, select);
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect)
        {
            if (where)
                return base.PageList<TReturn>(pageIndex, pageSize, trueSelect);
            else
                return base.PageList<TReturn>(pageIndex, pageSize, falseSelect);
        }
    }
    public class ISelectFrom<T, T1, T2, T3> : ISelect<T>
    {
        public ISelectFrom(QuerySet<T> querySet) : base(querySet)
        {

        }
        public ISelectFrom<T, T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> exp)
        {
            base.Where(exp);
            return this;
        }
        public ISelectFrom<T, T1, T2, T3> WhereIf(bool where, Expression<Func<T1, T2, T3, bool>> trueExp, Expression<Func<T1, T2, T3, bool>> falseExp)
        {
            if (where)
                base.Where(trueExp);
            else
                base.Where(falseExp);
            return this;
        }
        public TReturn Get<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return base.Get<TReturn>(select);
        }
        public TReturn Get<TReturn>(bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect)
        {
            if (where)
                return base.Get<TReturn>(trueSelect);
            else
                return base.Get<TReturn>(falseSelect);
        }
        public List<TReturn> ToList<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return base.ToList<TReturn>(select).ToList();
        }
        public List<TReturn> ToList<TReturn>(bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect)
        {
            if (where)
                return base.ToList<TReturn>(trueSelect).ToList();
            else
                return base.ToList<TReturn>(falseSelect).ToList();
        }
        public new ISelectFrom<T, T1, T2, T3> OrderBy<TProperty>(Expression<Func<TProperty, object>> field)
        {
            base.OrderBy(field);
            return this;
        }
        public new ISelectFrom<T, T1, T2, T3> OrderBy(string orderBy)
        {
            base.OrderBy(orderBy);
            return this;
        }
        public new ISelectFrom<T, T1, T2, T3> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field)
        {
            base.OrderByDescing(field);
            return this;
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return base.PageList<TReturn>(pageIndex, pageSize, select);
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect)
        {
            if (where)
                return base.PageList<TReturn>(pageIndex, pageSize, trueSelect);
            else
                return base.PageList<TReturn>(pageIndex, pageSize, falseSelect);
        }
    }
    public class ISelectFrom<T, T1, T2, T3, T4> : ISelect<T>
    {
        public ISelectFrom(QuerySet<T> querySet) : base(querySet)
        {

        }
        public ISelectFrom<T, T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> exp)
        {
            base.Where(exp);
            return this;
        }
        public ISelectFrom<T, T1, T2, T3, T4> WhereIf(bool where, Expression<Func<T1, T2, T3, T4, bool>> trueExp, Expression<Func<T1, T2, T3, T4, bool>> falseExp)
        {
            if (where)
                base.Where(trueExp);
            else
                base.Where(falseExp);
            return this;
        }
        public TReturn Get<TReturn>(Expression<Func<T1, T2, T3, T4, TReturn>> select)
        {
            return base.Get<TReturn>(select);
        }
        public TReturn Get<TReturn>(bool where, Expression<Func<T1, T2, T3, T4, TReturn>> trueSelect, Expression<Func<T1, T2, T3, T4, TReturn>> falseSelect)
        {
            if (where)
                return base.Get<TReturn>(trueSelect);
            else
                return base.Get<TReturn>(falseSelect);
        }
        public List<TReturn> ToList<TReturn>(Expression<Func<T1, T2, T3, T4, TReturn>> select)
        {
            return base.ToList<TReturn>(select).ToList();
        }
        public List<TReturn> ToList<TReturn>(bool where, Expression<Func<T1, T2, T3, T4, TReturn>> trueSelect, Expression<Func<T1, T2, T3, T4, TReturn>> falseSelect)
        {
            if (where)
                return base.ToList<TReturn>(trueSelect).ToList();
            else
                return base.ToList<TReturn>(falseSelect).ToList();
        }
        public new ISelectFrom<T, T1, T2, T3, T4> OrderBy<TProperty>(Expression<Func<TProperty, object>> field)
        {
            base.OrderBy(field);
            return this;
        }
        public new ISelectFrom<T, T1, T2, T3, T4> OrderBy(string orderBy)
        {
            base.OrderBy(orderBy);
            return this;
        }
        public new ISelectFrom<T, T1, T2, T3, T4> OrderByDescing<TProperty>(Expression<Func<TProperty, object>> field)
        {
            base.OrderByDescing(field);
            return this;
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, Expression<Func<T1, T2, T3, T4, TReturn>> select)
        {
            return base.PageList<TReturn>(pageIndex, pageSize, select);
        }
        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T1, T2, T3, T4, TReturn>> trueSelect, Expression<Func<T1, T2, T3, T4, TReturn>> falseSelect)
        {
            if (where)
                return base.PageList<TReturn>(pageIndex, pageSize, trueSelect);
            else
                return base.PageList<TReturn>(pageIndex, pageSize, falseSelect);
        }
    }
}
