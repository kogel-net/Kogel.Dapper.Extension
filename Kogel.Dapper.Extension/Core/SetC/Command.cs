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
        public IDbTransaction DbTransaction { get; private set; }

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

        public int Update(T entity, string[] excludeFields = null, int timeout = 120)
        {
            SqlProvider.FormatUpdate(entity, excludeFields);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, timeout, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public int Update(Expression<Func<T, T>> updateExpression)
        {
            SqlProvider.FormatUpdate(updateExpression);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public int Update(IEnumerable<T> entities, IDbDataAdapter adapter, string[] excludeFields = null, int timeout = 120)
        {
            SqlProvider.FormatUpdate(entities, excludeFields);
            return DbCon.Update(SqlProvider.SqlString, SqlProvider.Params, adapter, entities, SqlProvider, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<int> UpdateAsync(T entity, string[] excludeFields = null, int timeout = 120)
        {
            SqlProvider.FormatUpdate(entity, null);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression)
        {
            SqlProvider.FormatUpdate(updateExpression);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<int> UpdateAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120)
        {
            int result = 0;
            //bool isSeedTran = false;
            //if (DbTransaction == null)
            //{
            //    isSeedTran = true;
            //    if (DbCon.State == ConnectionState.Closed)
            //        DbCon.Open();
            //    DbTransaction = DbCon.BeginTransaction(IsolationLevel.ReadCommitted);
            //}
            foreach (var entity in entities)
            {
                result += await UpdateAsync(entity, excludeFields, timeout);
            }
            //if (isSeedTran)
            //{
            //    DbTransaction.Commit();
            //    DbTransaction.Dispose();
            //    DbCon.Close();
            //}
            return result;
        }

        public int Delete()
        {
            SqlProvider.FormatDelete();
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public int Delete(T model)
        {
            SqlProvider.FormatDelete();
            //设置参数
            DynamicParameters param = new DynamicParameters();
            SqlProvider.SqlString = $"{SqlProvider.SqlString} {SqlProvider.GetIdentityWhere(model, param)}";
            return DbCon.Execute(SqlProvider.SqlString, param, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<int> DeleteAsync()
        {
            SqlProvider.FormatDelete();
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<int> DeleteAsync(T model)
        {
            SqlProvider.FormatDelete();
            //设置参数
            DynamicParameters param = new DynamicParameters();
            SqlProvider.SqlString = $"{SqlProvider.SqlString} {SqlProvider.GetIdentityWhere(model, param)}";
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, param, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public int Insert(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatInsert(entity, excludeFields);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public long InsertIdentity(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatInsertIdentity(entity, excludeFields);
            object result = DbCon.ExecuteScalar(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
            return result != null ? Convert.ToInt64(result) : 0;
        }

        public int Insert(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120)
        {
            SqlProvider.FormatInsert(entities, excludeFields);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, timeout, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<int> InsertAsync(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatInsert(entity, excludeFields);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }

        public async Task<long> InsertIdentityAsync(T entity, string[] excludeFields = null)
        {
            SqlProvider.FormatInsertIdentity(entity, excludeFields);
            object result = await DbCon.ExecuteScalarAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
            return result != null ? Convert.ToInt64(result) : 0;
        }

        public async Task<int> InsertAsync(IEnumerable<T> entities, string[] excludeFields = null, int timeout = 120)
        {
            SqlProvider.FormatInsert(entities, excludeFields);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction, timeout, isExcludeUnitOfWork: SqlProvider.IsExcludeUnitOfWork);
        }
    }
}
