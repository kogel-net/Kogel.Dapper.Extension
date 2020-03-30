using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using System;
using System.Data;
using System.Text;
using Dapper;

namespace Kogel.Dapper.Extension.Oracle.Extension
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
						fieldType = $"NUMBER({(length == 0 ? 10 : length)})";
						break;
					}
				case SqlDbType.BigInt:
					{
						fieldType = $"NUMBER({(length == 0 ? 19 : length)})";
						break;
					}
				case SqlDbType.Decimal:
					{
						fieldType = $"NUMBER({(length == 0 ? 10 : length)},3)";
						break;
					}
				case SqlDbType.Bit:
					{
						fieldType = $"NUMBER({(length == 0 ? 1 : length)})";
						break;
					}
				case SqlDbType.VarChar:
					{
						fieldType = $"VARCHAR2({(length == 0 ? 50 : length)})";
						break;
					}
				case SqlDbType.NVarChar:
					{
						fieldType = $"NVARCHAR2({(length == 0 ? 50 : length)})";
						break;
					}
				case SqlDbType.Date:
					{
						fieldType = $"DATE";
						break;
					}
				case SqlDbType.Time:
					{
						fieldType = $"TIMESTAMP";
						break;
					}
				case SqlDbType.DateTime:
					{
						fieldType = $"DATE";
						break;
					}
				case SqlDbType.UniqueIdentifier:
					{
						fieldType = $"CHAR(36)";
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
		/// <returns></returns>
		public string SyncField(EntityObject typeEntity, EntityField field)
		{
			string fieldName = connection.QuerySingleOrDefault<string>($@"SELECT COLUMN_NAME FROM ALL_TAB_COLUMNS 
                                                   WHERE UPPER(TABLE_NAME) = UPPER('{typeEntity.Name}') AND UPPER(COLUMN_NAME) = UPPER('{field.FieldName}')");
			//存在
			if (!string.IsNullOrEmpty(fieldName))
			{
				//存在的字段不会做任何修改
				return "";
			}
			else//不存在
			{
				string fieldType = ConversionFieldType(field.SqlDbType, field.Length);
				StringBuilder fieldScript = new StringBuilder($@"ALTER TABLE ""{typeEntity.Name}""");
				fieldScript.Append($@" ADD (""{ field.FieldName}"" {fieldType}");
				//设置默认值
				if (field.DefaultValue != null)
					fieldScript.Append($" DEFAULT '{field.DefaultValue}'");
				//设置是否可以为空
				if (field.IsNull)
					fieldScript.Append(" NULL");
				else
					fieldScript.Append(" NOT NULL");
				//设置是否是主键
				if (field.IsIdentity)
					fieldScript.Append($" CONSTRAINT PRIMARY_KEY_{typeEntity.Name}_{field.FieldName} PRIMARY KEY");
				fieldScript.Append(")");

				string script = fieldScript.ToString();
				connection.Execute(script);
				//设置备注
				if (!string.IsNullOrEmpty(field.Description))
					connection.Execute($@"COMMENT ON COLUMN ""{typeEntity.Name}"".""{field.FieldName}""   is   '{field.Description} '");
				//设置是否自增
				if (field.IsIncrease)
				{
					//序列名称
					string sequenceName = ($"{typeEntity.Name}_{field.FieldName}_SEQ").ToUpper();
					//检索是否存在此序列
					string seqName = connection.QueryFirstOrDefault<string>($@"SELECT SEQUENCE_NAME FROM all_sequences 
                          WHERE SEQUENCE_NAME = '{sequenceName}'");
					//首先查询序列是否存在
					if (!string.IsNullOrEmpty(seqName))
					{
						//删除旧序列
						connection.Execute($"DROP SEQUENCE {seqName}");
					}
					//创建序列
					connection.Execute($"CREATE SEQUENCE {sequenceName} Increment by 1 Start With 1 Nomaxvalue Nocycle Cache 10");
					//创建触发器
					connection.Execute($@"CREATE OR REPLACE TRIGGER {typeEntity.Name}_SEQUENCE_TRIG
                                       BEFORE INSERT ON ""{typeEntity.Name}""
									   FOR EACH ROW
									   BEGIN
									       SELECT {sequenceName}.NEXTVAL INTO :new.""{field.FieldName}"" FROM DUAL;
									   END;");
				}
				return script;
			}
		}

		/// <summary>
		/// 同步整体实体结构
		/// </summary>
		public void SyncStructure()
		{
			foreach (var entity in EntityCache.GetEntities())
			{
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
			string tableName = connection.QuerySingleOrDefault<string>($"SELECT TABLE_NAME FROM ALL_TABLES WHERE UPPER(TABLE_NAME) =UPPER('{typeEntity.Name}')");
			//脚本字符 oracle无法多条语句一起执行
			StringBuilder scriptBuilder = new StringBuilder();
			//创建表时会产生一个测试字段
			string testidName = $"TEST_ID_{DateTime.Now.ToString("mmssffff")}";
			//不存在表，先创建表
			if (string.IsNullOrEmpty(tableName))
			{
				//创建整张表
				scriptBuilder.Append($@"CREATE TABLE ""{typeEntity.Name}"" (
                                          ""{testidName}"" NUMBER
                                          )");
				connection.Execute(scriptBuilder.ToString());
			}
			foreach (var field in typeEntity.EntityFieldList)
			{
				SyncField(typeEntity, field);
			}
			//执行脚本
			if (scriptBuilder.Length != 0)
			{
				string script = scriptBuilder.ToString();
				//删除测试字段
				if (script.Contains(testidName))
				{
					connection.Execute($@"ALTER TABLE ""{typeEntity.Name}"" DROP (""{testidName}"")");
				}
			}
		}
	}
}
