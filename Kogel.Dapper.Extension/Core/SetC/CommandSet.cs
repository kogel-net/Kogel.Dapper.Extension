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

        public ICommand<T> AsTableName(Type type, string tableName)
        {
            SqlProvider.AsTableNameDic.Add(type, tableName);
            return this;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
        public ICommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            WhereExpressionList.Add(predicate);
            return this;
        }
		/// <summary>
		/// 使用sql查询条件
		/// </summary>
		/// <param name="sqlWhere"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public ICommand<T> Where(string sqlWhere, object param = null)
		{
			WhereBuilder.Append(" AND " + sqlWhere);
			if (param != null)
			{
				Params.AddDynamicParams(param);
			}
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="where"></param>
		/// <param name="truePredicate"></param>
		/// <param name="falsePredicate"></param>
		/// <returns></returns>
		public ICommand<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate)
		{
			if (where)
				WhereExpressionList.Add(truePredicate);
			else
				WhereExpressionList.Add(falsePredicate);
			return this;
		}
	}
}
