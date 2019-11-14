using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Kogel.Repository.UnitOfWork
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
			//解析方法里的访问对象
			SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
			transactionMethod.Invoke();
			SqlMapper.Aop.OnExecuting -= Aop_OnExecuting;
			return this;
		}

		private void Aop_OnExecuting(ref IDbConnection connection,ref CommandDefinition command)
		{
			connection = this.connection;
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
