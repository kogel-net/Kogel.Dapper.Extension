using System.Data;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
	/// <summary>
	/// 同步实体到数据库
	/// </summary>
	public interface ICodeFirst
	{
		/// <summary>
		/// 同步字段(生成sql)
		/// </summary>
		/// <param name="typeEntity"></param>
		/// <param name="field"></param>
		string SyncField(EntityObject typeEntity, EntityField field);

		/// <summary>
		/// 同步单个表结构
		/// </summary>
		/// <param name="typeEntity"></param>
		void SyncTable(EntityObject typeEntity);

		/// <summary>
		/// 同步整体实体结构
		/// </summary>
		void SyncStructure();

		/// <summary>
		/// 转换字段类型
		/// </summary>
		/// <param name="sqlDbType">字段类型</param>
		/// <param name="length">长度</param>
		/// <returns></returns>
		string ConversionFieldType(SqlDbType sqlDbType, int length);
	}
}
