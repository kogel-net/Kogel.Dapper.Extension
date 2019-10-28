using System.Diagnostics;

namespace Lenic.DI.Core
{
    /// <summary>
    /// 【类型：变量】键值对
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("{Type.Text}, {Variable.Text}")]
    public class TypeVariable
    {
        /// <summary>
        /// 获取类型字符单元.
        /// </summary>
        public Token Type { get; private set; }
        /// <summary>
        /// 获取变量字符单元.
        /// </summary>
        public Token Variable { get; private set; }
        /// <summary>
        /// 获取一个值, 通过该值指示是否包含类型定义. <c>true</c> 表示包含类型定义.
        /// </summary>
        public bool ExistType { get; private set; }

        /// <summary>
        /// 初始化新建一个 <see cref="TypeVariable"/> 类的实例对象.
        /// </summary>
        /// <param name="type">类型字符单元.</param>
        /// <param name="variable">变量字符单元.</param>
        public TypeVariable(Token? type, Token variable)
        {
            Variable = variable;
            if (ExistType = type.HasValue)
                Type = type.Value;
        }
    }
}
