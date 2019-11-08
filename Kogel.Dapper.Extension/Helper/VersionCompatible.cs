using System;
using System.Reflection;
using System.Linq;

namespace Kogel.Dapper.Extension.Helper
{
    /// <summary>
    /// [已弃用]版本兼容类(兼容.netstandard1.3的语法)
    /// </summary>
    public static class VersionCompatible
    {
        public static Type BaseTypes(this Type PropertyType)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().BaseType;
#else
            return PropertyType.BaseType;
#endif
        }
        public static bool IsGenericTypes(this Type PropertyType)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().IsGenericType;
#else
            return PropertyType.IsGenericType;
#endif
        }
        public static bool IsEnums(this Type PropertyType)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().IsEnum;
#else
            return PropertyType.IsEnum;
#endif
        }
        public static bool IsValueTypes(this Type PropertyType)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().IsValueType;
#else
            return PropertyType.IsValueType;
#endif
        }
        public static PropertyInfo GetPropertys(this Type PropertyType, string name)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().GetDeclaredProperty(name);
#else
            if (string.IsNullOrEmpty(name)) { return null; }
            return PropertyType.GetProperty(name);
#endif
        }
        public static MethodInfo GetMethodInfos(this Type PropertyType,string name)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().GetDeclaredMethod(name);
#else
            if (string.IsNullOrEmpty(name)) { return null; }
            return PropertyType.GetMethod(name);
#endif
        }
        public static MethodInfo[] GetMethodss(this Type PropertyType)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().DeclaredMethods.ToArray();
#else
            return PropertyType.GetMethods();
#endif
        }
        /// <summary>
        /// 当在派生类中重写，则返回的所有自定义特性应用于此成员的数组。
        /// </summary>
        /// <param name="PropertyType"></param>
        /// <param name="inherit">true 搜索此成员继承链，以查找这些属性;否则为 false。 属性和事件，则忽略此参数请参阅备注。</param>
        /// <returns></returns>
        public static object[] GetCustomAttributess(this Type PropertyType,bool inherit)
        {
#if NETSTANDARD1_3
           return PropertyType.GetTypeInfo().GetCustomAttributes().ToArray<object>();
#else
            return PropertyType.GetCustomAttributes(inherit);
#endif
        }
        public static object [] GetCustomAttributess(this PropertyInfo PropertyInfo,bool inherit)
        {
#if NETSTANDARD1_3
           return PropertyInfo.GetType().GetTypeInfo().GetCustomAttributes().ToArray<object>();
#else
            return PropertyInfo.GetCustomAttributes(inherit);
#endif
        }
        public static Type ReflectedTypes(this MethodInfo method)
        {
#if NETSTANDARD1_3
           return method.ReturnType;
#else
            return method.ReflectedType;
#endif
        }

        public static PropertyInfo[] GetPropertiess(this Type PropertyType)
        {
#if NETSTANDARD1_3
           return PropertyType.GetProperties();
#else
            return PropertyType.GetProperties();
#endif
        }
    }
}
