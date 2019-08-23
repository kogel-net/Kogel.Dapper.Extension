using System;

namespace Kogel.Dapper.Extension
{
    public static class ExpressExpansion
    {
        public static bool In(this int input, int[] array)
        {
            return true;
        }
        public static bool In(this string input, string[] array)
        {
            return true;
        }
        public static bool NotIn(this int input, int[] array)
        {
            return true;
        }
        public static bool NotIn(this string input, string[] array)
        {
            return true;
        }
        public static bool IsNull(this object input)
        {
            return true;
        }
        public static bool IsNotNull(this object input)
        {
            return true;
        }
        public static bool Between(this int input,int from,int to)
        {
            return true;
        }
        public static bool Between(this DateTime input, DateTime from, DateTime to)
        {
            return true;
        }
    }
}
