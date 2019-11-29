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
using Kogel.Dapper.Extension;
using Dapper;

namespace Kogel.Repository
{
	public abstract class BaseRepository<T> : IBaseRepository<T>, IDisposable
	{
		internal RepositoryOptionsBuilder OptionsBuilder { get; }
		/// <summary>
		/// 连接对象
		/// </summary>
		public IDbConnection Orm{ get => OptionsBuilder?.Connection ?? throw new DapperExtensionException("请在 OnConfiguring 中配置Connection"); }
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
		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <returns></returns>
		public QuerySet<T> QuerySet()
		{
			return new QuerySet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, null);
		}
		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public QuerySet<T> QuerySet(IDbTransaction transaction)
		{
			return new QuerySet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, transaction);
		}
		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <returns></returns>
		public CommandSet<T> CommandSet()
		{
			return new CommandSet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, null);
		}
		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public CommandSet<T> CommandSet(IDbTransaction transaction)
		{
			return new CommandSet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider, transaction);
		}
		/// <summary>
		/// 根据主键获取当前实体数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public T FindById(int id)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, id);
			return this.QuerySet(this.UnitOfWork.Transaction)
				.Where($"{entityObject.Identitys}={OptionsBuilder.Provider.ProviderOption.ParameterPrefix}{entityObject.Identitys}", param)
				.Get();
		}
		/// <summary>
		/// 增加(并且返回自增主键到写入到实体中)
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int Insert(T entity)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			//存在主键写入自增id
			if (!string.IsNullOrEmpty(entityObject.Identitys))
			{
				var id = this.CommandSet(this.UnitOfWork.Transaction)
			       .InsertIdentity(entity);
				//写入主键数据
				entityObject.Properties
				   .FirstOrDefault(x => x.Name == entityObject.Identitys)
				   .SetValue(entity, id);
				return 1;
			}
			else
			{
				//不存在就直接返回影响行数
				return this.CommandSet(this.UnitOfWork.Transaction)
					.Insert(entity);
			}
		}
		/// <summary>
		/// 删除(根据主键)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int Delete(int id)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, id);
			return this.CommandSet(this.UnitOfWork.Transaction)
				.Where($"{entityObject.Identitys}={OptionsBuilder.Provider.ProviderOption.ParameterPrefix}{entityObject.Identitys}", param)
			    .Delete();
		}
		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int Update(T entity)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			//获取主键数据
			var id = entityObject.Properties
				.FirstOrDefault(x => x.Name == entityObject.Identitys)
				.GetValue(entity);
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, id);
			return this.CommandSet(this.UnitOfWork.Transaction)
				.Where($"{entityObject.Identitys}={OptionsBuilder.Provider.ProviderOption.ParameterPrefix}{entityObject.Identitys}", param)
				.Update(entity);
		}
	}
}
