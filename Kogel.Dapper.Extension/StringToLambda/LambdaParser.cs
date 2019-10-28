using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Lenic.DI.Core;

namespace StringToLambda
{
    /// <summary>
    /// Lambda 表达式分析器
    /// </summary>
    [DebuggerStepThrough]
    public static class LambdaParser
    {
        /// <summary>
        /// 将源字符串分析为 Lambda 表达式树.
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="source">用于分析的源字符串.</param>
        /// <param name="ns">分析过程中可能用到的命名空间列表.</param>
        /// <returns>分析完成的 Lambda 表达式树.</returns>
        public static Expression<T> Parse<T>(string source, params string[] ns)
        {
            var parseResult = SymbolParser.Build(source);
            var parser = new ExpressionParserCore(parseResult, typeof(T), ns, null);

            return parser.ToLambdaExpression<T>();
        }

        /// <summary>
        /// 将源字符串分析为委托.
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="source">用于分析的源字符串.</param>
        /// <param name="ns">分析过程中可能用到的命名空间列表.</param>
        /// <returns>分析委托.</returns>
        public static T Compile<T>(string source, params string[] ns)
        {
            var parseResult = SymbolParser.Build(source);
            var parser = new ExpressionParserCore(parseResult, typeof(T), ns, null);

            return parser.ToLambdaExpression<T>().Compile();
        }

        /// <summary>
        /// 将源字符串分析为委托.
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="source">用于分析的源字符串.</param>
        /// <param name="assemblies">可能用到的程序集列表.</param>
        /// <param name="ns">分析过程中可能用到的命名空间列表.</param>
        /// <returns>
        /// 分析委托.
        /// </returns>
        public static T Compile<T>(string source, Assembly[] assemblies, params string[] ns)
        {
            var parseResult = SymbolParser.Build(source);
            var parser = new ExpressionParserCore(parseResult, typeof(T), ns, assemblies);

            return parser.ToLambdaExpression<T>().Compile();
        }
    }
}
