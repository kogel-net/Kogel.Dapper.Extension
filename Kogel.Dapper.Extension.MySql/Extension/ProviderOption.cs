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
    }
}
