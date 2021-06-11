using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Repository.Interfaces;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Repository
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public RepositoryOptionsBuilder Options { get; set; }

        /// <summary>
        /// 当前的连接对象
        /// </summary>
        public IDbConnection Orm { get => Options.GetConnection("Orm")?.Connection ?? throw new DapperExtensionException("请在 OnConfiguring 中配置Connection"); }

        /// <summary>
        /// 工作单元
        /// </summary>
        public IUnitOfWork UnitOfWork { get; set; }

        public BaseRepository()
        {
            RepositoryOptionsBuilder builder = new RepositoryOptionsBuilder();
            OnConfiguring(builder);
            this.Options = builder;
            UnitOfWork = new UnitOfWork(Orm);

            //codefirst
            SyncStructure();
        }

        public BaseRepository(RepositoryOptionsBuilder repositoryOptions)
        {
            OnConfiguring(repositoryOptions);
            this.Options = repositoryOptions;
            UnitOfWork = new UnitOfWork(Orm);

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

            //释放整个连接池
            if (Options.CurrentConnectionPool != null)
                foreach (var item in Options.CurrentConnectionPool)
                {
                    item.Connection.Dispose();
                }
        }

        /// <summary>
        /// 改变连接（分库时可能会用到）
        /// </summary>
        /// <param name="dbName">为空时切换回主库</param>
        public void ChangeDataBase(string dbName = "master")
        {
            lock (Options.CurrentConnectionPool)
            {
                if (Options.CurrentConnectionPool.Any(x => x.DbName == dbName))
                {
                    foreach (var item in Options.CurrentConnectionPool)
                    {
                        if (item.DbName == dbName)
                        {
                            item.IsCurrent = true;
                        }
                        else
                        {
                            item.IsCurrent = false;
                        }
                    }
                }
                else
                {
                    Options.GetConnection(dbName);
                    ChangeDataBase(dbName);
                }
            }
        }

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <returns></returns>
        public IQuerySet<T> QuerySet()
        {
            return new QuerySet<T>(Orm, Options.Provider.Create(), null);
        }

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQuerySet<T> QuerySet(IDbTransaction transaction)
        {
            return new QuerySet<T>(Orm, Options.Provider.Create(), transaction);
        }

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IQuerySet<TEntity> QuerySet<TEntity>()
        {
            return new QuerySet<TEntity>(Orm, Options.Provider.Create());
        }

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQuerySet<TEntity> QuerySet<TEntity>(IDbTransaction transaction)
        {
            return new QuerySet<TEntity>(Orm, Options.Provider.Create(), transaction);
        }

        /// <summary>
        /// 获取编辑对象
        /// </summary>
        /// <returns></returns>
        public ICommandSet<T> CommandSet()
        {
            return new CommandSet<T>(Orm, Options.Provider.Create(), null);
        }

        /// <summary>
        /// 获取编辑对象
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public ICommandSet<T> CommandSet(IDbTransaction transaction)
        {
            return new CommandSet<T>(Orm, Options.Provider.Create(), transaction);
        }

        /// <summary>
        /// 获取编辑对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ICommandSet<TEntity> CommandSet<TEntity>()
        {
            return new CommandSet<TEntity>(Orm, Options.Provider.Create(), null);
        }

        /// <summary>
        /// 获取编辑对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public ICommandSet<TEntity> CommandSet<TEntity>(IDbTransaction transaction)
        {
            return new CommandSet<TEntity>(Orm, Options.Provider.Create(), transaction);
        }

        /// <summary>
        /// 根据主键获取当前实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(object id)
        {
            DynamicParameters param = new DynamicParameters();
            string whereSql = Options.Provider.GetIdentityWhere<T>(id, param);
            return QuerySet()
                .Where($" 1=1 {whereSql}", param)
                .Get();
        }

        /// <summary>
        /// 根据主键获取当前实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> FindByIdAsync(object id)
        {
            DynamicParameters param = new DynamicParameters();
            string whereSql = Options.Provider.GetIdentityWhere<T>(id, param);
            return await QuerySet()
                .Where($" 1=1 {whereSql}", param)
                .GetAsync();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(T entity)
        {
            return CommandSet().Insert(entity);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(T entity)
        {
            return await CommandSet().InsertAsync(entity);
        }

        /// <summary>
		/// 批量新增
		/// </summary>
		/// <param name="entitys"></param>
		/// <returns></returns>
		public int Insert(IEnumerable<T> entitys)
        {
            return CommandSet().Insert(entitys);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(IEnumerable<T> entitys)
        {
            return await CommandSet().InsertAsync(entitys);
        }

        /// <summary>
        /// 删除(根据主键)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(object id)
        {
            DynamicParameters param = new DynamicParameters();
            string whereSql = Options.Provider.GetIdentityWhere<T>(id, param);
            return CommandSet()
                .Where($" 1=1 {whereSql}", param)
                .Delete();
        }

        /// <summary>
        /// 删除(根据主键)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(object id)
        {
            DynamicParameters param = new DynamicParameters();
            string whereSql = Options.Provider.GetIdentityWhere<T>(id, param);
            return await CommandSet()
                .Where($" 1=1 {whereSql}", param)
                .DeleteAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(T entity)
        {
            DynamicParameters param = new DynamicParameters();
            string whereSql = Options.Provider.GetIdentityWhere(entity, param);
            return CommandSet()
                .Where($" 1=1 {whereSql}", param)
                .Update(entity);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(T entity)
        {
            DynamicParameters param = new DynamicParameters();
            string whereSql = Options.Provider.GetIdentityWhere(entity, param);
            return await CommandSet()
                .Where($" 1=1 {whereSql}", param)
                .UpdateAsync(entity);
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
            if (this.Options.IsAutoSyncStructure)
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
                        switch (this.Options.Provider.GetType().Name)
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
