using Kogel.Dapper.Extension.Core.Interfaces;
using System;
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
    }
}
