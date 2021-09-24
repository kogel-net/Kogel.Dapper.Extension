using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Entites;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Extension;
using System;

namespace Kogel.Dapper.Extension.Expressions
{
    public sealed class UpdateExpression<T> : BaseExpressionVisitor
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
#if DEBUG
                        Console.WriteLine("警告:更新操作不支持导航属性写入!");
#endif
                    }
                    else
                    {
                        //值对象
                        Visit(memberInit.Expression);
                        var entityObject = EntityCache.QueryEntity(expression.ReturnType);
                        string fieldName = entityObject.FieldPairs[memberInit.Member.Name];
                        _sqlCmd.Append($" {provider.ProviderOption.CombineFieldName(fieldName)} = {base.SpliceField} ");
                        Param.AddDynamicParams(base.Param);
                    }
                    base.Index++;
                }
                _sqlCmd.Insert(0, " SET ");
            }
            else//匿名类
            {
                var entityValue = expression.Body.ToConvertAndGetValue();
                string sql = provider.ResolveExpression.ResolveUpdate((T)entityValue, Param, null);
                _sqlCmd.Append(sql);
                //throw new DapperExtensionException("更新操作不支持匿名类写入");
            }
        }
    }
}
