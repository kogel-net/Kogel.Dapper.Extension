﻿using Kogel.Dapper.Extension.Entites;
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

        /// <summary>
        /// 
        /// </summary>
        public string OpenQuote { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
            return $"{OpenQuote}{field}{CloseQuote}";
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public abstract string GetDate();

        /// <summary>
        /// 结合转换处理
        /// </summary>
        /// <param name="convertOption"></param>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        public abstract void CombineConvert(ConvertOption convertOption, StringBuilder _sqlBuilder, Action fieldInkove);

        /// <summary>
        /// 结合时间处理
        /// </summary>
        /// <param name="dateOption"></param>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        /// <param name="valueInkove"></param>
        public abstract void CombineDate(DateOption dateOption, StringBuilder _sqlBuilder, Action fieldInkove, Action valueInkove);

        /// <summary>
        /// 模糊转义
        /// </summary>
        /// <param name="value"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual object FuzzyEscaping(object value, ref string param, EFuzzyLocation eFuzzyLocation = EFuzzyLocation.Both)
        {
            switch (eFuzzyLocation)
            {
                case EFuzzyLocation.Right:
                    value = $"{value}%";
                    return value;
                case EFuzzyLocation.Left:
                    value = $"%{value}";
                    return value;
                case EFuzzyLocation.Both:
                default:
                    value = $"%{value}%";
                    return value;
            }
        }

        /// <summary>
        /// 转小写
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        /// <returns></returns>
        public virtual void ToLower(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" lower(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }

        /// <summary>
        /// 转大写
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        /// <returns></returns>
        public virtual void ToUpper(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" upper(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }

        /// <summary>
        /// if null函数
        /// </summary>
        /// <param name="beginInkove"></param>
        /// <param name="endInkove"></param>
        public abstract string IfNull();

        #region 临时属性
        /// <summary>
        /// 子查询导航的集合
        /// </summary>
        public List<NavigationMemberAssign> NavigationList { get; set; }

        /// <summary>
        /// 记录映射对象
        /// </summary>
        public Dictionary<string, string> MappingList { get; set; }

        /// <summary>
        /// 是否重命名   table  as newName
        /// </summary>
        public bool IsAsName { get; set; }
        #endregion

        #region 聚合函数
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        /// <returns></returns>
        public virtual void Count(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" Count(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }

        /// <summary>
        /// 计总
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        public virtual void Sum(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" Sum(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        public virtual void Max(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" Max(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        public virtual void Min(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" Min(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <param name="_sqlBuilder"></param>
        /// <param name="fieldInkove"></param>
        public virtual void Avg(StringBuilder _sqlBuilder, Action fieldInkove)
        {
            _sqlBuilder.Append(" Avg(");
            fieldInkove.Invoke();
            _sqlBuilder.Append(") ");
        }
        #endregion
    }

    /// <summary>
    /// 转换处理
    /// </summary>
    public enum ConvertOption
    {
        ToInt32,
        ToString,
        ToDecimal,
        ToDouble,
        ToBoolean,
        ToDateTime
    }

    /// <summary>
    /// 时间格式处理
    /// </summary>
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
