using System;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.MsSql.Extension;
using System.Text.RegularExpressions;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Dapper.Extension.MsSql
{
    public class MsSqlProvider : SqlProvider
    {
        private const string OpenQuote = "[";
        private const string CloseQuote = "]";
        private const char ParameterPrefix = '@';
        private IResolveExpression ResolveExpression;
        public MsSqlProvider()
        {
            ProviderOption = new ProviderOption(OpenQuote, CloseQuote, ParameterPrefix);
            ResolveExpression = new ResolveExpression(ProviderOption);
        }

        public sealed override IProviderOption ProviderOption { get; set; }

        public override SqlProvider FormatGet<T>()
        {
            var selectSql = ResolveExpression.ResolveSelect(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression, 1,Params);

            var fromTableSql = FormatTableName();

            var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql, Context.Set.SelectExpression);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params);

            var orderbySql = ResolveExpression.ResolveOrderBy(Context.Set);

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} {orderbySql}";

            return this;
        }

        public override SqlProvider FormatToList<T>()
        {
            var topNum = DataBaseContext<T>().QuerySet.TopNum;

            var selectSql = ResolveExpression.ResolveSelect(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression, topNum, Params);

            var fromTableSql = FormatTableName();

            var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql,Context.Set.SelectExpression);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params);

            var orderbySql = ResolveExpression.ResolveOrderBy(Context.Set);

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} {orderbySql}";

            return this;
        }

        public override SqlProvider FormatToPageList<T>(int pageIndex, int pageSize, bool IsSelectCount = true)
        {
            var orderbySql = ResolveExpression.ResolveOrderBy(Context.Set);
            if (string.IsNullOrEmpty(orderbySql))
                throw new DapperExtensionException("order by takes precedence over pagelist");

            var selectSql = ResolveExpression.ResolveSelect(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression, null, Params);

            var fromTableSql = FormatTableName();

            var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql, Context.Set.SelectExpression);

            var whereSql = string.Empty;
            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params);
            if (IsSelectCount)
                SqlString = $"SELECT COUNT(1) {fromTableSql} {nolockSql} {joinSql} {whereSql};" + Environment.NewLine;
            else
                SqlString = "";
            SqlString += $@"SELECT T.* FROM    ( 
                            SELECT ROW_NUMBER() OVER ( {orderbySql} ) AS ROWNUMBER,
                            {(new Regex("SELECT").Replace(selectSql, "", 1))}
                            {fromTableSql} {nolockSql}{joinSql}
                            {whereSql}
                            ) T
                            WHERE ROWNUMBER BETWEEN {((pageIndex - 1) * pageSize) + 1} AND {pageIndex * pageSize};";

            return this;
        }

        public override SqlProvider FormatCount()
        {
            var selectSql = "SELECT COUNT(1)";

            var fromTableSql = FormatTableName();

            var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

            string noneSql = "";
            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql, Context.Set.SelectExpression);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params);

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} ";

            return this;
        }

        public override SqlProvider FormatDelete()
        {
            var fromTableSql = FormatTableName(false, false);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params, null, false);

            SqlString = $"DELETE {fromTableSql} {whereSql }";

            return this;
        }

        public override SqlProvider FormatInsert<T>(T entity)
        {
            var paramsAndValuesSql = FormatInsertParamsAndValues(entity);
            SqlString = $"INSERT INTO {FormatTableName(false, false)} ({paramsAndValuesSql[0]}) VALUES({paramsAndValuesSql[1]})";
            return this;
        }
        public override SqlProvider FormatInsertIdentity<T>(T entity)
        {
            var paramsAndValuesSql = FormatInsertParamsAndValues(entity);
            SqlString = $"INSERT INTO {FormatTableName(false, false)} ({paramsAndValuesSql[0]}) VALUES({paramsAndValuesSql[1]}) SELECT @@IDENTITY";
            return this;
        }

        public override SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            var update = ResolveExpression.ResolveUpdate(updateExpression);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params, null, false);
            Params.AddDynamicParams(update.Param);

            SqlString = $"UPDATE {FormatTableName(false, false)} {update.SqlCmd} {whereSql}";

            return this;
        }

        public override SqlProvider FormatUpdate<T>(T entity)
        {
            var update = ResolveExpression.ResolveUpdates<T>(entity, Params);
            var whereSql = string.Empty;
            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params, null, false);

            SqlString = $"UPDATE {FormatTableName(false, false)} {update} {whereSql}";
            return this;
        }

        public override SqlProvider FormatSum(LambdaExpression sumExpression)
        {
            var selectSql = ResolveExpression.ResolveSum(sumExpression);

            var fromTableSql = FormatTableName();

            var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

            string noneSql = "";
            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql, Context.Set.SelectExpression);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params);

            SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} ";

            return this;
        }

        public override SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator)
        {
            var update = ResolveExpression.ResolveUpdate(updator);

            var selectSql = ResolveExpression.ResolveSelectOfUpdate(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression);

            var whereSql = string.Empty;

            //表查询条件
            var whereParamsList = ResolveExpression.ResolveWhereList(Context.Set, ref whereSql, Params);
            Params.AddDynamicParams(update.Param);

            SqlString = $"UPDATE {FormatTableName(false, false)} {update.SqlCmd} {selectSql} {whereSql}";

            return this;
        }
    }
}
