using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kogel.Repository.Interfaces;

namespace Kogel.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private IDbConnection connection { get; }
		private IDbTransaction transaction { get; set; }
		public UnitOfWork(IDbConnection connection)
		{
			this.connection = connection;
		}
		/// <summary>
		/// 开始事务
		/// </summary>
		/// <param name="transactionMethod"></param>
		/// <param name="IsolationLevel"></param>
		/// <returns></returns>
		public IUnitOfWork BeginTransaction(Action transactionMethod, IsolationLevel IsolationLevel = IsolationLevel.Serializable)
		{
			if (connection.State == ConnectionState.Closed)
				connection.Open();
			transaction = connection.BeginTransaction(IsolationLevel);
			SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
			try
			{
				transactionMethod.Invoke();
			}
			catch(Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			SqlMapper.Aop.OnExecuting -= Aop_OnExecuting;
			return this;
		}
		/// <summary>
		/// 工作单元内所有访问数据库操作执行前
		/// </summary>
		/// <param name="command"></param>
		private void Aop_OnExecuting(ref CommandDefinition command)
		{
			command.Connection = this.connection;
			command.Transaction = this.transaction;
		}
		/// <summary>
		/// 提交
		/// </summary>
		public void Commit()
		{
			if (transaction != null)
				transaction.Commit();
		}
		/// <summary>
		/// 回滚
		/// </summary>
		public void Rollback()
		{
			if (transaction != null)
				transaction.Rollback();
		}
		/// <summary>
		/// 释放对象
		/// </summary>
		public void Dispose()
		{
			if (transaction != null)
				transaction.Dispose();
			if (connection != null)
			{
				if (connection.State == ConnectionState.Open)
					connection.Close();
				connection.Dispose();
			}
		}

		~UnitOfWork()
		{
			Dispose();
		}
	}
}
