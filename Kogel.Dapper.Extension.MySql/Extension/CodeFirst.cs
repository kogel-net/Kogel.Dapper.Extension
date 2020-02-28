using Kogel.Dapper.Extension.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Dapper.Extension.MySql.Extension
{
	/// <summary>
	/// 同步实体到数据库 UseAutoSyncStructure
	/// </summary>
	public class CodeFirst : ICodeFirst
	{
		private IDbConnection connection;

		public CodeFirst(IDbConnection connection)
		{
			this.connection = connection;
		}

		/// <summary>
		/// 转换字段类型
		/// </summary>
		/// <param name="sqlDbType"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string ConversionFieldType(SqlDbType sqlDbType, int length)
		{
			string fieldType = string.Empty;
			switch (sqlDbType)
			{
				case SqlDbType.Int:
					{
						fieldType = $"Int({(length == 0 ? 11 : length)})";
						break;
					}
				case SqlDbType.BigInt:
					{
						fieldType = $"BigInt({(length == 0 ? 11 : length)})";
						break;
					}
				case SqlDbType.Decimal:
					{
						fieldType = $"Decimal({(length == 0 ? 8 : length)})";
						break;
					}
				case SqlDbType.Bit:
					{
						fieldType = $"Bit({(length == 0 ? 2 : length)})";
						break;
					}
				case SqlDbType.VarChar:
					{
						fieldType = $"VarChar({(length == 0 ? 50 : length)})";
						break;
					}
				case SqlDbType.NVarChar:
					{
						fieldType = $"NVarChar({(length == 0 ? 50 : length)})";
						break;
					}
				case SqlDbType.Date:
					{
						fieldType = $"Date";
						break;
					}
				case SqlDbType.Time:
					{
						fieldType = $"Time";
						break;
					}
				case SqlDbType.DateTime:
					{
						fieldType = $"DateTime({(length == 0 ? 6 : length)})";
						break;
					}
				default:
					{
						throw new Exception.DapperExtensionException("不存在的数据类型，请参考文档设置SqlDbType");
					}
			}
			return fieldType;
		}

		/// <summary>
		/// 同步字段
		/// </summary>
		/// <param name="typeEntity"></param>
		/// <param name="field"></param>
		public string SyncField(EntityObject typeEntity, EntityField field)
		{
			string fieldName = connection.QuerySingleOrDefault<string>($@"SELECT COLUMN_NAME  FROM information_schema.columns 
                                                   WHERE TABLE_NAME = '{typeEntity.Name}' AND COLUMN_NAME = '{field.FieldName}'");
			//存在
			if (!string.IsNullOrEmpty(fieldName))
			{
				//存在的字段不会做任何修改
				return "";
			}
			else//不存在
			{
				string fieldType = ConversionFieldType(field.SqlDbType, field.Length);
				StringBuilder fieldScript = new StringBuilder($"ALTER TABLE `{typeEntity.Name}`");
				fieldScript.Append($" ADD `{field.FieldName}` {fieldType} ");
				//设置是否可以为空
				if (field.IsNull)
					fieldScript.Append(" NULL");
				else
					fieldScript.Append(" NOT NULL");
				//设置备注
				if (!string.IsNullOrEmpty(field.Description))
					fieldScript.Append($" COMMENT '{field.Description}'");
				//设置是否是主键
				if (field.IsIdentity)
					fieldScript.Append(" PRIMARY KEY");
				//设置是否自增
				if (field.IsIncrease)
					fieldScript.Append(" AUTO_INCREMENT");
				fieldScript.Append(";");
				return fieldScript.ToString();
			}
		}

		/// <summary>
		/// 同步整体实体结构
		/// </summary>
		public void SyncStructure()
		{
			foreach (var entity in EntityCache.GetEntities())
			{
				Type type;
				if (ExpressionExtension.IsAnyBaseEntity(entity.Type, out type))
					SyncTable(entity);
			}
		}

		/// <summary>
		/// 同步单个表结构
		/// </summary>
		/// <param name="typeEntity"></param>
		public void SyncTable(EntityObject typeEntity)
		{
			//首先检查表是否存在
			string tableName = connection.QuerySingleOrDefault<string>($"SHOW TABLES LIKE '{typeEntity.Name}'");
			//脚本字符
			StringBuilder scriptBuilder = new StringBuilder();
			//创建表时会产生一个测试字段
			string testidName = $"TEST_ID_{DateTime.Now.ToString("mmssffff")}";
			//不存在表，先创建表
			if (string.IsNullOrEmpty(tableName))
			{
				//创建整张表
				scriptBuilder.Append($@"CREATE TABLE `{typeEntity.Name}`(
                                       `{testidName}` INT 
                                     );");
			}
			foreach (var field in typeEntity.EntityFieldList)
			{
				scriptBuilder.Append(SyncField(typeEntity, field));
			}
			//执行脚本
			if (scriptBuilder.Length != 0)
			{
				string script = scriptBuilder.ToString();
				//删除测试字段
				if (script.Contains(testidName))
				{
					script += $@"ALTER TABLE `{typeEntity.Name}`
                                            DROP `{testidName}`";
				}
				connection.Execute(script);
			}
		}
	}
}
