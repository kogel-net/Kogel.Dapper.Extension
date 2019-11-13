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
	public abstract class BaseRepository
	{
		[ThreadStatic]
		internal static IDbConnection _orm;
		/// <summary>
		/// 连接对象
		/// </summary>
		public IDbConnection Orm => _orm ?? throw new DapperExtensionException("请在 OnConfiguring 中配置Connection");

		public BaseRepository()
		{
			RepositoryOptionsBuilder builder = new RepositoryOptionsBuilder();
			OnConfiguring(builder);
			if (_orm == null)
			{
				_orm = builder.connection;
				UnitOfWork = new UnitOfWork.UnitOfWork(_orm);
			}
			else
			{
				//关闭多余的连接
				if (builder.connection != null)
					builder.connection.Dispose();
			}
		}

		~BaseRepository()
		{
			if (_orm != null)
			{
				_orm.Dispose();
			}
		}
		/// <summary>
		/// 配置连接信息
		/// </summary>
		/// <param name="connectionFactory"></param>
		public abstract void OnConfiguring(RepositoryOptionsBuilder builder);
		/// <summary>
		/// 工作单元
		/// </summary>
		public IUnitOfWork UnitOfWork { get; set; }
	}
}
