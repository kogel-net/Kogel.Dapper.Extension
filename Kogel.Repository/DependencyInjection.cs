#if NETCOREAPP
using Kogel.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using static Kogel.Repository.RepositoryExtension;

namespace Kogel.Repository
{
    public static class DependencyInjection
    {
        private static IServiceCollection _services;
        private static readonly MethodInfo _registerMethod;
        static DependencyInjection()
        {
            _registerMethod = typeof(DependencyInjection).GetMethod("Register");
        }
        /// <summary>
        /// 注册仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IServiceCollection AddKogelRepository(this IServiceCollection services, Action<RepositoryOptionsBuilder> setup)
        {
            _services = services;
            var options = new RepositoryOptionsBuilder();
            setup.Invoke(options);
            return services;
        }

        /// <summary>
        /// 根据实体类型动态注册仓储
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entite"></param>
        /// <returns></returns>
        public static RepositoryOptionsBuilder BuilderType(this RepositoryOptionsBuilder builder, Type entite)
        {
            _registerMethod
               .MakeGenericMethod(new Type[] { entite })
               .Invoke(null, new object[] { _services, builder });
            return builder;
        }

        /// <summary>
        /// 根据实体类型动态注册仓储
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entite"></param>
        /// <returns></returns>
        public static RepositoryOptionsBuilder BuilderType(this RepositoryOptionsBuilder builder, Type[] entites)
        {
            foreach (var entite in entites)
            {
                BuilderType(builder, entite);
            }
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        public static void Register<T>(IServiceCollection services, RepositoryOptionsBuilder builder)
        {
            services.AddTransient<IRepository<T>>(x => new BaseRepositoryExtension<T>(builder));
        }
    }
}
#endif