using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension.Exception;
using Kogel.Repository.UnitOfWork;

namespace Kogel.Repository
{
	public abstract class BaseRepository: IDisposable
	{
		internal IDbConnection _orm;
		/// <summary>
		/// 连接对象
		/// </summary>
		public IDbConnection Orm => _orm ?? throw new DapperExtensionException("请在 OnConfiguring 中配置Connection");

		public BaseRepository()
		{
			RepositoryOptionsBuilder builder = new RepositoryOptionsBuilder();
			OnConfiguring(builder);
			this._orm = builder.connection;
			UnitOfWork = new UnitOfWork.UnitOfWork(_orm);
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
			if (_orm != null)
				_orm.Dispose();
			if (UnitOfWork != null)
				UnitOfWork.Dispose();
		}

		/// <summary>
		/// 工作单元
		/// </summary>
		public IUnitOfWork UnitOfWork { get; set; }
	}
}
