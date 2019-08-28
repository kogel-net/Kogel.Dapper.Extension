using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Oracle
{
    internal class ResolveExpression:IResolveExpression
    {
        public ResolveExpression(IProviderOption providerOption):base(providerOption)
        {

        }
        public override string ResolveSelect(EntityObject entityObject, LambdaExpression selector, int? topNum, DynamicParameters Param)
        {
            var selectFormat = " SELECT {0} ";
            var selectSql = "";
            if (selector == null)
            {
                var propertyBuilder = GetTableField(entityObject);
                selectSql = string.Format(selectFormat, propertyBuilder);
            }
            else
            {
                var selectExp = new SelectExpression(selector, "", providerOption);
                selectSql = string.Format(selectFormat, selectExp.SqlCmd);
                Param.AddDynamicParams(selectExp.Param);
            }
            return selectSql;
        }

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
                        selectSql = $@" SELECT NVL(SUM({entityObject.GetAsName(providerOption)}
                                 {providerOption.CombineFieldName(entityObject.FieldPairs[memberName])}),0)  ";
                    }
                    break;
                case ExpressionType.MemberInit:
                    throw new DapperExtensionException("不支持该表达式类型");
            }

            return selectSql;
        }
        public override string ResolveWithNoLock(bool nolock)
        {
            return "";
        }
    }
}
