using Kogel.Dapper.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	public partial class SqlMapper
	{
		///// <summary>
		///// 查询带聚合导航属性的对象集合
		///// </summary>
		///// <typeparam name="TFirst">主体对象类型</typeparam>
		///// <typeparam name="TSecond">聚合导航对象类型</typeparam>
		///// <param name="setting">设置聚合导航属性的方法</param>
		//public static IEnumerable<TFirst> Query<TFirst, TSecond>(this IDbConnection cnn, string sql, Action<TFirst, TSecond> setting, object param = null, string splitOn = "Id")
		//	where TFirst : IBaseEntity<int>
		//	where TSecond : IBaseEntity<int>
		//{
		//	TFirst lookup = null;
		//	var hashes = new HashSet<TFirst>();
		//	cnn.Query<TFirst, TSecond, TFirst>(sql, (first, second) =>
		//	{
		//		//第一条记录，或者新的主体记录，否则lookup还是上一条记录
		//		if (lookup == null || lookup.Id != first.Id)
		//			lookup = first;

		//		if (second != null && second.Id > 0 && setting != null)
		//			setting(lookup, second);

		//		if (!hashes.Any(m => m.Id == lookup.Id))
		//			hashes.Add(lookup);

		//		return null;
		//	}, param: param, splitOn: splitOn);

		//	return hashes;
		//}
		///// <summary>
		///// 异步查询带聚合导航属性的对象集合
		///// </summary>
		///// <typeparam name="TFirst">主体对象类型</typeparam>
		///// <typeparam name="TSecond">聚合导航对象类型</typeparam>
		///// <param name="setting">设置聚合导航属性的方法</param>
		//public static async Task<IEnumerable<TFirst>> QueryAsync<TFirst, TSecond>(this IDbConnection cnn, string sql, Action<TFirst, TSecond> setting, object param = null, string splitOn = "Id")
		//	where TFirst : IBaseEntity<int>
		//	where TSecond : IBaseEntity<int>
		//{
		//	TFirst lookup = null;
		//	var hashes = new HashSet<TFirst>();
		//	await cnn.QueryAsync<TFirst, TSecond, TFirst>(sql, (first, second) =>
		//	{
		//		//第一条记录，或者新的主体记录，否则lookup还是上一条记录
		//		if (lookup == null || lookup.Id != first.Id)
		//			lookup = first;

		//		if (second != null && second.Id > 0 && setting != null)
		//			setting(lookup, second);

		//		if (!hashes.Any(m => m.Id == lookup.Id))
		//			hashes.Add(lookup);

		//		return null;
		//	}, param: param, splitOn: splitOn);

		//	return hashes;
		//}

		public static DataSet QueryDataSet(this IDbConnection cnn, IDbDataAdapter adapter, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			var ds = new DataSet();
			var command = new CommandDefinition(cnn, sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
			var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param == null ? null : param.GetType(), null);
			var info = GetCacheInfo(identity, param, command.AddToCache);
			bool wasClosed = cnn.State == ConnectionState.Closed;
			if (wasClosed) cnn.Open();
			adapter.SelectCommand = command.SetupCommand(cnn, info.ParamReader);
			adapter.Fill(ds);
			if (wasClosed) cnn.Close();
			return ds;
		}
	}
}
