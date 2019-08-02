using System;
using Kogel.Dapper.Extension.Attributes;
using System.Linq;
using System.Reflection;
using Kogel.Dapper.Extension.Exception;

namespace Kogel.Dapper.Extension.Extension
{
    public static class ReflectExtension
    {
        public static PropertyInfo GetKeyPropertity(this object obj)
        {
            var properties = EntityCache.QueryEntity(obj.GetType()).Properties.Where(a => a.GetCustomAttribute<Identity>() != null).ToArray();

            if (!properties.Any())
                throw new DapperExtensionException($"the {nameof(obj)} entity with no KeyAttribute Propertity");

            if (properties.Length > 1)
                throw new DapperExtensionException($"the {nameof(obj)} entity with greater than one KeyAttribute Propertity");

            return properties.First();
        }
        public static PropertyInfo GetKeyPropertity(this Type typeInfo)
        {
            var properties = EntityCache.QueryEntity(typeInfo).Properties.Where(a => a.GetCustomAttribute<Identity>() != null).ToArray();

            if (!properties.Any())
                throw new DapperExtensionException($"the type {nameof(typeInfo.FullName)} entity with no KeyAttribute Propertity");

            if (properties.Length > 1)
                throw new DapperExtensionException($"the type {nameof(typeInfo.FullName)} entity with greater than one KeyAttribute Propertity");

            return properties.First();
        }
    }
}
