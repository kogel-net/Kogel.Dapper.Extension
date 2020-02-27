using System;

namespace Kogel.Dapper.Extension
{
    public static class ExpressExpansion
    {
        public static bool In(this int input, int[] array)
        {
            return true;
        }
		public static bool In(this long input, long[] array)
		{
			return true;
		}
		public static bool In(this string input, string[] array)
        {
            return true;
        }
		public static bool In(this Guid input, Guid[] array)
		{
			return true;
		}
		public static bool NotIn(this int input, int[] array)
        {
            return true;
        }
		public static bool NotIn(this long input, long[] array)
		{
			return true;
		}
		public static bool NotIn(this string input, string[] array)
        {
            return true;
        }
		public static bool NotIn(this Guid input, Guid[] array)
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
		public static bool Between(this long input, long from, long to)
		{
			return true;
		}
		public static bool Between(this DateTime input, DateTime from, DateTime to)
        {
            return true;
        }
    }
	/// <summary>
	/// 聚合函数
	/// </summary>
	public class Group
	{
		public static int Count<T>(T field)
		{
			return 0;
		}
		public static T Sum<T>(T field)
		{
			return default(T);
		}
		public static T Max<T>(T field)
		{
			return default(T);
		}
		public static T Min<T>(T field)
		{
			return default(T);
		}
		public static T Avg<T>(T field)
		{
			return default(T);
		}
	}
}
