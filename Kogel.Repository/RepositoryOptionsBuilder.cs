using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension;

namespace Kogel.Repository
{
	public class RepositoryOptionsBuilder
	{
		/// <summary>
		/// 数据库连接
		/// </summary>
		internal IDbConnection Connection { get; private set; }

		/// <summary>
		/// 数据提供者
		/// </summary>
		internal SqlProvider Provider { get; private set; }

		/// <summary>
		/// 是否自动同步
		/// </summary>
		internal bool IsAutoSyncStructure { get; private set; }

		/// <summary>
		/// 配置连接方式
		/// </summary>
		/// <param name="connectionFactory"></param>
		/// <returns></returns>
		public RepositoryOptionsBuilder BuildConnection(IDbConnection connection)
		{
			this.Connection = connection;
			return this;
		}
		/// <summary>
		/// 配置数据库提供者
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public RepositoryOptionsBuilder BuildProvider(SqlProvider provider)
		{
			this.Provider = provider;
			return this;
		}

		/// <summary>
		/// 配置是否自动同步实体结构到数据库
		/// </summary>
		/// <param name="IsAutoSyncStructure"></param>
		/// <returns></returns>
		public RepositoryOptionsBuilder BuildAutoSyncStructure(bool IsAutoSyncStructure = false)
		{
			this.IsAutoSyncStructure = IsAutoSyncStructure;
			return this;
		}
	}
}
