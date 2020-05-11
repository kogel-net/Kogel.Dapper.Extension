using Kogel.Dapper.Extension.Core.SetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.Interfaces;

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
		IQuerySet<T> QuerySet();

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		IQuerySet<T> QuerySet(IDbTransaction transaction);

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <returns></returns>
		IQuerySet<TEntity> QuerySet<TEntity>();

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="transaction"></param>
		/// <returns></returns>
		IQuerySet<TEntity> QuerySet<TEntity>(IDbTransaction transaction);

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <returns></returns>
		ICommandSet<T> CommandSet();

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		ICommandSet<T> CommandSet(IDbTransaction transaction);

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="transaction"></param>
		/// <returns></returns>
		ICommandSet<TEntity> CommandSet<TEntity>(IDbTransaction transaction = null);


		/// <summary>
		/// 根据主键获取当前实体数据
		/// </summary>
		/// <returns></returns>
		T FindById(object id);

		/// <summary>
		/// 增加(并且返回自增主键到写入到实体中)
		/// </summary>
		/// <returns></returns>
		int Insert(T entity);

		/// <summary>
		/// 删除(根据主键)
		/// </summary>
		/// <returns></returns>
		int Delete(object id);

		/// <summary>
		/// 修改(根据主键)
		/// </summary>
		/// <returns></returns>
		int Update(T entity);
	}
}
