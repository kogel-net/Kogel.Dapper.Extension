using Kogel.Dapper.Extension.Core.Interfaces;
using System;

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
        public override string CombineDate(DateOption dateOption,string table, string field, string value)
        {
            string result = string.Empty;
            switch (dateOption)
            {
                case DateOption.AddYears:
                    {
                        result = $"date_add({table}.{field}, interval {value} year)";
                    }
                    break;
                case DateOption.AddMonths:
                    {
                        result = $"date_add({table}.{field}, interval {value} month)";
                    }
                    break;
                case DateOption.AddDays:
                    {
                        result = $"date_add({table}.{field}, interval {value} day)";
                    }
                    break;
                case DateOption.AddHours:
                    {
                        result = $"date_add({table}.{field}, interval {value} hour)";
                    }
                    break;
                case DateOption.AddMinutes:
                    {
                        result = $"date_add({table}.{field}, interval {value} minute)";
                    }
                    break;
                case DateOption.AddSeconds:
                    {
                        result = $"date_add({table}.{field}, interval {value} second)";
                    }
                    break;
            }
            return result;
        }
    }
}
