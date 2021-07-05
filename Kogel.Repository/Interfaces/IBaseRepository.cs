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
	public interface IBaseRepository<T>: IDisposable
	{
		/// <summary>
		/// 当前的连接对象
		/// </summary>
		IDbConnection Orm { get; }

		/// <summary>
		/// 改变当前连接库（分库时可能会用到）
		/// </summary>
		/// <param name="dbName"></param>
		void ChangeDataBase(string dbName = "master");

		/// <summary>
		/// 改变当前提供方（分库时可能会用到）
		/// </summary>
		/// <param name="dbName"></param>
		void ChangeProvider(string dbName = "master");

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
		/// <returns></returns>
		ICommandSet<TEntity> CommandSet<TEntity>();

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="transaction"></param>
		/// <returns></returns>
		ICommandSet<TEntity> CommandSet<TEntity>(IDbTransaction transaction);

		/// <summary>
		/// 根据主键获取当前实体数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		T FindById(object id);

		/// <summary>
		/// 根据主键获取当前实体数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<T> FindByIdAsync(object id);

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int Insert(T entity);

		/// <summary>
		/// 批量新增
		/// </summary>
		/// <param name="entitys"></param>
		/// <returns></returns>
		int Insert(IEnumerable<T> entitys);

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<int> InsertAsync(T entity);

		/// <summary>
		/// 批量新增
		/// </summary>
		/// <param name="entitys"></param>
		/// <returns></returns>
		Task<int> InsertAsync(IEnumerable<T> entitys);

		/// <summary>
		/// 删除(根据主键)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		int Delete(object id);

		/// <summary>
		/// 删除(根据主键)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<int> DeleteAsync(object id);

		/// <summary>
		/// 修改(根据主键)
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int Update(T entity);

		/// <summary>
		/// 修改(根据主键)
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<int> UpdateAsync(T entity);
	}
}
