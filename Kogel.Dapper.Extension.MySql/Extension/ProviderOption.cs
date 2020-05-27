using Kogel.Dapper.Extension.Core.Interfaces;
using System;
using System.Text;

namespace Kogel.Dapper.Extension.MySql.Extension
{
	public class ProviderOption : IProviderOption
	{
		public ProviderOption(string openQuote, string closeQuote, char parameterPrefix) : base(openQuote, closeQuote, parameterPrefix)
		{

		}
		public override string GetDate()
		{
			return "now()";
		}
		/// <summary>
		/// 结合转换处理
		/// </summary>
		/// <param name="convertOption"></param>
		/// <param name="spliceField"></param>
		/// <param name="fieldInkove"></param>
		public override void CombineConvert(ConvertOption convertOption, StringBuilder spliceField, Action fieldInkove)
		{
			switch (convertOption)
			{
				case ConvertOption.ToBoolean:
					{
						fieldInkove.Invoke();
						//spliceField.Append(" not in ('0','0')");
					}
					break;
				case ConvertOption.ToDecimal:
					{
						spliceField.Append(" cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as decimal(36,18))");
					}
					break;
				case ConvertOption.ToDouble:
					{
						spliceField.Append(" cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as decimal(32,16))");
					}
					break;
				case ConvertOption.ToInt32:
					{
						spliceField.Append("cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as signed)");
					}
					break;
				case ConvertOption.ToString:
					{
						spliceField.Append("cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as char)");
					}
					break;
				case ConvertOption.ToDateTime:
					{
						spliceField.Append("cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as datetime)");
					}
					break;
			}
		}
		/// <summary>
		/// 结合时间处理
		/// </summary>
		/// <param name="dateOption"></param>
		/// <param name="spliceField"></param>
		/// <param name="fieldInkove"></param>
		/// <param name="valueInkove"></param>
		public override void CombineDate(DateOption dateOption, StringBuilder spliceField, Action fieldInkove, Action valueInkove)
		{
			//string result = string.Empty;
			switch (dateOption)
			{
				case DateOption.AddYears:
					{
						//result = $"date_add({field}, interval {value} year)";
						spliceField.Append(" date_add(");
						fieldInkove.Invoke();
						spliceField.Append(", interval ");
						valueInkove.Invoke();
						spliceField.Append(" year)");
					}
					break;
				case DateOption.AddMonths:
					{
						//result = $"date_add({field}, interval {value} month)";
						spliceField.Append(" date_add(");
						fieldInkove.Invoke();
						spliceField.Append(", interval ");
						valueInkove.Invoke();
						spliceField.Append(" month)");
					}
					break;
				case DateOption.AddDays:
					{
						//result = $"date_add({field}, interval {value} day)";
						spliceField.Append(" date_add(");
						fieldInkove.Invoke();
						spliceField.Append(", interval ");
						valueInkove.Invoke();
						spliceField.Append(" day)");
					}
					break;
				case DateOption.AddHours:
					{
						//result = $"date_add({field}, interval {value} hour)";
						spliceField.Append(" date_add(");
						fieldInkove.Invoke();
						spliceField.Append(", interval ");
						valueInkove.Invoke();
						spliceField.Append(" hour)");
					}
					break;
				case DateOption.AddMinutes:
					{
						//result = $"date_add({field}, interval {value} minute)";
						spliceField.Append(" date_add(");
						fieldInkove.Invoke();
						spliceField.Append(", interval ");
						valueInkove.Invoke();
						spliceField.Append(" minute)");
					}
					break;
				case DateOption.AddSeconds:
					{
						//result = $"date_add({field}, interval {value} second)";
						spliceField.Append(" date_add(");
						fieldInkove.Invoke();
						spliceField.Append(", interval ");
						valueInkove.Invoke();
						spliceField.Append(" second)");
					}
					break;
			}
			//return result;
		}

		/// <summary>
		/// if null 函数
		/// </summary>
		/// <returns></returns>
		public override string IfNull()
		{
			return "IfNull";
		}
	}
}
