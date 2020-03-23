using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using System.Linq;

namespace Kogel.Dapper.Extension.Core.SetC
{
    /// <summary>
    /// 指令
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Command<T> : AbstractSet, ICommand<T>
    {
   
        public readonly IDbConnection DbCon;
        public readonly IDbTransaction DbTransaction;

        protected DataBaseContext<T> SetContext { get; set; }

        protected Command(IDbConnection conn, SqlProvider sqlProvider)
        {
            SqlProvider = sqlProvider;
            DbCon = conn;
        }

        protected Command(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction)
        {
            SqlProvider = sqlProvider;
            DbCon = conn;
            DbTransaction = dbTransaction;
        }

        public int Update(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatUpdate(entity, excludeFields);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

		public int Update(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120)
		{
			SqlProvider.FormatUpdate(entities.FirstOrDefault(), excludeFields, true);
			//批量修改不需要别名（暂时有点小bug，先勉强使用下）
			SqlProvider.SqlString = SqlProvider.SqlString.Replace("Update_", "").Replace("_0","").Replace("_1", "");
			var identity = EntityCache.QueryEntity(typeof(T)).Identitys;
			SqlProvider.SqlString += $" AND {identity}={SqlProvider.ProviderOption.ParameterPrefix + identity}";
			return DbCon.Execute(SqlProvider.SqlString, entities, DbTransaction, timeout);
		}

		public async Task<int> UpdateAsync(T entity)
        {
            SqlProvider.FormatUpdate(entity, null);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Update(Expression<Func<T, T>> updateExpression)
        {
            SqlProvider.FormatUpdate(updateExpression);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression)
        {
            SqlProvider.FormatUpdate(updateExpression);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Delete()
        {
            SqlProvider.FormatDelete();
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> DeleteAsync()
        {
            SqlProvider.FormatDelete();
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Insert(T entity, string[] excludeFields = null)
        {
			SqlProvider.FormatInsert(entity, excludeFields);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> InsertAsync(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatInsert(entity, excludeFields);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int InsertIdentity(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatInsertIdentity(entity, excludeFields);
            object result= DbCon.ExecuteScalar(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result != null ? Convert.ToInt32(result) : 0;
        }

		public int Insert(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120)
		{
			SqlProvider.FormatInsert(entities.FirstOrDefault(), excludeFields);
			return DbCon.Execute(SqlProvider.SqlString, entities, DbTransaction, timeout);
		}

        public async Task<int> InsertAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120)
        {
            SqlProvider.FormatInsert(entities.FirstOrDefault(), excludeFields);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, entities, DbTransaction, timeout);
        }

		public int Delete(T model)
		{
			SqlProvider.FormatDelete();
			var entityObject = EntityCache.QueryEntity(model.GetType());
			var identity = entityObject.EntityFieldList.FirstOrDefault(x => x.IsIdentity);
			if (identity == null)
				throw new System.Exception("主键不存在!");
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, identity.PropertyInfo.GetValue(model));
			return DbCon.Execute($@"{SqlProvider.SqlString} AND {entityObject.Identitys}={SqlProvider.ProviderOption.ParameterPrefix}{entityObject.Identitys}
			", param);
		}
	}
}
