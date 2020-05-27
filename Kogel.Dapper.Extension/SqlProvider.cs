using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Dapper;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Exception;

namespace Kogel.Dapper.Extension
{
	public abstract class SqlProvider
	{
		public AbstractDataBaseContext Context { get; set; }

		protected SqlProvider()
		{
			Params = new DynamicParameters();
			JoinList = new List<JoinAssTable>();
			AsTableNameDic = new Dictionary<Type, string>();
		}

		public abstract IProviderOption ProviderOption { get; set; }

		public string SqlString { get; set; }
		/// <summary>
		/// 连接对象集合
		/// </summary>
		public List<JoinAssTable> JoinList { get; set; }
		/// <summary>
		/// 参数对象
		/// </summary>
		public DynamicParameters Params { get; set; }
		/// <summary>
		/// 重命名目录
		/// </summary>
		public Dictionary<Type, string> AsTableNameDic { get; set; }

		public abstract SqlProvider FormatGet<T>();

		public abstract SqlProvider FormatToList<T>();

		public abstract SqlProvider FormatToPageList<T>(int pageIndex, int pageSize);

		public abstract SqlProvider FormatCount();

		public abstract SqlProvider FormatDelete();

		public abstract SqlProvider FormatInsert<T>(T entity, string[] excludeFields);

		public abstract SqlProvider FormatInsertIdentity<T>(T entity, string[] excludeFields);

		public abstract SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression);

		public abstract SqlProvider FormatUpdate<T>(T entity, string[] excludeFields, bool isBatch = false);

		public abstract SqlProvider FormatSum(LambdaExpression sumExpression);

		public abstract SqlProvider FormatMin(LambdaExpression MinExpression);

		public abstract SqlProvider FormatMax(LambdaExpression MaxExpression);

		public abstract SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator);

		public abstract SqlProvider CreateNew();

		/// <summary>
		/// 获取表名称
		/// </summary>
		/// <param name="isNeedFrom"></param>
		/// <param name="isAsName"></param>
		/// <param name="tableType">连接查询时会用到</param>
		/// <returns></returns>

		public string FormatTableName(bool isNeedFrom = true, bool isAsName = true, Type tableType = null)
		{
			var entity = EntityCache.QueryEntity(tableType == null ? Context.Set.TableType : tableType);
			string schema = string.IsNullOrEmpty(entity.Schema) ? "" : ProviderOption.CombineFieldName(entity.Schema) + ".";
			string fromName = entity.Name;
			//函数AsTableName优先级大于一切
			string asTableName;
			if (AsTableNameDic.TryGetValue(entity.Type, out asTableName))
			{
				fromName = asTableName;
			}
			//是否存在实体特性中的AsName标记
			if (isAsName)
				fromName = entity.AsName.Equals(fromName) ? ProviderOption.CombineFieldName(fromName) : $"{ProviderOption.CombineFieldName(fromName)} {entity.AsName}";
			else
				fromName = ProviderOption.CombineFieldName(fromName);
			SqlString = $" {schema}{fromName} ";
			if (isNeedFrom)
				SqlString = " FROM " + SqlString;

			return SqlString;
		}

		protected string[] FormatInsertParamsAndValues<T>(T t, string[] excludeFields = null)
		{
			var paramSqlBuilder = new StringBuilder(64);
			var valueSqlBuilder = new StringBuilder(64);

			var entity = EntityCache.QueryEntity(t.GetType());
			var properties = entity.Properties;

			var isAppend = false;
			foreach (var propertiy in properties)
			{
				//是否是排除字段
				if (excludeFields != null && excludeFields.Contains(propertiy.Name))
				{
					continue;
				}
				var customAttributes = propertiy.GetCustomAttributess(true);
				//导航属性排除
				if (customAttributes.Any(x => x.GetType().Equals(typeof(ForeignKey))))
				{
					continue;
				}
				//主键标识
				var typeAttribute = customAttributes.FirstOrDefault(x => x.GetType().Equals(typeof(Identity)));
				if (typeAttribute != null)
				{
					var identity = typeAttribute as Identity;
					//是否自增
					if (identity.IsIncrease)
					{
						continue;
					}
				}
				//排除掉时间格式为最小值的字段
				if (propertiy.PropertyType == typeof(DateTime))
				{
					if (Convert.ToDateTime(propertiy.GetValue(t)) == DateTime.MinValue)
					{
						continue;
					}
				}
				if (isAppend)
				{
					paramSqlBuilder.Append(",");
					valueSqlBuilder.Append(",");
				}
				var name = propertiy.GetColumnAttributeName();
				paramSqlBuilder.AppendFormat("{0}{1}{2}", ProviderOption.OpenQuote, entity.FieldPairs[name], ProviderOption.CloseQuote);
				valueSqlBuilder.Append(ProviderOption.ParameterPrefix + name);
				Params.Add(ProviderOption.ParameterPrefix + name, propertiy.GetValue(t));
				isAppend = true;
			}
			return new[] { paramSqlBuilder.ToString(), valueSqlBuilder.ToString() };
		}

		protected DataBaseContext<T> DataBaseContext<T>()
		{
			return (DataBaseContext<T>)Context;
		}
	
		/// <summary>
		/// 根据主键获取条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected string GetIdentityWhere<T>(T entity, DynamicParameters param)
		{
			var entityObject = EntityCache.QueryEntity(typeof(T));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			//获取主键数据
			var id = entityObject.Properties
				.FirstOrDefault(x => x.Name == entityObject.Identitys)
				.GetValue(entity);
			//设置参数
			param.Add(entityObject.Identitys, id);
			return $" AND {entityObject.Identitys}={ProviderOption.ParameterPrefix}{entityObject.Identitys} ";
		}
	}
}
