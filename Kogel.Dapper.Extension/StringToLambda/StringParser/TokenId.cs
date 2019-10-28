
namespace Lenic.DI.Core
{
    /// <summary>
    /// 字符单元类型
    /// </summary>
    public enum TokenId
    {
        /// <summary>
        /// End
        /// </summary>
        End,
        /// <summary>
        /// Identifier
        /// </summary>
        Identifier,
        /// <summary>
        /// String
        /// </summary>
        StringLiteral,
        /// <summary>
        /// Integer Literal
        /// </summary>
        IntegerLiteral,
        /// <summary>
        /// Long Integer Literal
        /// </summary>
        LongIntegerLiteral,
        /// <summary>
        /// Single Real Literal
        /// </summary>
        SingleRealLiteral,
        /// <summary>
        /// Decimal Real Literal
        /// </summary>
        DecimalRealLiteral,
        /// <summary>
        /// Real Literal
        /// </summary>
        RealLiteral,
        /// <summary>
        /// !
        /// </summary>
        Exclamation,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// &amp;
        /// </summary>
        Amphersand,
        /// <summary>
        /// (
        /// </summary>
        OpenParen,
        /// <summary>
        /// )
        /// </summary>
        CloseParen,
        /// <summary>
        /// *
        /// </summary>
        Asterisk,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// /
        /// </summary>
        Slash,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// &lt;
        /// </summary>
        LessThan,
        /// <summary>
        /// =
        /// </summary>
        Equal,
        /// <summary>
        /// &gt;
        /// </summary>
        GreaterThan,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        /// <summary>
        /// ??
        /// </summary>
        DoubleQuestion,
        /// <summary>
        /// [
        /// </summary>
        OpenBracket,
        /// <summary>
        /// ]
        /// </summary>
        CloseBracket,
        /// <summary>
        /// |
        /// </summary>
        Bar,
        /// <summary>
        /// !=
        /// </summary>
        ExclamationEqual,
        /// <summary>
        /// &amp;&amp;
        /// </summary>
        DoubleAmphersand,
        /// <summary>
        /// &lt;=
        /// </summary>
        LessThanEqual,
        /// <summary>
        /// &lt;&gt; 
        /// </summary>
        LessGreater,
        /// <summary>
        /// ==
        /// </summary>
        DoubleEqual,
        /// <summary>
        /// &gt;=
        /// </summary>
        GreaterThanEqual,
        /// <summary>
        /// ||
        /// </summary>
        DoubleBar,
        /// <summary>
        /// =&gt;
        /// </summary>
        LambdaPrefix,
        /// <summary>
        /// {
        /// </summary>
        OpenBrace,
        /// <summary>
        /// }
        /// </summary>
        CloseBrace,
    }
}
