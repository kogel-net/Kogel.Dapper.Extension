using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Repository
{
    /// <summary>
    /// 仓储扩展
    /// </summary>
    public static class RepositoryExtension
    {

        public class BaseRepositoryExtension<T> : BaseRepository<T>, IRepository<T>
        {
            public BaseRepositoryExtension(RepositoryOptionsBuilder options) : base(options)
            {
            }
            public override void OnConfiguring(RepositoryOptionsBuilder builder)
            {
                //通用仓储不实现此方法（交给后续直接添加）
            }
        }

        /// <summary>
        /// 获取自定义仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querySet"></param>
        /// <returns></returns>
        public static IBaseRepository<T> GetRepository<T>(this IQuerySet<T> querySet)
        {
            return (querySet as QuerySet<T>).GetRepository();
        }

        /// <summary>
        /// 获取自定义仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querySet"></param>
        /// <returns></returns>
        public static IBaseRepository<T> GetRepository<T>(this QuerySet<T> querySet)
        {
            //从基础querySet对象中取出连接对象和提供方
            var options = new RepositoryOptionsBuilder();
            options.BuildConnection(x => querySet.DbCon);
            options.BuildProvider(querySet.SqlProvider);
            //设置给通用仓储
            var baseRepository = new BaseRepositoryExtension<T>(options);
            return baseRepository;
        }

        /// <summary>
        /// 获取自定义仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        public static IBaseRepository<T> GetRepository<T>(this ICommandSet<T> commandSet)
        {
            return (commandSet as CommandSet<T>).GetRepository();
        }

        /// <summary>
        /// 获取自定义仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        public static IBaseRepository<T> GetRepository<T>(this CommandSet<T> commandSet)
        {
            //从基础querySet对象中取出连接对象和提供方
            var options = new RepositoryOptionsBuilder();
            options.BuildConnection(x => commandSet.DbCon);
            options.BuildProvider(commandSet.SqlProvider);
            //设置给通用仓储
            var baseRepository = new BaseRepositoryExtension<T>(options);
            return baseRepository;
        }

        /// <summary>
        /// 排除在工作单元外
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querySet"></param>
        /// <returns></returns>
        public static IQuerySet<T> NotUnitOfWork<T>(this IQuerySet<T> querySet)
        {
            NotUnitOfWork(querySet as QuerySet<T>);
            return querySet;
        }

        /// <summary>
        /// 排除在工作单元外
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querySet"></param>
        /// <returns></returns>
        public static QuerySet<T> NotUnitOfWork<T>(this QuerySet<T> querySet)
        {
            querySet.SqlProvider.IsExcludeUnitOfWork = true;
            return querySet;
        }

        /// <summary>
        /// 排除在工作单元外
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        public static ICommandSet<T> NotUnitOfWork<T>(this ICommandSet<T> commandSet)
        {
            NotUnitOfWork(commandSet as CommandSet<T>);
            return commandSet;
        }

        /// <summary>
        /// 排除在工作单元外
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        public static CommandSet<T> NotUnitOfWork<T>(this CommandSet<T> commandSet)
        {
            commandSet.SqlProvider.IsExcludeUnitOfWork = true;
            return commandSet;
        }
    }

    /// <summary>
    /// 简化仓储名称
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IBaseRepository<T>
    {

    }
}
