using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension
{
	/// <summary>
	/// 函数列表
	/// </summary>
	public static class Function
	{
		public static int Count<T>(T countExpression)
		{
			return 0;
		}

		public static T Sum<T>(T sumExpression)
		{
			return default(T);
		}

		public static T Max<T>(T maxExpression)
		{
			return default(T);
		}

		public static T Min<T>(T minExpression)
		{
			return default(T);
		}

		public static T Avg<T>(T avgExpression)
		{
			return default(T);
		}
	}
}
