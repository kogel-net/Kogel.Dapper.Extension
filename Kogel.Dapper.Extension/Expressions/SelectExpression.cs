using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Kogel.Dapper.Extension.Helper;
using System.Collections.Generic;

namespace Kogel.Dapper.Extension.Expressions
{
    /// <summary>
    /// 解析自定义查询字段
    /// </summary>
    public class SelectExpression : BaseExpressionVisitor
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

        private IProviderOption providerOption;

        #endregion
        #region 当前解析的对象
        private EntityObject entity { get; }
        #endregion
        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="prefix">字段前缀</param>
        /// <param name="providerOption"></param>
        public SelectExpression(LambdaExpression expression, string prefix, IProviderOption providerOption) : base(providerOption)
        {
            this._sqlCmd = new StringBuilder(100);
            this.Param = new DynamicParameters();
            this.providerOption = providerOption;
            //当前定义的查询返回对象
            this.entity = EntityCache.QueryEntity(expression.Body.Type);
            //字段数组
            string[] fieldArr;
            //判断是不是实体类
            if (expression.Body is MemberInitExpression)
            {
				var bingings = ((MemberInitExpression)expression.Body).Bindings;
				List<string> fieldList = new List<string>();
				foreach (var bind in bingings)
				{
					if (bind is MemberAssignment)
					{
						//必须存在实体类中
						if (entity.FieldPairs.Any(x => x.Key.Equals(bind.Member.Name)))
						{
							var assignment = (bind as MemberAssignment);
							//判断是列表还是不是系统函数
							if (assignment.Expression.Type.FullName.Contains("System.Collections.Generic.List") 
								|| assignment.Expression.Type.BaseType.FullName.Contains("Kogel.Dapper.Extension.IBaseEntity"))
							{
								providerOption.NavigationList.Add(new NavigationMemberAssign()
								{
									MemberAssign = assignment,
									MemberAssignName = bind.Member.Name
								});
							}
							else
							{
								fieldList.Add(entity.FieldPairs[bind.Member.Name]);
							}
						}
					}
				}
				fieldArr = fieldList.ToArray();
			}
            else//匿名类（暂时不支持子导航属性查询）
            {
				if (entity.Properties.Length == 0)
				{
					fieldArr = new string[] { "field1" };
				}
				else
				{
					fieldArr = entity.Properties.Select(x => x.Name).ToArray();
				}
            }
			Visit(expression);
			if (!expression.Body.NodeType.Equals(ExpressionType.MemberAccess))
			{
				//查询指定字段
				if (base.FieldList.Any())
				{
					//开始拼接成查询字段
					for (var i = 0; i < fieldArr.Length; i++)
					{
						if (i < base.FieldList.Count)
						{
							if (_sqlCmd.Length != 0)
								_sqlCmd.Append(",");
							_sqlCmd.Append(base.FieldList[i] + " as " + fieldArr[i]);
							//记录隐射对象
							providerOption.MappingList.Add(base.FieldList[i], fieldArr[i]);
						}
					}
				}
				else
				{
					_sqlCmd.Append(base.SpliceField);
				}
			}
			else
			{
				//单个字段返回
				if (base.FieldList.Any())
					_sqlCmd.Append(base.FieldList[0]);
				else
					_sqlCmd.Append(base.SpliceField);
			}
            this.Param.AddDynamicParams(base.Param);
        }
		/// <summary>
		/// 匿名类每组表达式解析
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitNew(NewExpression node)
		{
			foreach (var argument  in node.Arguments)
			{
				base.SpliceField.Clear();
				base.Visit(argument);
				base.FieldList.Add(SpliceField.ToString());
			}
			return node;
		}
	}
}
