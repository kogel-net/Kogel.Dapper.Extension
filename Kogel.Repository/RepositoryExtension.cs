using Kogel.Dapper.Extension.Core.Interfaces;
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

        public class BaseRepositoryExtension<T> : BaseRepository<T>
        {
            public BaseRepositoryExtension(RepositoryOptionsBuilder options) : base(options)
            {
            }
            public override void OnConfiguring(RepositoryOptionsBuilder builder)
            {
                //通用仓储不实现此方法（交给后续直接添加）
            }
        }

        public static IBaseRepository<T> GetRepository<T>(this IQuerySet<T> iQuerySet)
        {
            QuerySet<T> querySet = iQuerySet as QuerySet<T>;
            return querySet.GetRepository();
        }

        public static IBaseRepository<T> GetRepository<T>(this QuerySet<T> querySet)
        {
            //从基础querySet对象中取出连接对象和提供方
            var options = new RepositoryOptionsBuilder();
            options.BuildConnection(querySet.DbCon);
            options.BuildProvider(querySet.SqlProvider);
            //设置给通用仓储
            var baseRepository = new BaseRepositoryExtension<T>(options);
            return baseRepository;
        }
    }
}
