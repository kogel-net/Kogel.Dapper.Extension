using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Model;
using System;
using System.Linq.Expressions;
using System.Text;

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
                        //spliceField.Append(" not in ('0',false)");
                    }
                    break;
                case ConvertOption.ToDecimal:
                    {
                        spliceField.Append(" cast(");
                        fieldInkove.Invoke();
                        spliceField.Append(" as decimal(36,19))");
                    }
                    break;
                case ConvertOption.ToDouble:
                    {
                        spliceField.Append(" cast(");
                        fieldInkove.Invoke();
                        spliceField.Append(" as decimal(36,16))");
                    }
                    break;
                case ConvertOption.ToInt32:
                    {
                        spliceField.Append("cast(");
                        fieldInkove.Invoke();
                        spliceField.Append(" as int)");
                    }
                    break;
                case ConvertOption.ToString:
                    {
                        spliceField.Append("cast(");
                        fieldInkove.Invoke();
                        spliceField.Append(" as nvarchar(2000))");
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
        /// 时间转义
        /// </summary>
        /// <param name="dateOption"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override void CombineDate(DateOption dateOption, StringBuilder spliceField, Action fieldInkove, Action valueInkove)
        {
            //string result = string.Empty;
            switch (dateOption)
            {
                case DateOption.AddYears:
                    {
                        //result = $"dateadd(yy,{value},{field})";
                        spliceField.Append(" dateadd(yy,");
                        valueInkove.Invoke();
                        spliceField.Append(",");
                        fieldInkove.Invoke();
                        spliceField.Append(" )");
                    }
                    break;
                case DateOption.AddMonths:
                    {
                        //result = $"dateadd(mm,{value},{field})";
                        spliceField.Append(" dateadd(mm,");
                        valueInkove.Invoke();
                        spliceField.Append(",");
                        fieldInkove.Invoke();
                        spliceField.Append(" )");
                    }
                    break;
                case DateOption.AddDays:
                    {
                        //result = $"dateadd(dd,{value},{field})";
                        spliceField.Append(" dateadd(dd,");
                        valueInkove.Invoke();
                        spliceField.Append(",");
                        fieldInkove.Invoke();
                        spliceField.Append(" )");
                    }
                    break;
                case DateOption.AddHours:
                    {
                        //result = $"dateadd(hh,{value},{field})";
                        spliceField.Append(" dateadd(hh,");
                        valueInkove.Invoke();
                        spliceField.Append(",");
                        fieldInkove.Invoke();
                        spliceField.Append(" )");
                    }
                    break;
                case DateOption.AddMinutes:
                    {
                        //result = $"dateadd(minute,{value},{field})";
                        spliceField.Append(" dateadd(minute,");
                        valueInkove.Invoke();
                        spliceField.Append(",");
                        fieldInkove.Invoke();
                        spliceField.Append(" )");
                    }
                    break;
                case DateOption.AddSeconds:
                    {
                        //result = $"dateadd(ss,{value},{field})";
                        spliceField.Append(" dateadd(ss,");
                        valueInkove.Invoke();
                        spliceField.Append(",");
                        fieldInkove.Invoke();
                        spliceField.Append(" )");
                    }
                    break;
            }
            //return result;
        }
        /// <summary>
        /// 模糊转义
        /// </summary>
        /// <param name="value"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override object FuzzyEscaping(object value, ref string param, EFuzzyLocation eFuzzyLocation = EFuzzyLocation.Both)
        {
            if (value != null && value.ToString() != string.Empty)
            {
                switch (eFuzzyLocation)
                {
                    case EFuzzyLocation.Right:
                        value = $@"\{value}%";
                        break;
                    case EFuzzyLocation.Left:
                        value = $@"%\{value}";
                        break;
                    case EFuzzyLocation.Both:
                    default:
                        value = $@"%\{value}%";
                        break;
                }
                param += @" ESCAPE'\'";
            }
            else
            {
                base.FuzzyEscaping(value, ref param, eFuzzyLocation);
            }
            return value;
        }

        /// <summary>
        /// is null 函数
        /// </summary>
        /// <returns></returns>
        public override string IfNull()
        {
            return "IsNull";
        }
    }
}
