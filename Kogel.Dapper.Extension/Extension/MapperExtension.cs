using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Expressions;

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
		public static T QueryFirst_1<T>(this IDbConnection conn, string sql, IProviderOption providerOption, object param = null, IDbTransaction transaction = null)
		{
			return QueryRowImpl<T>(conn, sql, param, transaction).FirstOrDefault().SetNavigation(conn, providerOption);
		}
		public static List<T> Query_1<T>(this IDbConnection conn, string sql, IProviderOption providerOption, object param = null, IDbTransaction transaction = null)
		{
			return QueryRowImpl<T>(conn, sql, param, transaction).SetNavigationList(conn, providerOption);
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
			Type type = typeof(T);
			ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance
			| BindingFlags.NonPublic
			| BindingFlags.Public);
			ConstructorInfo noParameterConstructorInfo = constructorInfoArray.FirstOrDefault(x => x.GetParameters().Length == 0);
			if (null == noParameterConstructorInfo && type.FullName.Contains("AnonymousType"))//匿名类型
			{
				using (var reader = conn.ExecuteReader(sql, param, transaction))
				{
					data = reader.Parse<T>().ToList();
				}
			}
			else
			{
				data = conn.Query<T>(sql, param, transaction).ToList();
			}
			return data;
		}
		#endregion
		#region 导航拓展

		/// <summary>
		/// 写入导航属性到实体(单条)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="dbCon"></param>
		/// <returns></returns>
		private static T SetNavigation<T>(this T data, IDbConnection dbCon, IProviderOption providerOption)
		{
			foreach (var navigation in providerOption.NavigationList)
			{
				var navigationExpression = new NavigationExpression(navigation.MemberAssign.Expression);
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
				typeof(MapperExtension).GetMethod("SetValue")
					.MakeGenericMethod(new Type[] { typeof(T), navigationExpression.ReturnType })
					.Invoke(null, new object[] { data, dbCon, navigationExpression.SqlCmd, navigationExpression.Param, navigation.MemberAssignName });
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
			property.SetValue(data, navigationData);
		}
		/// <summary>
		/// 写入导航属性到实体(列表)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="DbCon"></param>
		/// <returns></returns>
		private static List<T> SetNavigationList<T>(this List<T> data, IDbConnection DbCon, IProviderOption providerOption)
		{
			if (providerOption.NavigationList.Any())
			{
				//暂时用循环（可能性能有点差,有更好的解决办法可以一起交流）
				foreach (var item in data)
				{
					item.SetNavigation(DbCon, providerOption);
				}
			}
			return data;
		}
		
		#endregion
	}
}
