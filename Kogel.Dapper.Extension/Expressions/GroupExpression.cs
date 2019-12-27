using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Expressions
{
	/// <summary>
	/// 解析分组
	/// </summary>
	public class GroupExpression: BaseExpressionVisitor
	{
		#region sql指令
		private readonly StringBuilder _sqlCmd;
		/// <summary>
		/// sql指令
		/// </summary>
		public string SqlCmd => _sqlCmd.ToString();
		/// <summary>
		/// 参数
		/// </summary>
		public new DynamicParameters Param;
		#endregion
		public GroupExpression(LambdaExpression expression, string prefix, SqlProvider provider) : base(provider)
		{
			this._sqlCmd = new StringBuilder();
			this.Param = new DynamicParameters();
			//当前定义的查询返回对象
			EntityObject entity = EntityCache.QueryEntity(expression.Body.Type);
			var newExpression = expression.Body as NewExpression;
			foreach (var argument in newExpression.Arguments)
			{
				base.SpliceField.Clear();
				base.Param = new DynamicParameters();
				if (_sqlCmd.Length != 0)
					_sqlCmd.Append(",");
				//返回类型
				var returnProperty = entity.Properties[base.Index];
				//实体类型
				Type entityType;
				//验证是实体类或者是泛型
				if (ExpressionExtension.IsAnyBaseEntity(returnProperty.PropertyType, out entityType))
				{
					throw new DapperExtensionException("GroupBy不支持导航属性!");
				}
				else
				{
					//值对象
					Visit(argument);
					_sqlCmd.Append($" {base.SpliceField} ");
					Param.AddDynamicParams(base.Param);
				}
				base.Index++;
			}
		}
	}
}
