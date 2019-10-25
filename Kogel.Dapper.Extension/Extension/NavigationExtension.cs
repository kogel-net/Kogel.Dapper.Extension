using Dapper;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Kogel.Dapper.Extension.Extension
{
	/// <summary>
	/// 导航属性扩展
	/// </summary>
	public static class NavigationExtension
	{
		/// <summary>
		/// 单个返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static T QueryFirstOrDefaults<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryFirstOrDefault<T>(sql, param, transaction).SetNavigation(DbCon);
		}
		/// <summary>
		/// 单个异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<T> QueryFirstOrDefaultAsyncs<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
		}
		/// <summary>
		/// 列表返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static IEnumerable<T> Querys<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.Query<T>(sql, param, transaction).SetNavigationList(DbCon);
		}
		/// <summary>
		/// 列表异步返回
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="DbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public static Task<IEnumerable<T>> QueryAsyncs<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryAsync<T>(sql, param, transaction);
		}


		public static GridReader QueryMultiples<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QueryMultiple(sql, param, transaction);
		}

		public static T QuerySingles<T>(this IDbConnection DbCon, string sql, object param, IDbTransaction transaction)
		{
			return DbCon.QuerySingle<T>(sql, param, transaction);
		}
		/// <summary>
		/// 写入导航属性到实体(单条)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="DbCon"></param>
		/// <returns></returns>
		private static T SetNavigation<T>(this T data, IDbConnection DbCon)
		{
			//当前实体的类型
			var entity = EntityCache.QueryEntity(typeof(T));
			if (entity.Navigations.Any())
			{
				//当前实体反射的属性
				var properties = data.GetType().GetProperties();
				//循环设置导航属性
				foreach (var navigation in entity.Navigations)
				{
					//获取当前关联字段的值
					object value = properties.FirstOrDefault(x => x.Name == navigation.CurrentAssoField).GetValue(data);
					DynamicParameters param = new DynamicParameters();
					param.Add(navigation.JoinAssoField, value);
					//sql
					string sql = $"SELECT * FROM {EntityCache.QueryEntity(navigation.JsonAssoTable).Name} WHERE {navigation.JoinAssoField}=@{navigation.JoinAssoField}";
					//设置导航属性的值
					var property = properties.FirstOrDefault(x => x.Name == navigation.AssoField);
					//反射写入实际值
					typeof(NavigationExtension).GetMethod("Set").MakeGenericMethod(navigation.JsonAssoTable).Invoke(null,
						new object[]
						{
							data,DbCon,sql,param,property,navigation.NavigationType
						});
				}
			}
			return data;
		}
		/// <summary>
		/// 写入导航属性到实体(列表)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="DbCon"></param>
		/// <returns></returns>
		private static IEnumerable<T> SetNavigationList<T>(this IEnumerable<T> data, IDbConnection DbCon)
		{
			//当前实体的类型
			var entity = EntityCache.QueryEntity(typeof(T));
			if (entity.Navigations.Any())
			{
				//暂时用循环（可能性能有点差,有更好的解决办法可以一起交流）
				foreach (var item in data)
				{
					item.SetNavigation(DbCon);
				}
			}
			return data;
		}
		/// <summary>
		/// 写入导航属性的值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="dbCon"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="property"></param>
		/// <param name="navigationEnum"></param>
		public static void Set<T>(object data, IDbConnection dbCon, string sql, DynamicParameters param, PropertyInfo property, NavigationEnum navigationEnum)
		{
			//property.SetValue(data,)
			if (navigationEnum == NavigationEnum.List)
			{
				var naviData = dbCon.Query<T>(sql, param);
				property.SetValue(data, naviData);
			}
			else
			{
				var naviData = dbCon.QueryFirstOrDefault<T>(sql, param);
				property.SetValue(data, naviData);
			}
		}
	}
}
