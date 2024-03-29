﻿using System;
using System.Text;
using Kogel.Dapper.Extension.Core.Interfaces;

namespace Kogel.Dapper.Extension.SQLite.Extension
{
    public class ProviderOption : IProviderOption
    {
        public ProviderOption(string openQuote, string closeQuote, char parameterPrefix) : base(openQuote, closeQuote, parameterPrefix)
        {

        }
        public override string GetDate()
        {
            return "datetime(CURRENT_TIMESTAMP, 'localtime')";
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
                        spliceField.Append(" ROUND(");
                        fieldInkove.Invoke();
                        spliceField.Append(", 18)");
                    }
                    break;
                case ConvertOption.ToDouble:
                    {
                        spliceField.Append(" ROUND(");
                        fieldInkove.Invoke();
                        spliceField.Append(", 16)");
                    }
                    break;
                case ConvertOption.ToInt32:
                    {
                        spliceField.Append("cast(");
                        fieldInkove.Invoke();
                        spliceField.Append(" as INTEGER)");
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
                        spliceField.Append("datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(")");
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
                        //result = $"datetime({field}, '{value} year')";
                        spliceField.Append(" datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(", '");
                        valueInkove.Invoke();
                        spliceField.Append(" year')");
                    }
                    break;
                case DateOption.AddMonths:
                    {
                        //result = $"datetime({field}, '{value} month')";
                        spliceField.Append(" datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(", '");
                        valueInkove.Invoke();
                        spliceField.Append(" month')");
                    }
                    break;
                case DateOption.AddDays:
                    {
                        //result = $"datetime({field}, '{value} day')";
                        spliceField.Append(" datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(", '");
                        valueInkove.Invoke();
                        spliceField.Append(" day')");
                    }
                    break;
                case DateOption.AddHours:
                    {
                        //result = $"datetime({field}, '{value} hour')";
                        spliceField.Append(" datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(", '");
                        valueInkove.Invoke();
                        spliceField.Append(" hour')");
                    }
                    break;
                case DateOption.AddMinutes:
                    {
                        //result = $"datetime({field}, '{value} minute')";
                        spliceField.Append(" datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(", '");
                        valueInkove.Invoke();
                        spliceField.Append(" minute')");
                    }
                    break;
                case DateOption.AddSeconds:
                    {
                        //result = $"datetime({field}, '{value} second')";
                        spliceField.Append(" datetime(");
                        fieldInkove.Invoke();
                        spliceField.Append(", '");
                        valueInkove.Invoke();
                        spliceField.Append(" second')");
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
