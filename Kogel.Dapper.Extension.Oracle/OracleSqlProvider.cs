using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Oracle.Extension;

namespace Kogel.Dapper.Extension.Oracle
{
    public class OracleSqlProvider : SqlProvider
    {
        private readonly static string OpenQuote = "\"";
        private readonly static string CloseQuote = "\"";
        private readonly static char ParameterPrefix = ':';
        private IResolveExpression ResolveExpression;
        public OracleSqlProvider()
        {
            ProviderOption = new ProviderOption(OpenQuote, CloseQuote, ParameterPrefix);
            ResolveExpression = new ResolveExpression(this);
        }

        public sealed override IProviderOption ProviderOption { get; set; }

        public override SqlProvider FormatGet<T>()
        {
            var selectSql = ResolveExpression.ResolveSelect(null);

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql);

            var groupSql = ResolveExpression.ResolveGroupBy();

            var havingSql = ResolveExpression.ResolveHaving();

            var orderbySql = ResolveExpression.ResolveOrderBy();

            SqlString = $@"SELECT T.* FROM( 
                            {selectSql}
                            {fromTableSql} {joinSql}
                            {whereSql}
                            {groupSql}
                            {havingSql}
                            {orderbySql}
                            ) T
                            WHERE ROWNUM<=1";

            return this;
        }

        public override SqlProvider FormatToList<T>()
        {
            var selectSql = ResolveExpression.ResolveSelect(null);

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql);

            var groupSql = ResolveExpression.ResolveGroupBy();

            var havingSql = ResolveExpression.ResolveHaving();

            var orderbySql = ResolveExpression.ResolveOrderBy();

            SqlString = $"{selectSql} {fromTableSql} {joinSql} {whereSql} {groupSql} {havingSql} {orderbySql}";

            return this;
        }

        public override SqlProvider FormatToPageList<T>(int pageIndex, int pageSize)
        {
            var orderbySql = ResolveExpression.ResolveOrderBy();

            var selectSql = ResolveExpression.ResolveSelect(null);

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql);

            var groupSql = ResolveExpression.ResolveGroupBy();

            var havingSql = ResolveExpression.ResolveHaving();

            SqlString = $@" SELECT T2.* FROM(
                            SELECT T.*,ROWNUM ROWNUMS FROM (
                            {selectSql}
                            {fromTableSql} {joinSql} {whereSql} {groupSql} {havingSql} {orderbySql}
                            ) T 
                            )T2
                            WHERE ROWNUMS BETWEEN {((pageIndex - 1) * pageSize) + 1} and {pageIndex * pageSize}";
            return this;
        }

        public override SqlProvider FormatCount()
        {
            var selectSql = "SELECT COUNT(1)";

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            string noneSql = string.Empty;

            //连表字符
            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql);

            if (!Context.Set.IsDistinct)
                SqlString = $"{selectSql} {fromTableSql} {joinSql} {whereSql} ";
            else
            {
                //字段解析字符
                string countBySql = ResolveExpression.ResolveSelect(null);

                SqlString = $@"SELECT COUNT(*) FROM(
                                {countBySql} {fromTableSql}
                                {joinSql}
                                {whereSql}
                                 )T";
            }

            return this;
        }

        public override SqlProvider FormatDelete()
        {
            var fromTableSql = FormatTableName(false, false);

            ProviderOption.IsAsName = false;

            var whereSql = ResolveExpression.ResolveWhereList();
            SqlString = $"DELETE {fromTableSql} {whereSql}";
            return this;
        }

        public override SqlProvider FormatInsert<T>(T entity, string[] excludeFields)
        {
            var fromTableSql = FormatTableName(false, false);
            var paramsAndValuesSql = FormatInsertParamsAndValues(entity);
            SqlString = $"INSERT INTO {fromTableSql} ({paramsAndValuesSql[0]}) VALUES({paramsAndValuesSql[1]})";
            return this;
        }

        /// <summary>
        /// oracle没有新增返回自增id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="excludeFields"></param>
        /// <returns></returns>
        public override SqlProvider FormatInsertIdentity<T>(T entity, string[] excludeFields)
        {
            //         var fromTableSql = ProviderOption.CombineFieldName(FormatTableName(false, false).Trim());
            //         var paramsAndValuesSql = FormatInsertParamsAndValues(entity);
            ////获取主键
            //var typeEntity = EntityCache.QueryEntity(typeof(T));
            //var field = typeEntity.EntityFieldList.Find(x => x.IsIdentity);
            ////获取序列名称
            //string sequenceName = ($"{typeEntity.Name}_{field.FieldName}_SEQ").ToUpper();

            //SqlString = $"INSERT INTO {fromTableSql} ({paramsAndValuesSql[0]}) VALUES({paramsAndValuesSql[1]});SELECT {sequenceName}.currval FROM DUAL";

            FormatInsert(entity, excludeFields);

            return this;
        }

        public override SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression)
        {
            var update = ResolveExpression.ResolveUpdate(updateExpression);

            var fromTableSql = FormatTableName(false, false);

            ProviderOption.IsAsName = false;

            var whereSql = ResolveExpression.ResolveWhereList();
            Params.AddDynamicParams(update.Param);

            SqlString = $"UPDATE {fromTableSql} {update.SqlCmd} {whereSql}";

            return this;
        }

        public override SqlProvider FormatUpdate<T>(T entity, string[] excludeFields, bool isBatch = false)
        {
            var update = ResolveExpression.ResolveUpdates<T>(entity, Params, excludeFields);
            var fromTableSql = FormatTableName(false, false);

            ProviderOption.IsAsName = false;

            var whereSql = ResolveExpression.ResolveWhereList();
            //如果不存在条件，就用主键作为条件
            if (!isBatch)
                if (whereSql.Trim().Equals("WHERE 1=1"))
                    whereSql += GetIdentityWhere(entity, Params);

            SqlString = $"UPDATE {fromTableSql} {update} {whereSql}";
            return this;
        }

        public override SqlProvider FormatSum(LambdaExpression sumExpression)
        {
            var selectSql = ResolveExpression.ResolveSum(sumExpression);

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            string noneSql = "";
            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql);

            SqlString = $"{selectSql} {fromTableSql}{joinSql} {whereSql} ";

            return this;
        }

        public override SqlProvider FormatMin(LambdaExpression minExpression)
        {
            var selectSql = ResolveExpression.ResolveMin(minExpression);

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            string noneSql = "";
            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql);

            SqlString = $"{selectSql} {fromTableSql}{joinSql} {whereSql} ";

            return this;
        }

        public override SqlProvider FormatMax(LambdaExpression maxExpression)
        {
            var selectSql = ResolveExpression.ResolveMax(maxExpression);

            var fromTableSql = FormatTableName();

            var whereSql = ResolveExpression.ResolveWhereList();

            string noneSql = "";
            var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql);

            SqlString = $"{selectSql} {fromTableSql}{joinSql} {whereSql} ";

            return this;
        }


        public override SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator)
        {
            var update = ResolveExpression.ResolveUpdate(updator);

            var fromTableSql = FormatTableName(false, false);
            var selectSql = ResolveExpression.ResolveSelectOfUpdate(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression);

            ProviderOption.IsAsName = false;

            var whereSql = ResolveExpression.ResolveWhereList();
            Params.AddDynamicParams(update.Param);

            SqlString = $"UPDATE {fromTableSql} {update.SqlCmd} {selectSql} {whereSql}";

            return this;
        }

        public override SqlProvider CreateNew()
        {
            return new OracleSqlProvider();
        }
    }
}
