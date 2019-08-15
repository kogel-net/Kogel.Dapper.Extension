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
                fieldArr = ((MemberInitExpression)expression.Body).Bindings.AsList().Select(x => entity.FieldPairs[x.Member.Name]).ToArray();
            }
            else//匿名类
            {
                fieldArr = entity.Properties.Select(x => x.Name).ToArray();
            }
            //开始解析对象
            Visit(expression);
            //开始拼接成查询字段
            for (var i = 0; i < fieldArr.Length; i++)
            {
                if (_sqlCmd.Length != 0)
                    _sqlCmd.Append(",");
                _sqlCmd.Append(base.FieldList[i] + " as " + fieldArr[i]);
            }
            this.Param.AddDynamicParams(base.Param);
        }
    }
}
