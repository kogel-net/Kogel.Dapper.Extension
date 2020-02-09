using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension.Core.Interfaces;
using System.Linq.Expressions;
using System.Data;

namespace Kogel.Dapper.Extension.Model
{
	public class EntityObject
	{
		public EntityObject(Type type)
		{
			//反射表名称
			this.Name = type.Name;
			//指定as名称
			this.AsName = type.Name;
			//获取是否有Display特性
			var typeAttribute = type.GetCustomAttributess(true).FirstOrDefault(x => x.GetType().Equals(typeof(Display)));
			if (typeAttribute != null)
			{
				var display = typeAttribute as Display;
				//是否有重命名
				var rename = display.Rename;
				if (!string.IsNullOrEmpty(rename))
				{
					this.Name = rename;
				}
				//是否有名称空间
				string schema = display.Schema;
				if (!string.IsNullOrEmpty(schema))
				{
					this.Schema = schema;
				}
				//是否有指定as名称
				string asName = display.AsName;
				if (!string.IsNullOrEmpty(asName))
				{
					this.AsName = asName;
				}
				else
				{
					//防止rename有值，这样就不会出现bug
					this.AsName = this.Name;
				}
			}
			this.Type = type;
			this.AssemblyString = type.FullName;
			//反射实体类属性
			this.Properties = type.GetProperties();
			List<PropertyInfo> PropertyInfoList = new List<PropertyInfo>();
			//字段字典
			this.FieldPairs = new Dictionary<string, string>();
			//导航列表
			this.Navigations = new List<JoinAssTable>();
			//字段列表
			this.EntityFieldList = new List<EntityField>();
			//反射实体类字段
			foreach (var item in this.Properties)
			{
				//子父类存在相同的字段
				if (this.FieldPairs.Any(x => x.Key == item.Name))
				{
					continue;
				}
				//当前字段是导航属性
				var foreignKey = item.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Equals(typeof(ForeignKey)));
				if (foreignKey != null)
				{
					var foreign = foreignKey as ForeignKey;
					//导航属性表
					var navigationTable = !item.PropertyType.FullName.Contains("System.Collections.Generic") ? item.PropertyType : item.PropertyType.GenericTypeArguments[0];
					var leftTab = EntityCache.QueryEntity(navigationTable);
					this.Navigations.Add(new JoinAssTable()
					{
						Action = JoinAction.Navigation,
						JoinMode = JoinMode.LEFT,
						RightTabName = this.AsName,
						RightAssName = foreign.IndexField,
						LeftTabName = leftTab.Name,
						LeftAssName = foreign.AssoField,
						TableType = navigationTable,
						//PropertyType = item.PropertyType
						PropertyInfo = item
					});
					PropertyInfoList.Add(item);
					continue;
				}
				//当前字段属性设置
				var fieldAttribute = item.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Equals(typeof(Display)));
				if (fieldAttribute != null)
				{
					var display = fieldAttribute as Display;
					//获取是否是表关系隐射字段
					if (display.IsField)
					{
						this.FieldPairs.Add(item.Name, item.Name);
						//获取是否有重命名
						if (!string.IsNullOrEmpty(display.Rename))
						{
							this.FieldPairs[item.Name] = display.Rename;
						}
						PropertyInfoList.Add(item);
						//设置详细属性
						EntityFieldList.Add(new EntityField()
						{
							FieldName = this.FieldPairs[item.Name],
							PropertyInfo = item,
							SqlDbType = display.SqlDbType != SqlDbType.Structured ? display.SqlDbType : GetSqlDbType(item.PropertyType),
							Length = display.Length,
							Description = display.Description,
							IsNull = display.IsNull,
							DefaultValue = display.DefaultValue
						});
					}
				}
				else
				{
					this.FieldPairs.Add(item.Name, item.Name);
					PropertyInfoList.Add(item);

					//设置详细属性
					EntityFieldList.Add(new EntityField()
					{
						FieldName = item.Name,
						PropertyInfo = item,
						SqlDbType = GetSqlDbType(item.PropertyType),
						Length = 0
					});
				}
				//获取主键
				if (string.IsNullOrEmpty(Identitys))
				{
					//当前字段是主键
					var identityAttribute = item.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Equals(typeof(Identity)));
					if (identityAttribute != null)
					{
						this.Identitys = this.FieldPairs[item.Name];
						EntityFieldList[EntityFieldList.Count - 1].IsIdentity = true;
						EntityFieldList[EntityFieldList.Count - 1].IsIncrease = (identityAttribute as Identity).IsIncrease;
					}
				}
			}
			this.Properties = PropertyInfoList.ToArray();
		}

		/// <summary>
		/// 主键名称
		/// </summary>
		public string Identitys { get; set; }

		/// <summary>
		/// 类名(表名称)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 名称空间
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		/// 指定as名称
		/// </summary>
		public string AsName { get; set; }

		/// <summary>
		/// 类型
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// 命名空间
		/// </summary>
		public string AssemblyString { get; set; }

		/// <summary>
		/// 类反射的属性实例
		/// </summary>
		public PropertyInfo[] Properties { get; set; }

		/// <summary>
		/// 字段目录(属性名称和实体名称)
		/// </summary>
		public Dictionary<string, string> FieldPairs { get; set; }

		/// <summary>
		/// 导航属性列表
		/// </summary>
		public List<JoinAssTable> Navigations { get; set; }

		/// <summary>
		/// 获取asname
		/// </summary>
		/// <param name="IsAsName"></param>
		/// <returns></returns>
		public string GetAsName(IProviderOption providerOption, bool IsAsName = true, bool IsSuffix = true)
		{
			string asName = string.Empty;
			if (IsAsName)
			{
				asName = this.AsName;
				//如果没有as name，则需要给表带上标记
				if (asName.Equals(this.Name))
					asName = providerOption.CombineFieldName(asName);
				//是否需要后缀
				if (IsSuffix)
					asName += ".";
			}
			return asName;
		}
		/// <summary>
		/// 字段列表
		/// </summary>
		public List<EntityField> EntityFieldList { get; set; }

		/// <summary>
		/// 获取默认数据类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private SqlDbType GetSqlDbType(Type type)
		{
			//设置数据库字段类型
			SqlDbType sqlDbType = SqlDbType.VarChar;
			if (type == typeof(int))
			{
				sqlDbType = SqlDbType.Int;
			}
			else if (type == typeof(long))
			{
				sqlDbType = SqlDbType.BigInt;
			}
			else if (type == typeof(Guid))
			{
				sqlDbType = SqlDbType.UniqueIdentifier;
			}
			else if (type == typeof(DateTime))
			{
				sqlDbType = SqlDbType.DateTime;
			}
			else if (type == typeof(decimal))
			{
				sqlDbType = SqlDbType.Decimal;
			}
			else if (type == typeof(bool))
			{
				sqlDbType = SqlDbType.Bit;
			}
			return sqlDbType;
		}
	}

	/// <summary>
	/// 实体字段
	/// </summary>
	public class EntityField
	{
		/// <summary>
		/// 是否是主键
		/// </summary>
		public bool IsIdentity { get; set; }

		/// <summary>
		/// 是否自增
		/// </summary>
		public bool IsIncrease { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public PropertyInfo PropertyInfo { get; set; }

		/// <summary>
		/// 字段名称
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		/// 字段类型
		/// </summary>
		public SqlDbType SqlDbType { get; set; }

		/// <summary>
		/// 字段长度
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// 字段描述
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 是否允许为空
		/// </summary>
		public bool IsNull { get; set; } = false;

		/// <summary>
		/// 默认值
		/// </summary>
		public object DefaultValue { get; set; }
	}
}
