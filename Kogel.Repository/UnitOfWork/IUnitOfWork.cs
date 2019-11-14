using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Kogel.Repository.UnitOfWork
{
	public interface IUnitOfWork: IDisposable
	{
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
