using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Kogel.Dapper.Extension.Extension
{
    public static class MapperExtension
    {
        #region 匿名类返回
        /// <summary>
        /// 只用来查询返回匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static T QueryFirst_1<T>(this IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null)
        {
            return QueryRowImpl<T>(conn, sql, param, transaction).FirstOrDefault();
        }
        public static List<T> Query_1<T>(this IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null)
        {
            return QueryRowImpl<T>(conn, sql, param, transaction);
        }
        /// <summary>
        /// 查询返回匿名类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static List<T> QueryRowImpl<T>(IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null)
        {
            List<T> data = new List<T>();
            //创建匿名类
            T t = default(T);
            Type type = typeof(T);
            ConstructorInfo[] constructorInfoArray = type.GetConstructors(System.Reflection.BindingFlags.Instance
            | BindingFlags.NonPublic
            | BindingFlags.Public);
            ConstructorInfo noParameterConstructorInfo = constructorInfoArray.FirstOrDefault(x => x.GetParameters().Length == 0);
            if (null == noParameterConstructorInfo && type.FullName.Contains("AnonymousType"))//匿名类型
            {
                noParameterConstructorInfo = constructorInfoArray.FirstOrDefault();
                using (var reader = conn.ExecuteReader(sql, param, transaction))
                {
                    var properties = EntityCache.QueryEntity(type).Properties;
                    while (reader.Read())
                    {
                        object[] array = new object[properties.Length];
                        for (var i = 0; i < properties.Length; i++)
                        {
                            var item = properties[i];
                            array[i] = Convert.ChangeType(reader[item.Name], item.PropertyType);
                        }
                        t = (T)noParameterConstructorInfo.Invoke(array);
                        data.Add(t);
                    }
                }
            }
            else
            {
                data = conn.Query<T>(sql, param, transaction).ToList();
            }
            return data;
        }
        #endregion
    }
}
