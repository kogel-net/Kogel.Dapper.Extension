using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.MsSql
{
    internal class ResolveExpression : IResolveExpression
    {
        public ResolveExpression(IProviderOption providerOption) : base(providerOption)
        {

        }
        /// <summary>
        /// 解析查询字段
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="selector"></param>
        /// <param name="topNum"></param>
        /// <param name="Param"></param>
        /// <returns></returns>
        public override string ResolveSelect(EntityObject entityObject, LambdaExpression selector, int? topNum, DynamicParameters Param)
        {
            var selectFormat = topNum.HasValue ? " SELECT {1} {0} " : " SELECT {0} ";
            var selectSql = "";
            //不是自定义返回视图则显示所有字段
            if (selector == null)
            {
                var propertyBuilder = GetTableField(entityObject);
                selectSql = string.Format(selectFormat, propertyBuilder, $" TOP {topNum} ");
            }
            else//自定义查询字段
            {
                var selectExp = new SelectExpression(selector, "", providerOption);
                selectSql = string.Format(selectFormat, selectExp.SqlCmd, $" TOP {topNum} ");
                Param.AddDynamicParams(selectExp.Param);
            }
            return selectSql;
        }
        /// <summary>
        /// 解析查询总和
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public override string ResolveSum(LambdaExpression selector)
        {
            if (selector == null)
                throw new ArgumentException("selector");
            var selectSql = "";

            switch (selector.NodeType)
            {
                case ExpressionType.Lambda:
                case ExpressionType.MemberAccess:
                    {
                        EntityObject entityObject = EntityCache.QueryEntity(selector.Parameters[0].Type);
                        var memberName = selector.Body.GetCorrectPropertyName();
                        selectSql = $" SELECT ISNULL(SUM({entityObject.AsName}.{providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}),0)  ";
                    }
                    break;
                case ExpressionType.MemberInit:
                    throw new DapperExtensionException("不支持该表达式类型");
            }

            return selectSql;
        }
    }
}
