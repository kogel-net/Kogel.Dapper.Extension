using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper
{
	public static class SqlMapperExtension
	{
		/// <summary>
		/// 获取拆分字段
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static string GetSplitOn<T>(SqlProvider provider)
		{
			string splitOn = string.Empty;
			var navigationList = provider.JoinList.Where(x => x.Action == JoinAction.Navigation).ToList();
			if (navigationList.Any())
			{
				splitOn = string.Join(",", navigationList.Select(x => x.FirstFieldName));
			}
			return splitOn;
		}

		/// <summary>
		/// 查询带聚合导航属性的对象集合
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <param name="cnn"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <param name="splitOn"></param>
		/// <returns></returns>
		public static TFirst QueryFirstOrDefault<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
		{
			return cnn.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(provider, transaction).FirstOrDefault();
		}

		/// <summary>
		/// 异步查询带聚合导航属性的对象集合
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <param name="cnn"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <param name="splitOn"></param>
		/// <returns></returns>
		public static async Task<TFirst> QueryFirstOrDefaultAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
		{
			return (await cnn.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(provider, transaction)).FirstOrDefault();
		}

		/// <summary>
		/// 查询带聚合导航属性的对象集合
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <param name="cnn"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <param name="splitOn"></param>
		/// <returns></returns>
		public static IEnumerable<TFirst> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
		{
			EntityObject firstEntity = EntityCache.QueryEntity(typeof(TFirst));
			string splitOn = GetSplitOn<TFirst>(provider);
			if (!string.IsNullOrEmpty(splitOn))
			{
				return cnn.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TFirst>(provider.SqlString, (first, second, third, fourth, fifth, sixth) =>
				{
				//设置导航属性对应
				var secondProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TSecond) || x.PropertyType.GenericTypeArguments.Any(y => y == typeof(TSecond)));
					if (secondProper != null)
					{
						secondProper.SetValue(first, second);
					}
					var thirdProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TThird));
					if (thirdProper != null)
					{
						thirdProper.SetValue(first, third);
					}
					var fourthProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TFourth));
					if (fourthProper != null)
					{
						fourthProper.SetValue(first, fourth);
					}
					var fifthProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TFifth));
					if (fifthProper != null)
					{
						fifthProper.SetValue(first, fifth);
					}
					var sixthProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TSixth));
					if (sixthProper != null)
					{
						sixthProper.SetValue(first, sixth);
					}
					return first;
				}, provider.Params, transaction, true, splitOn);
			}
			else
			{
				return cnn.Query<TFirst>(provider.SqlString, provider.Params, transaction);
			}
		}

		/// <summary>
		/// 异步查询带聚合导航属性的对象集合
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <param name="cnn"></param>
		/// <param name="provider"></param>
		/// <param name="transaction"></param>
		/// <param name="splitOn"></param>
		/// <returns></returns>
		public static Task<IEnumerable<TFirst>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
		{
			EntityObject firstEntity = EntityCache.QueryEntity(typeof(TFirst));
			string splitOn = GetSplitOn<TFirst>(provider);
			if (!string.IsNullOrEmpty(splitOn))
			{
				return cnn.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TFirst>(provider.SqlString, (first, second, third, fourth, fifth, sixth) =>
				{
					//设置导航属性对应
					var secondProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TSecond) || x.PropertyType.GenericTypeArguments.Any(y => y == typeof(TSecond)));
					if (secondProper != null)
					{
						secondProper.SetValue(first, second);
					}
					var thirdProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TThird));
					if (thirdProper != null)
					{
						thirdProper.SetValue(first, third);
					}
					var fourthProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TFourth));
					if (fourthProper != null)
					{
						fourthProper.SetValue(first, fourth);
					}
					var fifthProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TFifth));
					if (fifthProper != null)
					{
						fifthProper.SetValue(first, fifth);
					}
					var sixthProper = firstEntity.Properties.FirstOrDefault(x => x.PropertyType == typeof(TSixth));
					if (sixthProper != null)
					{
						sixthProper.SetValue(first, sixth);
					}
					return first;
				}, provider.Params, transaction, true, splitOn);
			}
			else
			{
				return cnn.QueryAsync<TFirst>(provider.SqlString, provider.Params, transaction);
			}
		}
		/// <summary>
		/// 查询返回DataSet
		/// </summary>
		/// <param name="cnn"></param>
		/// <param name="adapter"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <param name="buffered"></param>
		/// <param name="commandTimeout"></param>
		/// <param name="commandType"></param>
		/// <returns></returns>
		public static DataSet QueryDataSet(this IDbConnection cnn, IDbDataAdapter adapter, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			var ds = new DataSet();
			var command = new CommandDefinition(cnn, sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
			var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param == null ? null : param.GetType(), null);
			var info = SqlMapper.GetCacheInfo(identity, param, command.AddToCache);
			bool wasClosed = cnn.State == ConnectionState.Closed;
			if (wasClosed) cnn.Open();
			adapter.SelectCommand = command.SetupCommand(cnn, info.ParamReader);
			adapter.Fill(ds);
			if (wasClosed) cnn.Close();
			return ds;
		}
	}
}