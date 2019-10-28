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
				//fieldArr = bingings
				//	.AsList()
				//	.Where(x => entity.FieldPairs.Any(y => y.Key.Equals(x.Member.Name)))
				//	.Select(x => entity.FieldPairs[x.Member.Name])
				//	.ToArray();
				List<string> fieldList = new List<string>();
				foreach (var bind in bingings)
				{
					if (bind is MemberAssignment)
					{
						//必须存在实体类中
						if (entity.FieldPairs.Any(x=>x.Key.Equals(bind.Member.Name)))
						{
							var assignment = (bind as MemberAssignment);
							if (assignment.Expression.Type.FullName.Contains("System.Collections.Generic.List"))
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
            else//匿名类
            {
                fieldArr = entity.Properties.Select(x => x.Name).ToArray();
            }
            //开始解析对象
            Visit(expression);
			if (!expression.Body.NodeType.Equals(ExpressionType.MemberAccess))
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
				//单个字段返回
				_sqlCmd.Append(base.FieldList[0]);
			}
            this.Param.AddDynamicParams(base.Param);
        }
    }
}
