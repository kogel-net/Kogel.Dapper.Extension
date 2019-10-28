using System;
using System.Diagnostics;

namespace Lenic.DI.Core
{
    /// <summary>
    /// 分析语法错误类
    /// </summary>
    [DebuggerStepThrough]
    public class ParserSyntaxErrorException : Exception
    {
        /// <summary>
        /// 初始化新建一个 <see cref="ParserSyntaxErrorException"/> 类的实例对象.
        /// </summary>
        public ParserSyntaxErrorException()
            : base("syntax error!") { }
    }
}
