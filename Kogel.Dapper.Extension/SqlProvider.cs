using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Dapper;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Helper;

namespace Kogel.Dapper.Extension
{
    public abstract class SqlProvider
    {
        public AbstractDataBaseContext Context { get; set; }

        protected SqlProvider()
        {
            Params = new DynamicParameters();
            JoinList = new List<JoinAssTable>();
        }

        public abstract IProviderOption ProviderOption { get; set; }

        public string SqlString { get; set; }
        /// <summary>
        /// 是否在查询分页时查询总数
        /// </summary>
        public bool IsSelectCount = false;
        /// <summary>
        /// 连接对象集合
        /// </summary>
        public List<JoinAssTable> JoinList { get; set; }

        public DynamicParameters Params { get; set; }

        public abstract SqlProvider FormatGet<T>();

        public abstract SqlProvider FormatToList<T>();

        public abstract SqlProvider FormatToPageList<T>(int pageIndex, int pageSize,bool IsSelectCount=true);

        public abstract SqlProvider FormatCount();

        public abstract SqlProvider FormatDelete();

        public abstract SqlProvider FormatInsert<T>(T entity);

        public abstract SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression);

        public abstract SqlProvider FormatUpdate<T>(T entity);

        public abstract SqlProvider FormatSum(LambdaExpression sumExpression);

        public abstract SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator);

        protected string FormatTableName(bool isNeedFrom = true, bool isAsName = true)
        {
            var entity = EntityCache.QueryEntity(Context.Set.TableType);
            string schema = string.IsNullOrEmpty(entity.Schema) ? "" : ProviderOption.CombineFieldName(entity.Schema) + ".";
            string fromName = entity.Name;
            if (isAsName)
                fromName = entity.AsName.Equals(entity.Name) ? ProviderOption.CombineFieldName(entity.Name) : $"{ProviderOption.CombineFieldName(entity.Name)} {entity.AsName}";
            SqlString = $" {schema}{fromName} ";
            if (isNeedFrom)
                SqlString = " FROM " + SqlString;

            return SqlString;
        }

        protected string[] FormatInsertParamsAndValues<T>(T t)
        {
            var paramSqlBuilder = new StringBuilder(64);
            var valueSqlBuilder = new StringBuilder(64);

            var entity = EntityCache.QueryEntity(typeof(T));
            var properties = entity.Properties;

            var isAppend = false;
            foreach (var propertiy in properties)
            {
                //主键标识
                var typeAttribute = entity.Type.GetCustomAttributess(true).FirstOrDefault(x => x.GetType().Equals(typeof(Identity)));
                if (typeAttribute != null)
                {
                    var identity = typeAttribute as Identity;
                    //是否自增
                    if (identity.IsIncrease)
                    {
                        continue;
                    }
                }
                if (isAppend)
                {
                    paramSqlBuilder.Append(",");
                    valueSqlBuilder.Append(",");
                }
                var name = propertiy.GetColumnAttributeName();
                paramSqlBuilder.AppendFormat("{0}{1}{2}", ProviderOption.OpenQuote, name, ProviderOption.CloseQuote);
                valueSqlBuilder.Append(ProviderOption.ParameterPrefix + name);
                Params.Add(ProviderOption.ParameterPrefix + name, propertiy.GetValue(entity));
                isAppend = true;
            }
            return new[] { paramSqlBuilder.ToString(), valueSqlBuilder.ToString() };
        }

        protected DataBaseContext<T> DataBaseContext<T>()
        {
            return (DataBaseContext<T>)Context;
        }
    }
}
