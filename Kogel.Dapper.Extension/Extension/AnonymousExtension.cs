using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Extension
{
    /// <summary>
    /// 匿名类解析
    /// </summary>
    public static class AnonymousExtension
    {
        #region 匿名类返回
        /// <summary>
        /// 只用来查询返回匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static T QueryFirst_1<T>(this IDbConnection conn, SqlProvider provider, IDbTransaction transaction = null)
        {
            return QueryRowImpl<T>(conn, provider, transaction).FirstOrDefault().SetNavigation(conn, provider.ProviderOption);
        }

        public static List<T> Query_1<T>(this IDbConnection conn, SqlProvider provider, IDbTransaction transaction = null)
        {
            return QueryRowImpl<T>(conn, provider, transaction).SetNavigationList(conn, provider.ProviderOption);
        }

        public static async Task<T> QueryFirst_1Async<T>(this IDbConnection conn, SqlProvider provider, IDbTransaction transaction = null)
        {
            List<T> queryRow = await QueryRowImplAsync<T>(conn, provider, transaction);
            return queryRow.FirstOrDefault().SetNavigation(conn, provider.ProviderOption);
        }

        public static async Task<List<T>> Query_1Async<T>(this IDbConnection conn, SqlProvider provider, IDbTransaction transaction = null)
        {
            List<T> queryRow = await QueryRowImplAsync<T>(conn, provider, transaction);
            return queryRow.SetNavigationList(conn, provider.ProviderOption);
        }

        /// <summary>
        /// 查询返回匿名类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="provider"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static List<T> QueryRowImpl<T>(IDbConnection conn, SqlProvider provider, IDbTransaction transaction = null)
        {
            List<T> data;
            Type type = typeof(T);
            if (type.FullName.Contains("AnonymousType"))//匿名类型
            {
                ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                ConstructorInfo noParameterConstructorInfo = constructorInfoArray.FirstOrDefault();
                data = new List<T>();
                using (var reader = conn.ExecuteReader(provider.SqlString, provider.Params, transaction))
                {
                    var properties = EntityCache.QueryEntity(type).Properties;
                    while (reader.Read())
                    {
                        object[] array = new object[properties.Length];
                        for (var i = 0; i < properties.Length; i++)
                        {
                            var item = properties[i];
                            var value = reader[item.Name];
                            if (value != DBNull.Value)
                                value = Convert.ChangeType(value, item.PropertyType.GetAnonymousFieldType());
                            else
                                value = default;
                            array[i] = value;
                        }
                        T t = (T)noParameterConstructorInfo.Invoke(array);
                        data.Add(t);
                    }
                }
            }
            else
            {
                data = conn.Querys<T>(provider, transaction).ToList();
            }
            return data;
        }

        /// <summary>
        /// 查询返回匿名类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="provider"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static async Task<List<T>> QueryRowImplAsync<T>(IDbConnection conn, SqlProvider provider, IDbTransaction transaction = null)
        {
            List<T> data;
            Type type = typeof(T);
            if (type.FullName.Contains("AnonymousType"))//匿名类型
            {
                ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                ConstructorInfo noParameterConstructorInfo = constructorInfoArray.FirstOrDefault();
                data = new List<T>();
                using (var reader = await conn.ExecuteReaderAsync(provider.SqlString, provider.Params, transaction))
                {
                    var properties = EntityCache.QueryEntity(type).Properties;
                    while (reader.Read())
                    {
                        object[] array = new object[properties.Length];
                        for (var i = 0; i < properties.Length; i++)
                        {
                            var item = properties[i];
                            var value = reader[item.Name];
                            if (value != DBNull.Value)
                                value = Convert.ChangeType(value, item.PropertyType.GetAnonymousFieldType());
                            else
                                value = default;
                            array[i] = value;
                        }
                        T t = (T)noParameterConstructorInfo.Invoke(array);
                        data.Add(t);
                    }
                }
            }
            else
            {
                data = (await conn.QueryAsyncs<T>(provider, transaction)).ToList();
            }
            return data;
        }
        #endregion

        #region 导航拓展
        private static readonly MethodInfo _SetValueMethod = typeof(AnonymousExtension).GetMethod("SetValue");
        /// <summary>
        /// 写入导航属性到实体(单条)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dbCon"></param>
        /// <returns></returns>
        private static T SetNavigation<T>(this T data, IDbConnection dbCon, IProviderOption providerOption)
        {
            if (providerOption.NavigationList.Any() && data != null)
            {
                foreach (var navigation in providerOption.NavigationList)
                {
                    var navigationExpression = new NavigationExpression(navigation.MemberAssign.Expression);
                    if (providerOption.MappingList.Any())
                    {
                        //根据得知的映射列表获取条件值
                        foreach (var mapp in providerOption.MappingList)
                        {
                            //根据映射的键设置实际的值
                            if (navigationExpression.SqlCmd.IndexOf($"= {mapp.Key}") != -1)
                            {
                                string param = $"{providerOption.ParameterPrefix}{mapp.Value}";
                                navigationExpression.SqlCmd = navigationExpression.SqlCmd.Replace($"= {mapp.Key}", $"= {param}");
                                //获取实体类的值
                                object paramValue = EntityCache.QueryEntity(typeof(T)).Properties.FirstOrDefault(x => x.Name == mapp.Value).GetValue(data);
                                navigationExpression.Param.Add(param, paramValue);
                            }
                        }
                        _SetValueMethod
                            .MakeGenericMethod(new Type[] { typeof(T), navigationExpression.ReturnType })
                            .Invoke(null, new object[] { data, dbCon, navigationExpression.SqlCmd, navigationExpression.Param, navigation.MemberAssignName });
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 执行对象并写入值到对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="data"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="memberName"></param>
        public static void SetValue<T, T1>(T data, IDbConnection dbCon, string sql, DynamicParameters param, string memberName)
        {
            //执行sql
            var navigationData = dbCon.Query<T1>(sql, param);
            PropertyInfo property = EntityCache.QueryEntity(typeof(T)).Properties.FirstOrDefault(x => x.Name.Equals(memberName));
            if (property.PropertyType.FullName.Contains("System.Collections.Generic"))
                property.SetValue(data, navigationData);
            else
            {
                var firstData = navigationData.FirstOrDefault();
                if (firstData != null)
                    property.SetValue(data, firstData);
            }
        }

        private static readonly MethodInfo _SetListValueMethod = typeof(AnonymousExtension).GetMethod("SetListValue");
        /// <summary>
        /// 写入导航属性到实体(列表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="DbCon"></param>
        /// <returns></returns>
        private static List<T> SetNavigationList<T>(this List<T> data, IDbConnection dbCon, IProviderOption providerOption)
        {
            if (providerOption.NavigationList.Any() && data != null && data.Any())
            {
                foreach (var navigation in providerOption.NavigationList)
                {
                    StringBuilder sqlBuilder = new StringBuilder();
                    var navigationExpression = new NavigationExpression(navigation.MemberAssign.Expression);
                    //参数数
                    int paramCount = 0;
                    if (providerOption.MappingList.Any())
                    {
                        //var whereSql = navigationExpression.SqlCmd.Substring(navigationExpression.SqlCmd.IndexOf("WHERE"));
                        //根据得知的映射列表获取条件值
                        foreach (var mapp in providerOption.MappingList)
                        {
                            //根据映射的键设置实际的值
                            if (navigationExpression.SqlCmd.IndexOf($"= {mapp.Key}") != -1)
                            {
                                foreach (var item in data)
                                {
                                    string param = $"{providerOption.ParameterPrefix}{mapp.Value}_{paramCount++}";
                                    sqlBuilder.Append(navigationExpression.SqlCmd.Replace($"= {mapp.Key}", $"= {param}") + $";{Environment.NewLine}");
                                    //获取实体类的值
                                    object paramValue = EntityCache.QueryEntity(typeof(T)).Properties.FirstOrDefault(x => x.Name == mapp.Value).GetValue(item);
                                    navigationExpression.Param.Add(param, paramValue);
                                }
                            }
                        }
                        _SetListValueMethod
                           .MakeGenericMethod(new Type[] { typeof(T), navigationExpression.ReturnType })
                           .Invoke(null, new object[] { data, dbCon, sqlBuilder.ToString(), navigationExpression.Param,
                            navigation.MemberAssignName });
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 执行对象并写入值到对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="data"></param>
        /// <param name="dbCon"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="memberName"></param>
        public static void SetListValue<T, T1>(List<T> data, IDbConnection dbCon, string sql, DynamicParameters param, string memberName)
        {
            //得到需要分配值得对象
            PropertyInfo property = EntityCache.QueryEntity(typeof(T)).Properties.FirstOrDefault(x => x.Name.Equals(memberName));
            //执行sql
            var mapperGridList = dbCon.QueryMultiple(sql, param);
            //根据list关联的条件把值分配回去
            //ToList
            if (property.PropertyType.FullName.Contains("System.Collections.Generic.List"))
            {
                foreach (var item in data)
                {
                    var mapperDataList = mapperGridList.Read<T1>();
                    property.SetValue(item, mapperDataList);
                }
            }
            else
            {
                //Get
                foreach (var item in data)
                {
                    var mapperData = mapperGridList.ReadSingleOrDefault<T1>();
                    property.SetValue(item, mapperData);
                }
            }
            #endregion
        }
    }
}