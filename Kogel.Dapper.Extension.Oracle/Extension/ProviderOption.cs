using Dapper;
using Kogel.Dapper.Extension.Core.Interfaces;
using System;
using System.Data;
using System.Text;

namespace Kogel.Dapper.Extension.Oracle.Extension
{
    public class ProviderOption : IProviderOption
    {
        public ProviderOption(string openQuote, string closeQuote, char parameterPrefix) : base(openQuote, closeQuote, parameterPrefix)
        {

        }
        public override string GetDate()
        {
            return "sysdate";
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
						//spliceField.Append(" not in ('0','false')");
					}
					break;
				case ConvertOption.ToDecimal:
					{
						spliceField.Append(" cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as number)");
					}
					break;
				case ConvertOption.ToDouble:
					{
						spliceField.Append(" cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as number)");
					}
					break;
				case ConvertOption.ToInt32:
					{
						spliceField.Append(" cast(");
						fieldInkove.Invoke();
						spliceField.Append(" as number)");
					}
					break;
				case ConvertOption.ToString:
					{
						spliceField.Append("to_char(");
						fieldInkove.Invoke();
						spliceField.Append(" )");
					}
					break;
				case ConvertOption.ToDateTime:
					{
						spliceField.Append("to_date(");
						fieldInkove.Invoke();
						spliceField.Append(",'YYYY-MM-D HH24:MI:SS')");
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
                        //result = $"add_months({field},{value}*12)";
						spliceField.Append("add_months(");
						fieldInkove.Invoke();
						spliceField.Append(",");
						valueInkove.Invoke();
						spliceField.Append("*12)");
					}
                    break;
                case DateOption.AddMonths:
                    {
                        //result = $"add_months({field},{value})";
						spliceField.Append("add_months(");
						fieldInkove.Invoke();
						spliceField.Append(",");
						valueInkove.Invoke();
						spliceField.Append(")");
					}
                    break;
                case DateOption.AddDays:
                    {
                        //result = $"(({field})+{value})";
						spliceField.Append("((");
						fieldInkove.Invoke();
						spliceField.Append(")+");
						valueInkove.Invoke();
						spliceField.Append(")");
					}
                    break;
                case DateOption.AddHours:
                    {
                        //result = $"(({field})+({value}/24))";
						spliceField.Append("((");
						fieldInkove.Invoke();
						spliceField.Append(")+");
						valueInkove.Invoke();
						spliceField.Append("/24)");
					}
                    break;
                case DateOption.AddMinutes:
                    {
						//result = $"(({field})+({value}/24/60))";
						spliceField.Append("((");
						fieldInkove.Invoke();
						spliceField.Append(")+");
						valueInkove.Invoke();
						spliceField.Append("/24/60)");
					}
                    break;
                case DateOption.AddSeconds:
                    {
						//result = $"(({field})+({value}/24/60/60))";
						spliceField.Append("((");
						fieldInkove.Invoke();
						spliceField.Append(")+");
						valueInkove.Invoke();
						spliceField.Append("/24/60/60)");
					}
                    break;
            }
            //return result;
        }

		/// <summary>
		/// is null 函数
		/// </summary>
		/// <returns></returns>
		public override string IfNull()
		{
			return "NVL";
		}
	}

	public class BoolTypeHanlder : SqlMapper.TypeHandler<bool>
	{
		public override void SetValue(IDbDataParameter parameter, bool value)
		{
			parameter.DbType = DbType.Int16;
			if (value)
				parameter.Value = 1;
			else
				parameter.Value = 0;
		}

		public override bool Parse(object value)
		{
			return Convert.ToBoolean(value);
		}
	}
}
