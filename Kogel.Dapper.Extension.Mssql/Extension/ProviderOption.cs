using Kogel.Dapper.Extension.Core.Interfaces;
using System;

namespace Kogel.Dapper.Extension.MsSql.Extension
{
    public class ProviderOption : IProviderOption
    {
        public ProviderOption(string openQuote, string closeQuote, char parameterPrefix) : base(openQuote, closeQuote, parameterPrefix)
        {

        }
        public override string GetDate()
        {
            return "getdate()";
        }
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
    }
}
