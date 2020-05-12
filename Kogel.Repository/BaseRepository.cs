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
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Repository
{
	public abstract class BaseRepository<T> : IBaseRepository<T>, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		internal RepositoryOptionsBuilder OptionsBuilder { get; }

		/// <summary>
		/// 连接对象
		/// </summary>
		public IDbConnection Orm { get => OptionsBuilder?.Connection ?? throw new DapperExtensionException("请在 OnConfiguring 中配置Connection"); }

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

			//codefirst
			SyncStructure();
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
			if (UnitOfWork != null)
				UnitOfWork.Dispose();
			else
			{
				if (Orm != null)
				{
					Orm.Dispose();
				}
			}
		}

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <returns></returns>
		public IQuerySet<T> QuerySet()
		{
			return new QuerySet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew(), null);
		}

		/// <summary>
		/// 获取查询对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public IQuerySet<T> QuerySet(IDbTransaction transaction)
		{
			return new QuerySet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew(), transaction);
		}

		public IQuerySet<TEntity> QuerySet<TEntity>()
		{
			return new QuerySet<TEntity>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew());
		}

		public IQuerySet<TEntity> QuerySet<TEntity>(IDbTransaction transaction)
		{
			return new QuerySet<TEntity>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew(), transaction);
		}

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <returns></returns>
		public ICommandSet<T> CommandSet()
		{
			return new CommandSet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew(), null);
		}

		/// <summary>
		/// 获取编辑对象
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public ICommandSet<T> CommandSet(IDbTransaction transaction)
		{
			return new CommandSet<T>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew(), transaction);
		}

		public ICommandSet<TEntity> CommandSet<TEntity>(IDbTransaction transaction = null)
		{
			return new CommandSet<TEntity>(OptionsBuilder.Connection, OptionsBuilder.Provider.CreateNew(), transaction);
		}

		/// <summary>
		/// 根据主键获取当前实体数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public T FindById(object id)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, id);
			return this.QuerySet()
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
				//var id = this.CommandSet()
				//   .InsertIdentity(entity);
				////写入主键数据
				//entityObject.EntityFieldList
				//	.First(x => x.IsIdentity).PropertyInfo
				//	.SetValue(entity, id);
				return this.CommandSet()
				   .Insert(entity);
			}
			else
			{
				//不存在就直接返回影响行数
				return this.CommandSet()
					.Insert(entity);
			}
		}

		/// <summary>
		/// 删除(根据主键)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int Delete(object id)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, id);
			return this.CommandSet()
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
			var id = entityObject.EntityFieldList
				.First(x => x.IsIdentity).PropertyInfo
				.GetValue(entity);
			//设置参数
			DynamicParameters param = new DynamicParameters();
			param.Add(entityObject.Identitys, id);
			return this.CommandSet()
				.Where($"{entityObject.Identitys}={OptionsBuilder.Provider.ProviderOption.ParameterPrefix}{entityObject.Identitys}", param)
				.Update(entity);
		}

		/// <summary>
		/// 是否同步过实体
		/// </summary>
		private static bool IsFirstSyncStructure = false;

		/// <summary>
		/// 
		/// </summary>
		private static object AutoSyncLock = new object();

		/// <summary>
		/// 同步结构
		/// </summary>
		private void SyncStructure()
		{
			//用户配置是否需要同步
			if (this.OptionsBuilder.IsAutoSyncStructure)
			{
				lock (AutoSyncLock)
				{
					if (!IsFirstSyncStructure)
					{
						//标记已经第一次同步
						IsFirstSyncStructure = true;
						//命名空间
						string nameSpaces = string.Empty;
						//类的完全限定名
						string fullName = string.Empty;
						switch (this.OptionsBuilder.Provider.GetType().Name)
						{
							case "MsSqlProvider":
								{
									nameSpaces = "Kogel.Dapper.Extension.MsSql";
									fullName = "Kogel.Dapper.Extension.MsSql.Extension.CodeFirst";
									break;
								}
							case "MySqlProvider":
								{
									nameSpaces = "Kogel.Dapper.Extension.MySql";
									fullName = "Kogel.Dapper.Extension.MySql.Extension.CodeFirst";
									break;
								}
							case "OracleSqlProvider":
								{
									nameSpaces = "Kogel.Dapper.Extension.Oracle";
									fullName = "Kogel.Dapper.Extension.Oracle.Extension.CodeFirst";
									break;
								}
						}
						ICodeFirst codeFirst = (ICodeFirst)ReflectExtension.CreateInstance(nameSpaces, fullName, new object[] { Orm });
						UnitOfWork.BeginTransaction(() =>
						 {
							 //开始同步实体
							 codeFirst.SyncStructure();
						 });
						//提交（失败时会自动回滚）
						UnitOfWork.Commit();
					}
				}
			}
		}
	}
}
