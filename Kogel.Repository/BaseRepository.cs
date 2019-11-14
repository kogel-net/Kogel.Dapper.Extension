using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Repository.Interfaces;

namespace Kogel.Repository
{
	public abstract class BaseRepository<T> : IBaseRepository<T>, IDisposable
	{
		internal RepositoryOptionsBuilder OptionsBuilder { get; }
		/// <summary>
		/// 连接对象
		/// </summary>
		public IDbConnection Orm => OptionsBuilder?.Connection ?? throw new DapperExtensionException("请在 OnConfiguring 中配置Connection");

		/// <summary>
		/// 工作单元
		/// </summary>
		public IUnitOfWork UnitOfWork { get; set; }

		public BaseRepository()
		{
			RepositoryOptionsBuilder builder = new RepositoryOptionsBuilder();
			OnConfiguring(builder);
			this.OptionsBuilder = builder;
			UnitOfWork = new UnitOfWork(this.OptionsBuilder.Connection);
		}

		~BaseRepository()
		{
			Dispose();
		}
		/// <summary>
		/// 配置连接信息
		/// </summary>
		/// <param name="connectionFactory"></param>
		public abstract void OnConfiguring(RepositoryOptionsBuilder builder);

		public void Dispose()
		{
			if (OptionsBuilder.Connection != null)
				OptionsBuilder.Connection.Dispose();
			if (UnitOfWork != null)
				UnitOfWork.Dispose();
		}

		public QuerySet<T> QuerySet()
		{
			return new QuerySet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, null);
		}

		public QuerySet<T> QuerySet(IDbTransaction transaction)
		{
			return new QuerySet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, transaction);
		}

		public CommandSet<T> CommandSet()
		{
			return new CommandSet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, null);
		}

		public CommandSet<T> CommandSet(IDbTransaction transaction)
		{
			return new CommandSet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, transaction);
		}
	}
}
