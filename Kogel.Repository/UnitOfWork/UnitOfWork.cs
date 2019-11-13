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

		public void BeginTransaction(IsolationLevel IsolationLevel)
		{
			if (connection.State == ConnectionState.Closed)
				connection.Open();
			transaction = connection.BeginTransaction(IsolationLevel);
			//事务注入
			SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
		}

		private void Aop_OnExecuting(CommandDefinition command)
		{
			command.Transaction = transaction;
		}

		public void Commit()
		{
			if (transaction != null)
				transaction.Commit();
		}

		public void Rollback()
		{
			if (transaction != null)
				transaction.Commit();
		}

		public void Dispose()
		{
			if (transaction != null)
				transaction.Dispose();
			//取消事务注入
			SqlMapper.Aop.OnExecuting -= Aop_OnExecuting;
		}

		~UnitOfWork()
		{
			Dispose();
		}
	}
}
