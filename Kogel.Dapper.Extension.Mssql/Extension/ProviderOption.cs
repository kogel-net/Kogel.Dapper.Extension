using Kogel.Dapper.Extension.Core.Interfaces;
using System;
using System.Linq.Expressions;

namespace Kogel.Dapper.Extension.MsSql.Extension
{
    public class ProviderOption : IProviderOption
    {
        public ProviderOption(string openQuote, string closeQuote, char parameterPrefix) : base(openQuote, closeQuote, parameterPrefix)
        {

        }
		/// <summary>
		/// 获取当前时间
		/// </summary>
		/// <returns></returns>
        public override string GetDate()
        {
            return "getdate()";
        }
		/// <summary>
		/// 时间转义
		/// </summary>
		/// <param name="dateOption"></param>
		/// <param name="table"></param>
		/// <param name="field"></param>
		/// <param name="value"></param>
		/// <returns></returns>
        public override string CombineDate(DateOption dateOption, string table, string field,string value)
        {
            string result = string.Empty;
            switch (dateOption)
            {
                case DateOption.AddYears:
                    {
                        result = $"dateadd(yy,{value},{table}.{field})";
                    }
                    break;
                case DateOption.AddMonths:
                    {
                        result = $"dateadd(mm,{value},{table}.{field})";
                    }
                    break;
                case DateOption.AddDays:
                    {
                        result = $"dateadd(dd,{value},{table}.{field})";
                    }
                    break;
                case DateOption.AddHours:
                    {
                        result = $"dateadd(hh,{value},{table}.{field})";
                    }
                    break;
                case DateOption.AddMinutes:
                    {
                        result = $"dateadd(minute,{value},{table}.{field})";
                    }
                    break;
                case DateOption.AddSeconds:
                    {
                        result = $"dateadd(ss,{value},{table}.{field})";
                    }
                    break;
            }
            return result;
        }
		/// <summary>
		/// 模糊转义
		/// </summary>
		/// <param name="value"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public override object FuzzyEscaping(object value, ref string param)
		{
			if (value != null && value.ToString() != string.Empty)
			{
				value = $@"%\{value}%";
				param += @" ESCAPE'\'";
			}
			else
			{
				value = $@"%{value}%";
			}
			return value;
		}
	}
}
