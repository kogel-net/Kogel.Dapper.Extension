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
			ResolveExpression = new ResolveExpression(this);
		}

		public sealed override IProviderOption ProviderOption { get; set; }

		public override SqlProvider FormatGet<T>()
		{
			var selectSql = ResolveExpression.ResolveSelect(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression, 1, Params);

			var fromTableSql = FormatTableName();

			var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

			var whereSql = ResolveExpression.ResolveWhereList();

			var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql);

			var groupSql = ResolveExpression.ResolveGroupBy();

			var havingSql = ResolveExpression.ResolveHaving();

			var orderbySql = ResolveExpression.ResolveOrderBy();

			SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} {groupSql} {havingSql} {orderbySql}";

			return this;
		}

		public override SqlProvider FormatToList<T>()
		{
			var topNum = DataBaseContext<T>().QuerySet.TopNum;

			var selectSql = ResolveExpression.ResolveSelect(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression, topNum, Params);

			var fromTableSql = FormatTableName();

			var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

			var whereSql = ResolveExpression.ResolveWhereList();

			var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql);

			var groupSql = ResolveExpression.ResolveGroupBy();

			var havingSql = ResolveExpression.ResolveHaving();

			var orderbySql = ResolveExpression.ResolveOrderBy();

			SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} {groupSql} {havingSql} {orderbySql}";

			return this;
		}

		public override SqlProvider FormatToPageList<T>(int pageIndex, int pageSize)
		{
			var orderbySql = ResolveExpression.ResolveOrderBy();
			if (string.IsNullOrEmpty(orderbySql))
				throw new DapperExtensionException("order by takes precedence over pagelist");

			var selectSql = ResolveExpression.ResolveSelect(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression, null, Params);

			var fromTableSql = FormatTableName();

			var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

			var whereSql = ResolveExpression.ResolveWhereList();

			var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref selectSql);

			var groupSql = ResolveExpression.ResolveGroupBy();

			var havingSql = ResolveExpression.ResolveHaving();

			SqlString = $@"SELECT T.* FROM    ( 
                            SELECT ROW_NUMBER() OVER ( {orderbySql} ) AS ROWNUMBER,
                            {(new Regex("SELECT").Replace(selectSql, "", 1))}
                            {fromTableSql} {nolockSql}{joinSql}
                            {whereSql}
                            {groupSql}
                            {havingSql}
                            ) T
                            WHERE ROWNUMBER BETWEEN {((pageIndex - 1) * pageSize) + 1} AND {pageIndex * pageSize};";

			return this;
		}

		public override SqlProvider FormatCount()
		{
			var selectSql = "SELECT COUNT(1)";

			var fromTableSql = FormatTableName();

			var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

			var whereSql = ResolveExpression.ResolveWhereList();

			string noneSql = "";
			var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql);

			SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} ";

			return this;
		}

		public override SqlProvider FormatDelete()
		{
			var fromTableSql = FormatTableName(false, false);

			ProviderOption.IsAsName = false;

			var whereSql = ResolveExpression.ResolveWhereList();

			SqlString = $"DELETE {fromTableSql} {whereSql }";

			return this;
		}

		public override SqlProvider FormatInsert<T>(T entity, string[] excludeFields)
		{
			var paramsAndValuesSql = FormatInsertParamsAndValues(entity, excludeFields);
			SqlString = $"INSERT INTO {FormatTableName(false, false)} ({paramsAndValuesSql[0]}) VALUES({paramsAndValuesSql[1]})";
			return this;
		}
		public override SqlProvider FormatInsertIdentity<T>(T entity, string[] excludeFields)
		{
			var paramsAndValuesSql = FormatInsertParamsAndValues(entity, excludeFields);
			SqlString = $"INSERT INTO {FormatTableName(false, false)} ({paramsAndValuesSql[0]}) VALUES({paramsAndValuesSql[1]}) SELECT @@IDENTITY";
			return this;
		}

		public override SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression)
		{
			var update = ResolveExpression.ResolveUpdate(updateExpression);

			ProviderOption.IsAsName = false;

			var whereSql = ResolveExpression.ResolveWhereList();
			Params.AddDynamicParams(update.Param);

			SqlString = $"UPDATE {FormatTableName(false, false)} {update.SqlCmd} {whereSql}";

			return this;
		}

		public override SqlProvider FormatUpdate<T>(T entity, string[] excludeFields, bool isBatch = false)
		{
			var update = ResolveExpression.ResolveUpdates<T>(entity, Params, excludeFields);

			ProviderOption.IsAsName = false;

			var whereSql = ResolveExpression.ResolveWhereList();
			//如果不存在条件，就用主键作为条件
			if (!isBatch)
				if (whereSql.Trim().Equals("WHERE 1=1"))
					whereSql += GetIdentityWhere(entity, Params);

			SqlString = $"UPDATE {FormatTableName(false, false)} {update} {whereSql}";
			return this;
		}

		public override SqlProvider FormatSum(LambdaExpression sumExpression)
		{
			var selectSql = ResolveExpression.ResolveSum(sumExpression);

			var fromTableSql = FormatTableName();

			var nolockSql = ResolveExpression.ResolveWithNoLock(Context.Set.NoLock);

			var whereSql = ResolveExpression.ResolveWhereList();

			string noneSql = "";
			var joinSql = ResolveExpression.ResolveJoinSql(JoinList, ref noneSql);

			SqlString = $"{selectSql} {fromTableSql} {nolockSql} {joinSql} {whereSql} ";

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

			var selectSql = ResolveExpression.ResolveSelectOfUpdate(EntityCache.QueryEntity(typeof(T)), Context.Set.SelectExpression);

			var whereSql = ResolveExpression.ResolveWhereList();
			Params.AddDynamicParams(update.Param);

			SqlString = $"UPDATE {FormatTableName(false, false)} {update.SqlCmd} {selectSql} {whereSql}";

			return this;
		}
	}
}
