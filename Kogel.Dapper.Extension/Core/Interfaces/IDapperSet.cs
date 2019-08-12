using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Model;
using Dapper;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public abstract class AbstractSet
    {
        public Type TableType { get; protected set; }
        /// <summary>
        /// [已弃用]只用来生成对象
        /// </summary>
        public LambdaExpression WhereExpression { get; protected set; }

        public List<LambdaExpression> WhereExpressionList { get; set; }
        //表达式排序集合
        public Dictionary<LambdaExpression, EOrderBy> OrderbyExpressionList { get; protected set; }
        /// <summary>
        /// 字符串排序
        /// </summary>
        public StringBuilder OrderbyBuilder { get; protected set; }

        public LambdaExpression SelectExpression { get; internal set; }

        public bool NoLock { get; protected set; }

        #region sql字符串对象
        public StringBuilder WhereBuilder { get; protected set; }

        public DynamicParameters Params { get; set; }
        #endregion
    }
}
