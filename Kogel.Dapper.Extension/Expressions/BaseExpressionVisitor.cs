using Dapper;
using Kogel.Dapper.Extension.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Expressions
{
    /// <summary>
    /// 实现表达式解析的基类
    /// </summary>
    public class BaseExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 字段集合
        /// </summary>
        protected List<string> FieldList { get; set; }
        protected DynamicParameters Param { get; set; }
        public BaseExpressionVisitor()
        {
            FieldList = new List<string>();
            Param = new DynamicParameters();
        }
        /// <summary>
        /// 有+ - * /需要拼接的对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binary = new BinaryExpressionVisitor(node);
            GenerateField(binary.SpliceField.ToString());
            this.Param.AddDynamicParams(binary.Param);
            return node;
        }
        /// <summary>
        /// 成员对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            GenerateField(GetFieldValue(node));
            return node;
        }
        /// <summary>
        /// 值对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            var member = EntityCache.QueryEntity(node.Member.DeclaringType);
            string fieldName = member.FieldPairs[node.Member.Name];
            string field = member.Name + "." + fieldName;
            GenerateField(field);
            return node;
        }

        /// <summary>
        /// 待执行的方法对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType.FullName.Contains("Convert"))//使用convert函数里待执行的sql数据
            {
                Visit(node.Arguments[0]);
            }
            else
            {
                GenerateField(GetFieldValue(node));
            }
            return node;
        }
        /// <summary>
        /// 生成字段
        /// </summary>
        /// <param name="field"></param>
        protected virtual void GenerateField(string field)
        {
            FieldList.Add(field);
        }
        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string GetFieldValue(Expression expression)
        {
            object value;
            if (expression is ConstantExpression)
            {
                value = ((ConstantExpression)expression).Value;
            }
            else
            {
                value = expression.ToConvertAndGetValue();
            }

            if (expression.Type == typeof(string))
            {
                return "'" + value + "'";
            }
            else
            {
                return value.ToString();
            }
        }
    }
    /// <summary>
    /// 转用于解析二元表达式
    /// </summary>
    public class BinaryExpressionVisitor : BaseExpressionVisitor
    {
        internal StringBuilder SpliceField { get; set; }
        internal new DynamicParameters Param { get; set; }
        public BinaryExpressionVisitor(BinaryExpression expression)
        {
            SpliceField = new StringBuilder();
            Param = new DynamicParameters();
            SpliceField.Append("(");
            Visit(expression);
            SpliceField.Append(")");
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            SpliceField.Append(node.GetExpressionType());
            Visit(node.Right);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            SpliceField.Append(GetFieldValue(node));
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            var member = EntityCache.QueryEntity(node.Member.DeclaringType);
            string fieldName = member.FieldPairs[node.Member.Name];
            string field = member.Name + "." + fieldName;
            SpliceField.Append(field);
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //使用convert函数里待执行的sql数据
            if (node.Method.DeclaringType.FullName.Contains("Convert"))
            {
                Visit(node.Arguments[0]);
            }
            else if (node.Method.DeclaringType.FullName.Contains("Kogel.Dapper.Extension"))
            {
                DynamicParameters parameters = new DynamicParameters();
                SpliceField.Append("(" + node.MethodCallExpressionToSql(ref parameters) + ")");
                Param.AddDynamicParams(parameters);
            }
            else
            {
                SpliceField.Append("'" + GetFieldValue(node) + "'");
            }
            return node;
        }
    }
}
