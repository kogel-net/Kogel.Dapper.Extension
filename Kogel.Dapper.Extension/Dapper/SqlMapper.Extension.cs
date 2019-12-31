using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Extension;
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
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
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
		    where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
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
		    where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
		{
			EntityObject firstEntity = EntityCache.QueryEntity(typeof(TFirst));
			string splitOn = GetSplitOn<TFirst>(provider);
			if (!string.IsNullOrEmpty(splitOn))
			{
				var hashes = new HashSet<TFirst>();
				cnn.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TFirst>(provider.SqlString, (first, second, third, fourth, fifth, sixth) =>
				{
					object id = first.GetId();
					//判断当前主数据是否出现过
					var lookup = hashes.FirstOrDefault(x => x.GetId().Equals(id));
					if (lookup == null)
						lookup = first;

					//设置导航属性对应
					if (second != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, second);
					if (third != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, third);
					if (fourth != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, fourth);
					if (fifth != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, fifth);
					if (sixth != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, sixth);

					//不存在的主表数据才添加进来，防止重复
					if (!hashes.Any(x => x.GetId() == lookup.GetId()))
						hashes.Add(lookup);

					return default(TFirst);
				}, provider.Params, transaction, true, splitOn);

				return hashes;
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
		public static async Task<IEnumerable<TFirst>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
		{
			EntityObject firstEntity = EntityCache.QueryEntity(typeof(TFirst));
			string splitOn = GetSplitOn<TFirst>(provider);
			if (!string.IsNullOrEmpty(splitOn))
			{
				var hashes = new HashSet<TFirst>();
				await cnn.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TFirst>(provider.SqlString, (first, second, third, fourth, fifth, sixth) =>
				{
					object id = first.GetId();
					//判断当前主数据是否出现过
					var lookup = hashes.FirstOrDefault(x => x.GetId().Equals(id));
					if (lookup == null)
						lookup = first;

					//设置导航属性对应
					if (second != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, second);
					if (third != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, third);
					if (fourth != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, fourth);
					if (fifth != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, fifth);
					if (sixth != null)
						ExpressionExtension.SetProperValue(firstEntity, lookup, sixth);

					//不存在的主表数据才添加进来，防止重复
					if (!hashes.Any(x => x.GetId() == id))
						hashes.Add(lookup);

					return default(TFirst);
				}, provider.Params, transaction, true, splitOn);
				return hashes;
			}
			else
			{
				return await cnn.QueryAsync<TFirst>(provider.SqlString, provider.Params, transaction);
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