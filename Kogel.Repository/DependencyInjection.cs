#if NETCOREAPP || NETSTANDARD2_0
using Kogel.Dapper.Extension;
using Kogel.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using static Kogel.Repository.RepositoryExtension;

namespace Kogel.Repository
{
    public static class DependencyInjection
    {
        /// <summary>
        /// 注册仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IServiceCollection AddKogelRepository(this IServiceCollection services, Action<RepositoryOptionsBuilder> setup)
        {
            services.AddTransient((x) =>
            {
                var options = new RepositoryOptionsBuilder();
                setup.Invoke(options);
                return options;
            });
            services.AddTransient(typeof(IRepository<>), typeof(BaseRepositoryExtension<>));
            return services;
        }
    }
}
#endif