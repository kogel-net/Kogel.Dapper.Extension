namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public abstract class IProviderOption
    {
        public IProviderOption(string openQuote, string closeQuote, char parameterPrefix)
        {
            OpenQuote = openQuote;
            CloseQuote = closeQuote;
            ParameterPrefix = parameterPrefix;
        }

        public string OpenQuote { get; set; }

        public string CloseQuote { get; set; }
        /// <summary>
        /// 参数标识
        /// </summary>
        public char ParameterPrefix { get; set; }
        /// <summary>
        /// 字段处理
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string CombineFieldName(string field)
        {
            return OpenQuote + field + CloseQuote;
        }
        public abstract string GetDate();
        /// <summary>
        /// 结合时间处理
        /// </summary>
        /// <param name="dateOption">时间操作</param>
        /// <param name="table">表名</param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public abstract string CombineDate(DateOption dateOption, string table, string field, string value);
    }
    public enum DateOption
    {
        AddYears,
        AddMonths,
        AddDays,
        AddHours,
        AddMinutes,
        AddSeconds
    }
}
