using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Lenic.DI.Core
{
    /// <summary>
    /// Symbol Parser
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("CurrentPosition = {CurrentPosition}, Source = {Source}")]
    public sealed class SymbolParser
    {
        #region Fields And Properties
        /// <summary>
        /// Gets the source.
        /// </summary>
        public string Source { get; private set; }
        /// <summary>
        /// Gets the current position.
        /// </summary>
        public int CurrentPosition { get; private set; }
        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// Gets the current char.
        /// </summary>
        public char CurrentChar { get; private set; }

        private Token currentToken;
        /// <summary>
        /// Gets the current token.
        /// </summary>
        public Token CurrentToken { get { return currentToken; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolParser"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public SymbolParser(string source)
        {
            if (ReferenceEquals(null, source))
                throw new ArgumentNullException("source");

            Source = source;
            Length = source.Length;
            SetPosition(0);
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="index">The index.</param>
        public void SetPosition(int index)
        {
            CurrentPosition = index;
            CurrentChar = CurrentPosition < Length ? Source[CurrentPosition] : '\0';
        }

        /// <summary>
        /// Nexts the char.
        /// </summary>
        public void NextChar()
        {
            if (CurrentPosition < Length) CurrentPosition++;
            CurrentChar = CurrentPosition < Length ? Source[CurrentPosition] : '\0';
        }

        /// <summary>
        /// Nexts the token.
        /// </summary>
        /// <returns></returns>
        public Token NextToken()
        {
            while (Char.IsWhiteSpace(CurrentChar)) NextChar();
            TokenId t;
            int tokenPos = CurrentPosition;
            switch (CurrentChar)
            {
                case '!':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (CurrentChar == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (CurrentChar == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else if (CurrentChar == '>')
                    {
                        NextChar();
                        t = TokenId.LambdaPrefix;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    if (CurrentChar == '?')
                    {
                        NextChar();
                        t = TokenId.DoubleQuestion;
                    }
                    else
                    {
                        t = TokenId.Question;
                    }
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '{':
                    NextChar();
                    t = TokenId.OpenBrace;
                    break;
                case '}':
                    NextChar();
                    t = TokenId.CloseBrace;
                    break;
                case '|':
                    NextChar();
                    if (CurrentChar == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    char quote = CurrentChar;
                    do
                    {
                        NextChar();
                        while (CurrentPosition < Length && CurrentChar != quote) NextChar();
                        if (CurrentPosition == Length)
                            throw ParseError(CurrentPosition, "Unterminated string literal");
                        NextChar();
                    } while (CurrentChar == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (Char.IsLetter(CurrentChar) || CurrentChar == '@' || CurrentChar == '_')
                    {
                        do
                        {
                            NextChar();
                        } while (Char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '?');
                        t = TokenId.Identifier;
                        break;
                    }
                    if (Char.IsDigit(CurrentChar))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (Char.IsDigit(CurrentChar));

                        if (CurrentChar == 'l' || CurrentChar == 'L')
                        {
                            t = TokenId.LongIntegerLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'f' || CurrentChar == 'F')
                        {
                            t = TokenId.SingleRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'm' || CurrentChar == 'M')
                        {
                            t = TokenId.DecimalRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'd' || CurrentChar == 'D')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            break;
                        }

                        if (CurrentChar == '.')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(CurrentChar));
                        }

                        if (CurrentChar == 'E' || CurrentChar == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (CurrentChar == '+' || CurrentChar == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(CurrentChar));
                        }

                        if (CurrentChar == 'F' || CurrentChar == 'f')
                        {
                            t = TokenId.SingleRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'm' || CurrentChar == 'M')
                        {
                            t = TokenId.DecimalRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'd' || CurrentChar == 'D')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            break;
                        }

                        break;
                    }
                    if (CurrentPosition == Length)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(CurrentPosition, "Syntax error '{0}'", CurrentChar);
            }
            currentToken.ID = t;
            currentToken.Text = Source.Substring(tokenPos, CurrentPosition - tokenPos);
            currentToken.Index = tokenPos;

            return new Token { ID = t, Text = currentToken.Text, Index = tokenPos, };
        }

        /// <summary>
        /// Builds the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The Build result.</returns>
        public static SymbolParseResult Build(string source)
        {
            var item = new SymbolParser(source);
            List<Token> data = new List<Token>();
            while (true)
            {
                var token = item.NextToken();
                data.Add(token);
                if (token.ID == TokenId.End)
                    break;
            }
            return new SymbolParseResult(data);
        }
        #endregion

        #region Private Methods
        private void ValidateDigit()
        {
            if (!Char.IsDigit(CurrentChar)) throw ParseError(CurrentPosition, "Digit expected");
        }

        private Exception ParseError(string format, params object[] args)
        {
            return ParseError(currentToken.Index, format, args);
        }

        private Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }
        #endregion
    }
}
