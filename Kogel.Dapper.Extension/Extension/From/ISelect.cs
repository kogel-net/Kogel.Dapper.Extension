using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Expressions;
using System;
using System.Linq.Expressions;
using System.Linq;
using Kogel.Dapper.Extension.Model;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Threading.Tasks;

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

        public TReturn Get<TReturn>(LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatGet<T>();
            return querySet.DbCon.QueryFirst_1<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
        }

        public async Task<TReturn> GetAsync<TReturn>(LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatGet<T>();
            return await querySet.DbCon.QueryFirst_1Async<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
        }

        public IEnumerable<TReturn> ToList<TReturn>(LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatToList<T>();
            return querySet.DbCon.Query_1<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
        }

        public async Task<IEnumerable<TReturn>> ToListAsync<TReturn>(LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatToList<T>();
            return await querySet.DbCon.Query_1Async<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
        }

        public DataSet ToDataSet<TReturn>(LambdaExpression exp, IDbDataAdapter dataAdapter = null)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatToList<T>();
            return querySet.DbCon.QueryDataSets(querySet.SqlProvider, querySet.DbTransaction, dataAdapter);
        }

        public async Task<DataSet> ToDataSetAsync<TReturn>(LambdaExpression exp, IDbDataAdapter dataAdapter = null)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            querySet.SqlProvider.FormatToList<T>();
            return await querySet.DbCon.QueryDataSetsAsync(querySet.SqlProvider, querySet.DbTransaction, dataAdapter);
        }

        public PageList<TReturn> PageList<TReturn>(int pageIndex, int pageSize, LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            //查询总行数
            querySet.SqlProvider.FormatCount();
            var pageTotal = querySet.DbCon.QuerySingle<int>(querySet.SqlProvider.SqlString, querySet.SqlProvider.Params);
            //查询数据
            List<TReturn> itemList;
            querySet.SqlProvider.Clear();
            if (pageTotal != 0)
            {
                querySet.SqlProvider.FormatToPageList<T>(pageIndex, pageSize);
                itemList = querySet.DbCon.Query_1<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
            }
            else
            {
                itemList = new List<TReturn>();
            }
            return new PageList<TReturn>(pageIndex, pageSize, pageTotal, itemList);
        }

        public async Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, LambdaExpression exp)
        {
            querySet.SqlProvider.Context.Set.SelectExpression = exp;
            //查询总行数
            querySet.SqlProvider.FormatCount();
            var pageTotal = await querySet.DbCon.QuerySingleAsync<int>(querySet.SqlProvider.SqlString, querySet.SqlProvider.Params);
            //查询数据
            List<TReturn> itemList;
            querySet.SqlProvider.Clear();
            if (pageTotal != 0)
            {
                querySet.SqlProvider.FormatToPageList<T>(pageIndex, pageSize);
                itemList = await querySet.DbCon.Query_1Async<TReturn>(querySet.SqlProvider, querySet.DbTransaction);
            }
            else
            {
                itemList = new List<TReturn>();
            }
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

        public Task<TReturn> GetAsync<TReturn>(Expression<Func<T1, T2, TReturn>> select)
        {
            return base.GetAsync<TReturn>(select);
        }

        public Task<TReturn> GetAsync<TReturn>(bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect)
        {
            if (where)
                return base.GetAsync<TReturn>(trueSelect);
            else
                return base.GetAsync<TReturn>(falseSelect);
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

        public async Task<List<TReturn>> ToListAsync<TReturn>(Expression<Func<T1, T2, TReturn>> select)
        {
            return (await base.ToListAsync<TReturn>(select)).ToList();
        }

        public async Task<List<TReturn>> ToListAsync<TReturn>(bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect)
        {
            if (where)
                return (await base.ToListAsync<TReturn>(trueSelect)).ToList();
            else
                return (await base.ToListAsync<TReturn>(falseSelect)).ToList();
        }

        public DataSet ToDataSet<TReturn>(Expression<Func<T1, T2, TReturn>> select, IDbDataAdapter dataAdapter = null)
        {
            return base.ToDataSet<TReturn>(select, dataAdapter);
        }

        public DataSet ToDataSet<TReturn>(bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null)
        {
            if (where)
                return base.ToDataSet<TReturn>(trueSelect, dataAdapter);
            else
                return base.ToDataSet<TReturn>(falseSelect, dataAdapter);
        }

        public Task<DataSet> ToDataSetAsync<TReturn>(Expression<Func<T1, T2, TReturn>> select, IDbDataAdapter dataAdapter = null)
        {
            return base.ToDataSetAsync<TReturn>(select, dataAdapter);
        }

        public Task<DataSet> ToDataSetAsync<TReturn>(bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null)
        {
            if (where)
                return base.ToDataSetAsync<TReturn>(trueSelect, dataAdapter);
            else
                return base.ToDataSetAsync<TReturn>(falseSelect, dataAdapter);
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

        public Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, Expression<Func<T1, T2, TReturn>> select)
        {
            return base.PageListAsync<TReturn>(pageIndex, pageSize, select);
        }

        public Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T1, T2, TReturn>> trueSelect, Expression<Func<T1, T2, TReturn>> falseSelect)
        {
            if (where)
                return base.PageListAsync<TReturn>(pageIndex, pageSize, trueSelect);
            else
                return base.PageListAsync<TReturn>(pageIndex, pageSize, falseSelect);
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

        public Task<TReturn> GetAsync<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return base.GetAsync<TReturn>(select);
        }

        public Task<TReturn> GetAsync<TReturn>(bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect)
        {
            if (where)
                return base.GetAsync<TReturn>(trueSelect);
            else
                return base.GetAsync<TReturn>(falseSelect);
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

        public async Task<List<TReturn>> ToListAsync<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return (await base.ToListAsync<TReturn>(select)).ToList();
        }

        public async Task<List<TReturn>> ToListAsync<TReturn>(bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect)
        {
            if (where)
                return (await base.ToListAsync<TReturn>(trueSelect)).ToList();
            else
                return (await base.ToListAsync<TReturn>(falseSelect)).ToList();
        }

        public DataSet ToDataSet<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select, IDbDataAdapter dataAdapter = null)
        {
            return base.ToDataSet<TReturn>(select, dataAdapter);
        }

        public DataSet ToDataSet<TReturn>(bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null)
        {
            if (where)
                return base.ToDataSet<TReturn>(trueSelect, dataAdapter);
            else
                return base.ToDataSet<TReturn>(falseSelect, dataAdapter);
        }

        public Task<DataSet> ToDataSetAsync<TReturn>(Expression<Func<T1, T2, T3, TReturn>> select, IDbDataAdapter dataAdapter = null)
        {
            return base.ToDataSetAsync<TReturn>(select, dataAdapter);
        }

        public Task<DataSet> ToDataSetAsync<TReturn>(bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null)
        {
            if (where)
                return base.ToDataSetAsync<TReturn>(trueSelect, dataAdapter);
            else
                return base.ToDataSetAsync<TReturn>(falseSelect, dataAdapter);
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

        public Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, Expression<Func<T1, T2, T3, TReturn>> select)
        {
            return base.PageListAsync<TReturn>(pageIndex, pageSize, select);
        }

        public Task<PageList<TReturn>> PageListAsync<TReturn>(int pageIndex, int pageSize, bool where, Expression<Func<T1, T2, T3, TReturn>> trueSelect, Expression<Func<T1, T2, T3, TReturn>> falseSelect)
        {
            if (where)
                return base.PageListAsync<TReturn>(pageIndex, pageSize, trueSelect);
            else
                return base.PageListAsync<TReturn>(pageIndex, pageSize, falseSelect);
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
        public DataSet ToDataSet<TReturn>(Expression<Func<T1, T2, T3, T4, TReturn>> select, IDbDataAdapter dataAdapter = null)
        {
            return base.ToDataSet<TReturn>(select, dataAdapter);
        }
        public DataSet ToDataSet<TReturn>(bool where, Expression<Func<T1, T2, T3, T4, TReturn>> trueSelect, Expression<Func<T1, T2, T3, T4, TReturn>> falseSelect, IDbDataAdapter dataAdapter = null)
        {
            if (where)
                return base.ToDataSet<TReturn>(trueSelect, dataAdapter);
            else
                return base.ToDataSet<TReturn>(falseSelect, dataAdapter);
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
