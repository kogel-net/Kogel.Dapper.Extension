using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Kogel.Repository
{
	public class RepositoryOptionsBuilder
	{
		internal IDbConnection connection { get; private set; }
		/// <summary>
		/// 配置连接方式
		/// </summary>
		/// <param name="connectionFactory"></param>
		/// <returns></returns>
		public RepositoryOptionsBuilder BuildConnection(IDbConnection connection)
		{
			this.connection = connection;
			return this;
		}
		///// <summary>
		///// 仓储连接对象注入方式
		///// </summary>
		///// <param name="repositoryDI"></param>
		///// <returns></returns>
		//public RepositoryOptionsBuilder BuildRepositoryDI(RepositoryDI repositoryDI)
		//{
		//	this.repositoryDI = repositoryDI;
		//	return this;
		//}
	}
	/// <summary>
	/// 仓储连接对象注入方式
	/// </summary>
	public enum RepositoryDI
	{
		/// <summary>
		/// 每次创建对象获取一次连接对象
		/// </summary>
		Transient = 0,
		/// <summary>
		/// 同一线程内获取一次连接对象
		/// </summary>
		Scoped = 1,
	}
}
