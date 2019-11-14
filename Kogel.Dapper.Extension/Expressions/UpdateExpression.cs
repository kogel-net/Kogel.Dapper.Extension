using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.Core.Interfaces;

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
        #region 当前解析的对象
        private EntityObject entity { get; }
        #endregion
        /// <inheritdoc />
        /// <summary>
        /// 执行解析
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateExpression(LambdaExpression expression, IProviderOption providerOption) : base(providerOption)
        {
            this._sqlCmd = new StringBuilder(100);
            this.Param = new DynamicParameters();
            this.providerOption = providerOption;
			//update不需要重命名
			providerOption.IsAsName = false;
            //当前定义的查询返回对象
            this.entity = EntityCache.QueryEntity(expression.Body.Type);
            //字段数组
            string[] fieldArr = ((MemberInitExpression)expression.Body).Bindings.AsList().Select(x => entity.FieldPairs[x.Member.Name]).ToArray();

            //开始解析对象
            Visit(expression);
            //开始拼接成查询字段
            for (var i = 0; i < fieldArr.Length; i++)
            {
                if (_sqlCmd.Length != 0)
                    _sqlCmd.Append(",");
                string field = fieldArr[i];
                string value = base.FieldList[i];
				//判断是不是包含字段的值，如果是就不放入Param中
				if (value.Contains(entity.AsName) || value == providerOption.GetDate())
				{
					_sqlCmd.Append(field + "=" + value);
				}
				else
				{
					var ParamName = $"UPDATE_{field}_{i}";
					_sqlCmd.Append($"{providerOption.CombineFieldName(field)}={providerOption.ParameterPrefix + ParamName}");
					Param.Add(ParamName, value == "NULL" ? null : value);
				}
            }
            //加入条件参数
            Param.AddDynamicParams(base.Param);
            _sqlCmd.Insert(0, " SET ");
        }
    }
}
