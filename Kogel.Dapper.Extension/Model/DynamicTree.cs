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
        public string Table { get; set; }
        public string Field { get; set; }
        public ExpressionType Operators { get; set; }
        public string Value { get; set; }
        public DbType ValueType { get; set; }
    }
}
