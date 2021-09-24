using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Entites;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kogel.Dapper.Extension
{
    /// <summary>
    /// 实体类缓存
    /// </summary>
    public class EntityCache
    {
        internal static ConcurrentBag<EntityObject> EntitieList = new ConcurrentBag<EntityObject>();
        /// <summary>
        /// 注册动态化查询可能会用到的实体类
        /// </summary>
        /// <param name="entity">实体类</param>
        public static EntityObject Register(Type entity)
        {
            EntityObject entityObject = new EntityObject(entity);
            if (!EntitieList.Any(x => x.AssemblyString.Equals(entityObject.AssemblyString)))
            {
                SqlMapper.SetTypeMap(entityObject.Type, new CustomPropertyTypeMap(entityObject.Type,
                    (type, column) =>
                    type.GetPropertys(entityObject.FieldPairs.FirstOrDefault(x => x.Value.Equals(column)).Key)
                    ));
                EntitieList.Add(entityObject);
            }
            return entityObject;
        }

        /// <summary>
        /// 注册动态化查询可能会用到的实体类
        /// </summary>
        /// <param name="entitys">实体类</param>
        public static void Register(Type[] entitys)
        {
            foreach (var item in entitys)
            {
                Register(item);
            }
        }

        /// <summary>
        /// 注册动态化查询可能会用到的实体类
        /// </summary>
        /// <param name="assemblyString">通过给定程序集的长格式名称加载程序集。</param>
        public static void Register(string assemblyString)
        {
            Assembly assembly = Assembly.Load(assemblyString);
            Register(assembly.GetTypes());
        }

        /// <summary>
        /// 查询实体类信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static EntityObject QueryEntity(Type entity)
        {
            var entityType = EntitieList.FirstOrDefault(x => x.Type.FullName.Equals(entity.FullName));
            if (entityType != null)
            {
                return entityType;
            }
            else
            {
                return Register(entity);
            }
        }

        /// <summary>
        /// 查询实体类信息（模糊查询）
        /// </summary>
        /// <param name="entityFullName"></param>
        /// <returns></returns>
        public static EntityObject QueryEntity(string entityFullName)
        {
            var entityType = EntitieList.FirstOrDefault(x => x.Type.FullName.Contains(entityFullName));
            return entityType;
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns></returns>
        public static List<EntityObject> GetEntities()
        {
            return EntitieList.Where(x => ExpressionExtension.IsAnyBaseEntity(x.Type, out Type entityType)).Distinct().ToList();
        }
    }
}
