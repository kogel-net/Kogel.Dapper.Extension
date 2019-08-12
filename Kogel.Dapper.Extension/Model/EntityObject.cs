using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Helper;

namespace Kogel.Dapper.Extension.Model
{
    public class EntityObject
    {
        public EntityObject(Type type)
        {
            //反射表名称
            this.Name = type.Name;
            //指定as名称
            this.AsName = type.Name;
            //获取是否有Display特性
            var typeAttribute = type.GetCustomAttributess(true).FirstOrDefault(x => x.GetType().Equals(typeof(Display)));
            if (typeAttribute != null)
            {
                var display = typeAttribute as Display;
                //是否有重命名
                var rename = display.Rename;
                if (!string.IsNullOrEmpty(rename))
                {
                    this.Name = rename;
                }
                //是否有名称空间
                string schema = display.Schema;
                if (!string.IsNullOrEmpty(schema))
                {
                    this.Schema = schema;
                }
                //是否有指定as名称
                string asName = display.AsName;
                if (!string.IsNullOrEmpty(asName))
                {
                    this.AsName = asName;
                }
            }
            this.Type = type;
            this.AssemblyString = type.FullName;
            //反射实体类属性
            this.Properties = type.GetProperties();
            List<PropertyInfo> PropertyInfoList = new List<PropertyInfo>();
            this.FieldPairs = new Dictionary<string, string>();
            //反射实体类字段
            foreach (var item in this.Properties)
            {
                var fieldAttribute = item.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Equals(typeof(Display)));
                if (fieldAttribute != null)
                {
                    var display = fieldAttribute as Display;
                    //获取是否是表关系隐射字段
                    if (display.IsField)
                    {
                        this.FieldPairs.Add(item.Name, item.Name);
                        //获取是否有重命名
                        if (!string.IsNullOrEmpty(display.Rename))
                        {
                            this.FieldPairs[item.Name] = display.Rename;
                        }
                        PropertyInfoList.Add(item);
                    }
                }
                else
                {
                    this.FieldPairs.Add(item.Name, item.Name);
                    PropertyInfoList.Add(item);
                }
            }
            this.Properties = PropertyInfoList.ToArray();
        }
        /// <summary>
        /// 类名(表名称)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 名称空间
        /// </summary>
        public string Schema { get; set; }
        /// <summary>
        /// 指定as名称
        /// </summary>
        public string AsName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string AssemblyString { get; set; }
        /// <summary>
        /// 类反射的属性实例
        /// </summary>
        public PropertyInfo[] Properties { get; set; }
        /// <summary>
        /// 字段目录(属性名称和实体名称)
        /// </summary>
        public Dictionary<string,string> FieldPairs { get; set; }
    }
}
