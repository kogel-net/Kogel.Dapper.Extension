using System.Diagnostics;

namespace Lenic.DI.Core
{
    /// <summary>
    /// 字符单元
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("Text = {Text}, ID = {ID}, Index = {Index}")]
    public struct Token
    {
        #region Fields
        /// <summary>
        /// 空的字符单元
        /// </summary>
        public static readonly Token Empty = new Token();
        private TokenId id;
        private string text;
        private int index;
        private int? hash;
        #endregion

        #region Properties
        /// <summary>
        /// 获取或设置字符类型
        /// </summary>
        public TokenId ID
        {
            get { return id; }
            set
            {
                id = value;
                hash = null;
            }
        }
        /// <summary>
        /// 获取或设置当前字符单元的文本表示
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                hash = null;
            }
        }
        /// <summary>
        /// 获取或设置当前字符单元在整体结果中的索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                hash = null;
            }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (obj is Token)
                return Equals((Token)obj);
            else
                return false;
        }

        /// <summary>
        /// Equalses the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public bool Equals(Token token)
        {
            if (ReferenceEquals(token, null)) return false;
            return ID == token.id && Text == token.Text;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                if (!hash.HasValue)
                {
                    hash = ID.GetHashCode();
                    hash ^= Text.GetHashCode();
                    hash ^= Index.GetHashCode();
                }
                return hash.Value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return text;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Lenic.DI.Core.Token"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Token value)
        {
            return value.text;
        }
        #endregion

        #region Exception Throw
        /// <summary>
        /// 如果当前实例的文本表示与指定的字符串不符, 则抛出异常.
        /// </summary>
        /// <param name="id">待判断的字符串.</param>
        /// <returns>当前实例对象.</returns>
        public Token Throw(TokenId id)
        {
            if (ID != id)
                throw new ParserSyntaxErrorException();

            return this;
        }

        /// <summary>
        /// 如果当前实例的字符类型与指定的字符类型不符, 则抛出异常.
        /// </summary>
        /// <param name="id">待判断的目标类型的字符类型.</param>
        /// <returns>当前实例对象.</returns>
        public Token Throw(string text)
        {
            if (Text != text)
                throw new ParserSyntaxErrorException();

            return this;
        }
        #endregion
    }
}
