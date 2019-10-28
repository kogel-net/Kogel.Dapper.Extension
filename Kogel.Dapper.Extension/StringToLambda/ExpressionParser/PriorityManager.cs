using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Lenic.DI.Core
{
    [DebuggerStepThrough]
    internal static class PriorityManager
    {
        private static readonly Dictionary<string, int> operatorCache = null;

        private static readonly Dictionary<Type, int> numericCache = null;

        static PriorityManager()
        {
            operatorCache = new Dictionary<string, int>();

            operatorCache.Add("(", 100);
            operatorCache.Add(")", 100);
            operatorCache.Add("[", 100);
            operatorCache.Add("]", 100);

            operatorCache.Add(".", 13);
            operatorCache.Add("function()", 13);
            operatorCache.Add("index[]", 13);
            operatorCache.Add("++behind", 13);
            operatorCache.Add("--behind", 13);
            operatorCache.Add("new", 13);
            operatorCache.Add("typeof", 13);
            operatorCache.Add("checked", 13);
            operatorCache.Add("unchecked", 13);
            operatorCache.Add("->", 13);

            operatorCache.Add("++before", 12);
            operatorCache.Add("--before", 12);
            operatorCache.Add("+before", 12);
            operatorCache.Add("-before", 12);
            operatorCache.Add("!", 12);
            operatorCache.Add("~", 12);
            operatorCache.Add("convert()", 12);
            operatorCache.Add("sizeof", 12);

            operatorCache.Add("*", 11);
            operatorCache.Add("/", 11);
            operatorCache.Add("%", 11);
            operatorCache.Add("+", 10);
            operatorCache.Add("-", 10);
            operatorCache.Add("<<", 9);
            operatorCache.Add(">>", 9);
            operatorCache.Add(">", 8);
            operatorCache.Add("<", 8);
            operatorCache.Add(">=", 8);
            operatorCache.Add("<=", 8);
            operatorCache.Add("is", 8);
            operatorCache.Add("as", 8);
            operatorCache.Add("==", 7);
            operatorCache.Add("!=", 7);
            operatorCache.Add("&", 6);
            operatorCache.Add("^", 6);
            operatorCache.Add("|", 6);
            operatorCache.Add("&&", 5);
            operatorCache.Add("||", 5);
            operatorCache.Add("?", 5);
            operatorCache.Add("??", 4);
            operatorCache.Add("=", 4);
            operatorCache.Add("+=", 4);
            operatorCache.Add("-=", 4);
            operatorCache.Add("*=", 4);
            operatorCache.Add("/=", 4);
            operatorCache.Add("%=", 4);
            operatorCache.Add("&=", 4);
            operatorCache.Add("|=", 4);
            operatorCache.Add("^=", 4);
            operatorCache.Add(">>=", 4);
            operatorCache.Add("<<=", 4);

            numericCache = new Dictionary<Type, int>();

            numericCache.Add(typeof(byte), 1);
            numericCache.Add(typeof(short), 2);
            numericCache.Add(typeof(ushort), 3);
            numericCache.Add(typeof(int), 4);
            numericCache.Add(typeof(uint), 5);
            numericCache.Add(typeof(long), 6);
            numericCache.Add(typeof(ulong), 7);
            numericCache.Add(typeof(float), 8);
            numericCache.Add(typeof(double), 9);
            numericCache.Add(typeof(decimal), 10);
        }

        private static int GetOperatorLevel(string @operator, bool isBefore)
        {
            switch (@operator)
            {
                case "++":
                case "--":
                    @operator += isBefore ? "before" : "behind";
                    break;

                case "+":
                case "-":
                    @operator += isBefore ? "before" : null;
                    break;
            }
            return operatorCache[@operator];
        }

        public static int GetOperatorLevel(Token token)
        {
            if (string.IsNullOrEmpty(token.Text) ||
                token.ID == TokenId.CloseBrace ||
                token.ID == TokenId.Comma ||
                token.ID == TokenId.Colon)
                return -1;

            return GetOperatorLevel(token.Text, false);
        }

        public static int GetNumericLevel(Type type)
        {
            return numericCache[type.GetNoneNullableType()];
        }

        public static bool IsValueType(Type type)
        {
            return numericCache.ContainsKey(type.GetNoneNullableType());
        }
    }
}
