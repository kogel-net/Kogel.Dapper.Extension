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
        public readonly SqlProvider SqlProvider;
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

        public int Update(T entity)
        {
            SqlProvider.FormatUpdate(entity);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            SqlProvider.FormatUpdate(entity);
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

        public int Insert(T entity)
        {
            SqlProvider.FormatInsert(entity);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> InsertAsync(T entity)
        {
            SqlProvider.FormatInsert(entity);
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int InsertIdentity(T entity)
        {
            SqlProvider.FormatInsertIdentity(entity);
            object result= DbCon.ExecuteScalar(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int BatchInsert(IEnumerable<T> entities, int timeout = 120)
        {
            SqlProvider.FormatInsert(entities.FirstOrDefault());
            return DbCon.Execute(SqlProvider.SqlString, entities, DbTransaction, timeout);
        }

        public async Task<int> BatchInsertAsync(IEnumerable<T> entities, int timeout = 120)
        {
            SqlProvider.FormatInsert(entities.FirstOrDefault());
            return await DbCon.ExecuteAsync(SqlProvider.SqlString, entities, DbTransaction, timeout);
        }
    }
}
