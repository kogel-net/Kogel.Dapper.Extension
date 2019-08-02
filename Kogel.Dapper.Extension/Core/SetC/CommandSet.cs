using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Core.SetC
{
    /// <summary>
    /// 指令集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandSet<T> : Command<T>, ICommandSet<T>
    {
        public CommandSet(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {
            TableType = typeof(T);
            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Command
            };

            sqlProvider.Context = SetContext;
            WhereExpressionList = new List<LambdaExpression>();
            WhereBuilder = new StringBuilder();
            Params = new DynamicParameters();
        }

        public CommandSet(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {
            TableType = typeof(T);
            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Command
            };

            sqlProvider.Context = SetContext;

            WhereExpressionList = new List<LambdaExpression>();
            WhereBuilder = new StringBuilder();
            Params = new DynamicParameters();
        }

        internal CommandSet(IDbConnection conn, SqlProvider sqlProvider, Type tableType, LambdaExpression whereExpression) : base(conn, sqlProvider)
        {
            TableType = tableType;
            //WhereExpression = whereExpression;
            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Command
            };

            sqlProvider.Context = SetContext;

            WhereExpressionList = new List<LambdaExpression>();
            WhereExpressionList.Add(whereExpression);
            WhereBuilder = new StringBuilder();
            Params = new DynamicParameters();
        }

        public ICommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            //WhereExpression = WhereExpression == null ? predicate : ((Expression<Func<T, bool>>)WhereExpression).And(predicate);
            WhereExpressionList.Add(predicate);
            return this;
        }

        public IInsert<T> IfNotExists(Expression<Func<T, bool>> predicate)
        {
            //IfNotExistsExpression = IfNotExistsExpression == null ? predicate : ((Expression<Func<T, bool>>)IfNotExistsExpression).And(predicate);
            WhereExpressionList.Add(predicate);
            return this;
        }
    }
}
