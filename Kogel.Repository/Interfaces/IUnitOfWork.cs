using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Kogel.Repository.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		/// <summary>
		/// 数据库连接
		/// </summary>
		IDbConnection Connection { get; }
		/// <summary>
		/// 工作单元事务
		/// </summary>
		IDbTransaction Transaction { get; set; }
		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="transactionMethod"></param>
		/// <param name="IsolationLevel"></param>
		IUnitOfWork BeginTransaction(Action transactionMethod, IsolationLevel IsolationLevel = IsolationLevel.Serializable);
		/// <summary>
		/// 提交
		/// </summary>
		void Commit();
		/// <summary>
		/// 回滚
		/// </summary>
		void Rollback();
	}
}
