using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Kogel.Dapper.Extension.Model
{
    public class DynamicTree
    {
        public DynamicTree()
        {
            Operators = ExpressionType.Equal;
            ValueType = DbType.String;
        }
		/// <summary>
		/// 对应字段的表(默认为空)
		/// </summary>
        public string Table { get; set; }
		/// <summary>
		/// 字段
		/// </summary>
        public string Field { get; set; }
		/// <summary>
		/// 判断方式(13 等于,6 模糊查询,16 大于等于,21 小于等于)
		/// </summary>
        public ExpressionType Operators { get; set; }
		/// <summary>
		/// 判断的值
		/// </summary>
        public string Value { get; set; }
		/// <summary>
		/// 值类型(6 DateTime,11 Int32,16 String)
		/// </summary>
        public DbType ValueType { get; set; }
    }
}
