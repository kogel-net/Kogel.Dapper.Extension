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
		public static TFirst QueryFirstOrDefault<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
			where TSeventh : IBaseEntity
		{
			return cnn.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(provider, transaction).FirstOrDefault();
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
		public static async Task<TFirst> QueryFirstOrDefaultAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
			where TSeventh : IBaseEntity
		{
			return (await cnn.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(provider, transaction)).FirstOrDefault();
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
		public static IEnumerable<TFirst> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
			where TSeventh : IBaseEntity
		{
			EntityObject firstEntity = EntityCache.QueryEntity(typeof(TFirst));
			string splitOn = GetSplitOn<TFirst>(provider);
			if (!string.IsNullOrEmpty(splitOn))
			{
				//导航属性列表
				var navigationList = provider.JoinList.Where(x => x.Action == JoinAction.Navigation && x.IsMapperField).ToList();
				//把所有实体的信息存下做导航关联索引
				List<TFirst> firsts = new List<TFirst>();
				List<TSecond> seconds = new List<TSecond>();
				List<TThird> thirds = new List<TThird>();
				List<TFourth> fourths = new List<TFourth>();
				List<TFifth> fifths = new List<TFifth>();
				List<TSixth> sixths = new List<TSixth>();
				List<TSeventh> sevenths = new List<TSeventh>();
				cnn.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TFirst>(provider.SqlString, (first, second, third, fourth, fifth, sixth, seventh) =>
				{
					firsts.Add(first);
					if (second != null)
					{
						seconds.Add(second);
						if (third != null)
						{
							thirds.Add(third);
							if (fourth != null)
							{
								fourths.Add(fourth);
								if (fifth != null)
								{
									fifths.Add(fifth);
									if (sixth != null)
									{
										sixths.Add(sixth);
										if (seventh != null)
											sevenths.Add(seventh);
									}
								}
							}
						}
					}
					return default(TFirst);
				}, provider.Params, transaction, true, splitOn);
				//分割导航属性数据
				firsts = ExcisionData(firsts, seconds, thirds, fourths, fifths, sixths, sevenths, navigationList);
				return firsts;
			}
			else
			{
				return cnn.Query<TFirst>(provider.SqlString, provider.Params, transaction);
			}
		}

		/// <summary>
		/// 分割导航属性数据
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <typeparam name="TThird"></typeparam>
		/// <typeparam name="TFourth"></typeparam>
		/// <typeparam name="TFifth"></typeparam>
		/// <typeparam name="TSixth"></typeparam>
		/// <typeparam name="TSeventh"></typeparam>
		/// <param name="firsts"></param>
		/// <param name="seconds"></param>
		/// <param name="thirds"></param>
		/// <param name="fourths"></param>
		/// <param name="fifths"></param>
		/// <param name="sixths"></param>
		/// <param name="sevenths"></param>
		/// <param name="joinAssTables"></param>
		/// <returns></returns>
		private static List<TFirst> ExcisionData<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(List<TFirst> firsts, List<TSecond> seconds, List<TThird> thirds,
			List<TFourth> fourths, List<TFifth> fifths, List<TSixth> sixths, List<TSeventh> sevenths, List<JoinAssTable> joinAssTables)
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
			where TSeventh : IBaseEntity
		{
			List<TFirst> firstList = new List<TFirst>();
			List<TSecond> secondList = new List<TSecond>();
			List<TThird> thirdList = new List<TThird>();
			List<TFourth> fourthList = new List<TFourth>();
			List<TFifth> fifthList = new List<TFifth>();
			List<TSixth> sixthList = new List<TSixth>();
			List<TSeventh> seventhList = new List<TSeventh>();
			for (int i = 0; i < firsts.Count; i++)
			{
				TFirst first = firstList.Find(x => x.GetId().Equals(firsts[i].GetId()));
				if (first == null)
				{
					first = firsts[i];
					firstList.Add(first);
					//重置关联数据
					secondList.Clear();
					thirdList.Clear();
					fourthList.Clear();
					fifthList.Clear();
					sixthList.Clear();
					seventhList.Clear();
				}
				//设置第一个导航属性
				if (seconds.Count > i)
				{
					TSecond second = secondList.Find(x => x.GetId().Equals(seconds[i].GetId()));
					if (second == null)
					{
						second = seconds[i];
						secondList.Add(second);
						ExpressionExtension.SetProperValue(first, second, joinAssTables[0].PropertyInfo);
						//重置关联数据
						thirdList.Clear();
						fourthList.Clear();
						fifthList.Clear();
						sixthList.Clear();
						seventhList.Clear();
					}
					//设置第二个导航属性
					if (thirds.Count > i)
					{
						TThird third = thirdList.Find(x => x.GetId().Equals(thirds[i].GetId()));
						if (third == null)
						{
							third = thirds[i];
							thirdList.Add(third);
							//然后查找导航属性父级实体信息位置
							int parentIndex = joinAssTables.FindIndex(x => x.TableType == joinAssTables[1].PropertyInfo.DeclaringType) + 1;
							if (parentIndex == 0)
								ExpressionExtension.SetProperValue(first, third, joinAssTables[1].PropertyInfo);
							else if (parentIndex == 1)
								ExpressionExtension.SetProperValue(second, third, joinAssTables[1].PropertyInfo);
							//重置关联数据
							fourthList.Clear();
							fifthList.Clear();
							sixthList.Clear();
							seventhList.Clear();
						}
						//设置第三个导航属性
						if (fourths.Count > i)
						{
							TFourth fourth = fourthList.Find(x => x.GetId().Equals(fourths[i].GetId()));
							if (fourth == null)
							{
								fourth = fourths[i];
								fourthList.Add(fourth);
								//然后查找导航属性父级实体信息位置
								int parentIndex = joinAssTables.FindIndex(x => x.TableType == joinAssTables[2].PropertyInfo.DeclaringType) + 1;
								if (parentIndex == 0)
									ExpressionExtension.SetProperValue(first, fourth, joinAssTables[2].PropertyInfo);
								else if (parentIndex == 1)
									ExpressionExtension.SetProperValue(second, fourth, joinAssTables[2].PropertyInfo);
								else if (parentIndex == 2)
									ExpressionExtension.SetProperValue(third, fourth, joinAssTables[2].PropertyInfo);
								//重置关联数据
								fifthList.Clear();
								sixthList.Clear();
								seventhList.Clear();
							}
							//设置第四个导航属性
							if (fifths.Count > i)
							{
								TFifth fifth = fifthList.Find(x => x.GetId().Equals(fifths[i].GetId()));
								if (fifth == null)
								{
									fifth = fifths[i];
									fifthList.Add(fifth);
									//然后查找导航属性父级实体信息位置
									int parentIndex = joinAssTables.FindIndex(x => x.TableType == joinAssTables[3].PropertyInfo.DeclaringType) + 1;
									if (parentIndex == 0)
										ExpressionExtension.SetProperValue(first, fifth, joinAssTables[3].PropertyInfo);
									else if (parentIndex == 1)
										ExpressionExtension.SetProperValue(second, fifth, joinAssTables[3].PropertyInfo);
									else if (parentIndex == 2)
										ExpressionExtension.SetProperValue(third, fifth, joinAssTables[3].PropertyInfo);
									else if (parentIndex == 3)
										ExpressionExtension.SetProperValue(fourth, fifth, joinAssTables[3].PropertyInfo);
									//重置关联数据
									sixthList.Clear();
									seventhList.Clear();
								}
								//设置第五个导航属性
								if (sixths.Count > i)
								{
									TSixth sixth = sixthList.Find(x => x.GetId().Equals(sixths[i].GetId()));
									if (sixth == null)
									{
										sixth = sixths[i];
										sixthList.Add(sixth);
										//然后查找导航属性父级实体信息位置
										int parentIndex = joinAssTables.FindIndex(x => x.TableType == joinAssTables[4].PropertyInfo.DeclaringType) + 1;
										if (parentIndex == 0)
											ExpressionExtension.SetProperValue(first, sixth, joinAssTables[4].PropertyInfo);
										else if (parentIndex == 1)
											ExpressionExtension.SetProperValue(second, sixth, joinAssTables[4].PropertyInfo);
										else if (parentIndex == 2)
											ExpressionExtension.SetProperValue(third, sixth, joinAssTables[4].PropertyInfo);
										else if (parentIndex == 3)
											ExpressionExtension.SetProperValue(fourth, sixth, joinAssTables[4].PropertyInfo);
										else if (parentIndex == 4)
											ExpressionExtension.SetProperValue(fifth, sixth, joinAssTables[4].PropertyInfo);
										//重置关联数据
										seventhList.Clear();
									}
									//设置第六个导航属性
									if (sevenths.Count > i)
									{
										TSeventh seventh = seventhList.Find(x => x.GetId().Equals(sevenths[i].GetId()));
										if (seventh == null)
										{
											seventh = sevenths[i];
											seventhList.Add(seventh);
											//然后查找导航属性父级实体信息位置
											int parentIndex = joinAssTables.FindIndex(x => x.TableType == joinAssTables[5].PropertyInfo.DeclaringType) + 1;
											if (parentIndex == 0)
												ExpressionExtension.SetProperValue(first, seventh, joinAssTables[5].PropertyInfo);
											else if (parentIndex == 1)
												ExpressionExtension.SetProperValue(second, seventh, joinAssTables[5].PropertyInfo);
											else if (parentIndex == 2)
												ExpressionExtension.SetProperValue(third, seventh, joinAssTables[5].PropertyInfo);
											else if (parentIndex == 3)
												ExpressionExtension.SetProperValue(fourth, seventh, joinAssTables[5].PropertyInfo);
											else if (parentIndex == 4)
												ExpressionExtension.SetProperValue(fifth, seventh, joinAssTables[5].PropertyInfo);
											else if (parentIndex == 5)
												ExpressionExtension.SetProperValue(sixth, seventh, joinAssTables[5].PropertyInfo);
										}
									}
								}
							}
						}
					}
				}
			}
			return firstList;
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
		public static async Task<IEnumerable<TFirst>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this IDbConnection cnn, SqlProvider provider, IDbTransaction transaction = null)
			where TFirst : IBaseEntity
			where TSecond : IBaseEntity
			where TThird : IBaseEntity
			where TFourth : IBaseEntity
			where TFifth : IBaseEntity
			where TSixth : IBaseEntity
			where TSeventh : IBaseEntity
		{
			EntityObject firstEntity = EntityCache.QueryEntity(typeof(TFirst));
			string splitOn = GetSplitOn<TFirst>(provider);
			if (!string.IsNullOrEmpty(splitOn))
			{
				//导航属性列表
				var navigationList = provider.JoinList.Where(x => x.Action == JoinAction.Navigation && x.IsMapperField).ToList();
				//把所有实体的信息存下做导航关联索引
				List<TFirst> firsts = new List<TFirst>();
				List<TSecond> seconds = new List<TSecond>();
				List<TThird> thirds = new List<TThird>();
				List<TFourth> fourths = new List<TFourth>();
				List<TFifth> fifths = new List<TFifth>();
				List<TSixth> sixths = new List<TSixth>();
				List<TSeventh> sevenths = new List<TSeventh>();
				await cnn.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TFirst>(provider.SqlString, (first, second, third, fourth, fifth, sixth, seventh) =>
				{
					firsts.Add(first);
					if (second != null)
					{
						seconds.Add(second);
						if (third != null)
						{
							thirds.Add(third);
							if (fourth != null)
							{
								fourths.Add(fourth);
								if (fifth != null)
								{
									fifths.Add(fifth);
									if (sixth != null)
									{
										sixths.Add(sixth);
										if (seventh != null)
											sevenths.Add(seventh);
									}
								}
							}
						}
					}
					return default(TFirst);
				}, provider.Params, transaction, true, splitOn);
				//分割导航属性数据
				firsts = ExcisionData(firsts, seconds, thirds, fourths, fifths, sixths, sevenths, navigationList);
				return firsts;
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

	public class GuidTypeHanlder : SqlMapper.TypeHandler<Guid>
	{
		public override void SetValue(IDbDataParameter parameter, Guid value)
		{
			parameter.Size = 10;
			parameter.DbType = DbType.String;
			parameter.Value = value.ToString();
		}

		public override Guid Parse(object value)
		{
			return Guid.Parse((string)value);
		}
	}

	public class GuidArrTypeHanlder : SqlMapper.TypeHandler<Guid[]>
	{
		public override void SetValue(IDbDataParameter parameter, Guid[] value)
		{
			parameter.Size = 10;
			parameter.DbType = DbType.String;
			parameter.Value = value.ToString();
		}

		public override Guid[] Parse(object value)
		{
			List<Guid> guids = new List<Guid>();
			foreach (var item in value as string[])
			{
				guids.Add(Guid.Parse((string)item));
			}
			return guids.ToArray();
		}
	}
}