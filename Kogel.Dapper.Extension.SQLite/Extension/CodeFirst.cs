using System;
using System.Data;
using System.Text;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Entites;
using Kogel.Dapper.Extension.Extension;

namespace Kogel.Dapper.Extension.SQLite.Extension
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
                        fieldType = "INTEGER";
                        break;
                    }
                case SqlDbType.BigInt:
                    {
                        fieldType = "INTEGER";
                        break;
                    }
                case SqlDbType.Decimal:
                    {
                        fieldType = "REAL";
                        break;
                    }
                case SqlDbType.Bit:
                    {
                        fieldType = "INTEGER";
                        break;
                    }
                case SqlDbType.VarChar:
                    {
                        fieldType = $"VARCHAR({(length == 0 ? 50 : length)})";
                        break;
                    }
                case SqlDbType.NVarChar:
                    {
                        fieldType = $"VARCHAR({(length == 0 ? 50 : length)})";
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
                        fieldType = "timestamp";
                        break;
                    }
                default:
                    {
                        throw new DapperExtensionException("不存在的数据类型，请参考文档设置SqlDbType");
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
            string fieldName = connection.QuerySingleOrDefault<string>($@"SELECT * FROM sqlite_master WHERE name='{typeEntity.Name}' AND sql LIKE '%{field.FieldName}%'");
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
                if (field.IfNull)
                    fieldScript.Append(" NULL");
                else
                    fieldScript.Append(" NOT NULL DEFAULT ''");
                //设置备注
                //if (!string.IsNullOrEmpty(field.Description))
                //    fieldScript.Append($" COMMENT '{field.Description}'");
                //设置是否是主键
                //if (field.IsIdentity)
                //    fieldScript.Append(" PRIMARY KEY");
                //设置是否自增
                //if (field.IsIncrease)
                //    fieldScript.Append(" AUTO_INCREMENT");
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
            string tableName = connection.QuerySingleOrDefault<string>($"SELECT name FROM sqlite_master WHERE type='table' AND name='{typeEntity.Name}'");
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
