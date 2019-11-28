using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public abstract class IProviderOption
    {
        public IProviderOption(string openQuote, string closeQuote, char parameterPrefix)
        {
            OpenQuote = openQuote;
            CloseQuote = closeQuote;
            ParameterPrefix = parameterPrefix;
			NavigationList = new List<NavigationMemberAssign>();
			MappingList = new Dictionary<string, string>();
			IsAsName = true;
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
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public abstract string GetDate();
		/// <summary>
		/// 结合时间处理
		/// </summary>
		/// <param name="dateOption"></param>
		/// <param name="spliceField"></param>
		/// <param name="fieldInkove"></param>
		/// <param name="valueInkove"></param>
		public abstract void CombineDate(DateOption dateOption, StringBuilder spliceField,  Action fieldInkove, Action valueInkove);
		/// <summary>
		/// 模糊转义
		/// </summary>
		/// <param name="value"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public virtual object FuzzyEscaping(object value,ref string param)
		{
			value = $"%{value}%";
			return value;
		}
		/// <summary>
		/// 转小写
		/// </summary>
		/// <param name="spliceField"></param>
		/// <param name="fieldInkove"></param>
		/// <returns></returns>
		public virtual void ToLower(StringBuilder spliceField, Action fieldInkove)
		{
			spliceField.Append(" lower(");
			fieldInkove.Invoke();
			spliceField.Append(") ");
		}
		/// <summary>
		/// 转大写
		/// </summary>
		/// <param name="spliceField"></param>
		/// <param name="fieldInkove"></param>
		/// <returns></returns>
		public virtual void ToUpper(StringBuilder spliceField, Action fieldInkove)
		{
			spliceField.Append(" upper(");
			fieldInkove.Invoke();
			spliceField.Append(") ");
		}
		#region 临时属性
		/// <summary>
		/// 子查询导航的集合
		/// </summary>
		public List<NavigationMemberAssign> NavigationList { get; set; }
		/// <summary>
		/// 记录映射对象
		/// </summary>
		public Dictionary<string,string> MappingList { get; set; }
		/// <summary>
		/// 是否重命名   table  as newName
		/// </summary>
		public bool IsAsName { get; set; }
		#endregion
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
	/// <summary>
	/// 子查询导航
	/// </summary>
	public class NavigationMemberAssign
	{
		/// <summary>
		/// 导航查询的表达式
		/// </summary>
		public MemberAssignment MemberAssign { get; set; }
		/// <summary>
		/// 导航查询的对象
		/// </summary>
		public string MemberAssignName { get; set; }
		/// <summary>
		/// 导航查询对象的类型
		/// </summary>
		public Type MemberAssignType { get; set; }
	}
}
