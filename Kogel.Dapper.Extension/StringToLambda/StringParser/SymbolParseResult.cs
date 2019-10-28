using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Lenic.DI.Core
{
    /// <summary>
    /// Symbol Parse Result
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DebuggerDisplay("{ToString()}")]
    public class SymbolParseResult : ReadOnlyCollection<Token>
    {
        #region Private Fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxIndex = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _lastIndex = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _index = -1;
        #endregion

        #region Constuction
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolParseResult"/> class.
        /// </summary>
        internal SymbolParseResult()
            : base(new List<Token>())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolParseResult"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        internal SymbolParseResult(IList<Token> list)
            : base(list)
        {
            _maxIndex = list.Count - 1;
        }
        #endregion

        #region Business Properties
        /// <summary>
        /// 获取或设置当前读取索引
        /// </summary>
        public int Index
        {
            get { return _index; }
            private set
            {
                _lastIndex = _index;
                _index = value;
            }
        }
        /// <summary>
        /// 获取当前读取中的字符单元
        /// </summary>
        public Token Current
        {
            get
            {
                if (Index < 0 || Index > _maxIndex)
                    return Token.Empty;

                return this[Index];
            }
        }
        /// <summary>
        /// 获取完整的字符串表达式
        /// </summary>
        private string StringExpression
        {
            get { return string.Join(" ", this); }
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// 读取下一个字符单元, 同时读取索引前进.
        /// </summary>
        /// <returns>读取得到的字符单元</returns>
        public Token Next()
        {
            Token token;
            if (TryGetElement(out token, Index + 1))
                return token;
            else
                return Token.Empty;
        }

        /// <summary>
        /// 判断下一个字符单元是否是指定的类型, 同时读取索引前进.
        /// </summary>
        /// <param name="tokenId">期待得到的字符单元类型.</param>
        /// <param name="throwIfNot">如果设置为 <c>true</c> 表示抛出异常. 默认为 <c>false</c> 表示不抛出异常.</param>
        /// <returns><c>true</c> 表示读取的单元类型和期待的单元类型一致; 否则返回 <c>false</c> .</returns>
        public bool NextIs(TokenId tokenId, bool throwIfNot = false)
        {
            var result = Next().ID == tokenId;
            if (!result && throwIfNot)
                throw new ApplicationException(string.Format("next is not {0}", tokenId));
            return result;
        }

        /// <summary>
        /// 尝试读取下一个字符单元, 但并不前进.
        /// </summary>
        /// <param name="count">尝试读取的当前字符单元的后面第几个单元, 默认为后面第一个单元.</param>
        /// <returns>读取得到的字符单元.</returns>
        public Token PeekNext(int count = 1)
        {
            Token token;
            if (PeekGetElement(out token, Index + count))
                return token;
            else
                return Token.Empty;
        }

        /// <summary>
        /// 判断下一个字符单元是否是指定的类型, 但读取索引不前进.
        /// </summary>
        /// <param name="tokenId">期待得到的字符单元类型.</param>
        /// <param name="count">判断当前字符后面第几个是指定的字符单元类型, 默认值为 1 .</param>
        /// <param name="throwIfNot">如果设置为 <c>true</c> 表示抛出异常. 默认为 <c>false</c> 表示不抛出异常.</param>
        /// <returns>
        /// 	<c>true</c> 表示读取的单元类型和期待的单元类型一致; 否则返回 <c>false</c> .
        /// </returns>
        public bool PeekNextIs(TokenId tokenId, int count = 1, bool throwIfNot = false)
        {
            var result = PeekNext(count).ID == tokenId;
            if (!result && throwIfNot)
                throw new ApplicationException(string.Format("next is not {0}", tokenId));
            return result;
        }

        /// <summary>
        /// 前进跳过指定的字符单元.
        /// </summary>
        /// <param name="count">The count.</param>
        public void Skip(int count = 1)
        {
            count = Index + count;
            CheckIndexOut(count);

            Index = count;
        }

        /// <summary>
        /// 读取直到符合 predicate 的条件时停止.
        /// </summary>
        /// <param name="predicate">比较当前 Token 是否符合条件的方法.</param>
        /// <returns>读取停止时的 Token 列表.</returns>
        public IList<Token> SkipUntil(Func<Token, bool> predicate)
        {
            List<Token> data = new List<Token>();
            while (!predicate(Current) || Current.ID == TokenId.End)
                data.Add(Next());
            return data;
        }

        /// <summary>
        /// 返回到指定的读取索引.
        /// </summary>
        /// <param name="index">目标读取索引.</param>
        public void ReturnToIndex(int index)
        {
            if (index < -1 || index > _maxIndex)
                throw new IndexOutOfRangeException();

            Index = index;
        }
        #endregion

        #region Private Methods
        private bool TryGetElement(out Token token, int index)
        {
            bool result = PeekGetElement(out token, index);
            if (result)
                Index = index;
            return result;
        }

        private bool PeekGetElement(out Token token, int index)
        {
            if (index < 0 || index > _maxIndex)
            {
                token = Token.Empty;
                return false;
            }
            else
            {
                token = this[index];
                return true;
            }
        }

        private void CheckIndexOut(int index)
        {
            if (index < 0 || index > _maxIndex)
                throw new IndexOutOfRangeException();
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Join(" ", this.TakeWhile(p => p.Index < Current.Index));
        }
        #endregion
    }
}
