using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
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
		#region 当前解析的对象
		private EntityObject entity { get; }
		#endregion
		public GroupExpression(LambdaExpression expression, string prefix, IProviderOption providerOption) : base(providerOption)
		{
			this._sqlCmd = new StringBuilder(100);
			this.Param = new DynamicParameters();
			this.providerOption = providerOption;
			//当前定义的查询返回对象
			this.entity = EntityCache.QueryEntity(expression.Body.Type);
			//执行解析
			Visit(expression);
			//分组指定字段
			if (base.FieldList.Any())
			{
				//开始拼接成分组字段
				for (var i = 0; i < base.FieldList.Count; i++)
				{
						if (_sqlCmd.Length != 0)
							_sqlCmd.Append(",");
						_sqlCmd.Append(base.FieldList[i]);
				}
			}
			else
			{
				_sqlCmd.Append(base.SpliceField);
			}
		}
		/// <summary>
		/// 匿名类每组表达式解析
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitNew(NewExpression node)
		{
			foreach (var argument in node.Arguments)
			{
				base.SpliceField.Clear();
				base.Visit(argument);
				base.FieldList.Add(SpliceField.ToString());
			}
			return node;
		}
	}
}
