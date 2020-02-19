using Kogel.Dapper.Extension.Core.SetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension.Core.SetC;

namespace Kogel.Repository.Interfaces
{
	public interface IBaseRepository<T>
	{
		/// <summary>
		/// 
		/// </summary>
		IDbConnection Orm { get; }

		/// <summary>
		/// 工作单元
		/// </summary>
		IUnitOfWork UnitOfWork { get; set; }

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <returns></returns>
		QuerySet<T> QuerySet();

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		QuerySet<T> QuerySet(IDbTransaction transaction);

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <returns></returns>
		CommandSet<T> CommandSet();

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		CommandSet<T> CommandSet(IDbTransaction transaction);

		/// <summary>
		/// 根据主键获取当前实体数据
		/// </summary>
		/// <returns></returns>
		T FindById(int id);

		/// <summary>
		/// 增加(并且返回自增主键到写入到实体中)
		/// </summary>
		/// <returns></returns>
		int Insert(T entity);

		/// <summary>
		/// 删除(根据主键)
		/// </summary>
		/// <returns></returns>
		int Delete(int id);

		/// <summary>
		/// 修改(根据主键)
		/// </summary>
		/// <returns></returns>
		int Update(T entity);
	}
}
