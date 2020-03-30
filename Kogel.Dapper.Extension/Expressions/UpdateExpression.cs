using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Extension;
using System;

namespace Kogel.Dapper.Extension.Expressions
{
	public sealed class UpdateExpression : BaseExpressionVisitor
	{
		#region sql指令
		private readonly StringBuilder _sqlCmd;
		/// <summary>
		/// sql指令
		/// </summary>
		public string SqlCmd => _sqlCmd.ToString();

		public new DynamicParameters Param;
		#endregion
		/// <inheritdoc />
		/// <summary>
		/// 执行解析
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public UpdateExpression(LambdaExpression expression, SqlProvider provider) : base(provider)
		{
			this._sqlCmd = new StringBuilder(100);
			this.Param = new DynamicParameters();
			//update不需要重命名
			providerOption.IsAsName = false;
			if (expression.Body is MemberInitExpression)
			{
				var memberInitExpression = expression.Body as MemberInitExpression;
				foreach (MemberAssignment memberInit in memberInitExpression.Bindings)
				{
					base.SpliceField.Clear();
					base.Param = new DynamicParameters();
					if (_sqlCmd.Length != 0)
						_sqlCmd.Append(",");
					//实体类型
					Type entityType;
					//验证是实体类或者是泛型
					if (ExpressionExtension.IsAnyBaseEntity(memberInit.Expression.Type, out entityType))
					{
						//throw new DapperExtensionException("更新操作不支持导航属性写入");
						Console.WriteLine("警告:更新操作不支持导航属性写入!");
					}
					else
					{
						//值对象
						Visit(memberInit.Expression);
						_sqlCmd.Append($" {provider.ProviderOption.CombineFieldName(memberInit.Member.Name)} = {base.SpliceField} ");
						Param.AddDynamicParams(base.Param);
					}
					base.Index++;
				}
			}
			else//匿名类
			{
				throw new DapperExtensionException("更新操作不支持匿名类写入");
			}
			_sqlCmd.Insert(0, " SET ");
		}
	}
}
