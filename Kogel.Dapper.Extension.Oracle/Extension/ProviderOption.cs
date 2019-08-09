using Kogel.Dapper.Extension.Core.Interfaces;
using System;

namespace Kogel.Dapper.Extension.Oracle.Extension
{
    public class ProviderOption : IProviderOption
    {
        public ProviderOption(string openQuote, string closeQuote, char parameterPrefix) : base(openQuote, closeQuote, parameterPrefix)
        {

        }
        public override string GetDate()
        {
            return "sysdate()";
        }
        public override string CombineDate(DateOption dateOption, string table, string field, string value)
        {
            string result = string.Empty;
            switch (dateOption)
            {
                case DateOption.AddYears:
                    {
                        result = $"add_months({table}.{field},{value}*12)";
                    }
                    break;
                case DateOption.AddMonths:
                    {
                        result = $"add_months({table}.{field},{value})";
                    }
                    break;
                case DateOption.AddDays:
                    {
                        result = $"(({table}.{field})+{value})";
                    }
                    break;
                case DateOption.AddHours:
                    {
                        result = $"(({table}.{field})+({value}/24))";
                    }
                    break;
                case DateOption.AddMinutes:
                    {
                        result = $"(({table}.{field})+({value}/24/60))";
                    }
                    break;
                case DateOption.AddSeconds:
                    {
                        result = $"(({table}.{field})+({value}/24/60/60))";
                    }
                    break;
            }
            return result;
        }
    }
}
