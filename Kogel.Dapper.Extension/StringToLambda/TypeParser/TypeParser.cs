using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lenic.DI.Core
{
    /// <summary>
    /// 类型分析器
    /// </summary>
    [DebuggerStepThrough]
    public class TypeParser
    {
        #region Properties
        /// <summary>
        /// 原始字符串分析结果
        /// </summary>
        private SymbolParseResult spResult = null;
        /// <summary>
        /// 获得待分析的类型可能用到的命名空间列表
        /// </summary>
        private IEnumerable<string> namespaces = Enumerable.Empty<string>();
        /// <summary>
        /// 获得额外的程序集信息列表
        /// </summary>
        private IEnumerable<Assembly> assemblyExtensions = Enumerable.Empty<Assembly>();

        private TypeParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeParser"/> class.
        /// </summary>
        /// <param name="spResult">The symbol parse result.</param>
        internal TypeParser(ref SymbolParseResult spResult)
        {
            this.spResult = spResult;
        }

        /// <summary>
        /// 获得一个 <see cref="TypeParser"/> 类的实例对象
        /// </summary>
        public static TypeParser NewInstance
        {
            get { return new TypeParser(); }
        }

        private static Assembly[] _assemblies = null;
        /// <summary>
        /// 获取程序入口点所在目录程序集列表
        /// </summary>
        private static Assembly[] Assemblies
        {
            get
            {
                if (_assemblies == null)
                    _assemblies = AppDomain.CurrentDomain.GetAssemblies();

                return _assemblies;
            }
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// 添加可能遇到的命名空间字符串列表
        /// </summary>
        /// <param name="namespaces">新的命名空间字符串列表</param>
        /// <returns>修改后的自身</returns>
        public TypeParser SetNamespaces(IEnumerable<string> namespaces)
        {
            this.namespaces = namespaces ?? Enumerable.Empty<string>();

            return this;
        }

        /// <summary>
        /// 添加可能遇到的命名空间字符串列表
        /// </summary>
        /// <param name="namespaces">新的命名空间字符串列表</param>
        /// <returns>修改后的自身</returns>
        public TypeParser SetNamespaces(params string[] namespaces)
        {
            this.namespaces = namespaces ?? Enumerable.Empty<string>();

            return this;
        }

        /// <summary>
        /// 添加可能遇到的程序集信息列表
        /// </summary>
        /// <param name="assemblies">附加的程序集信息列表</param>
        /// <returns>修改后的自身</returns>
        public TypeParser SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            assemblyExtensions = assemblies ?? Enumerable.Empty<Assembly>();

            return this;
        }

        /// <summary>
        /// 添加可能遇到的程序集信息列表
        /// </summary>
        /// <param name="assemblies">附加的程序集信息列表</param>
        /// <returns>修改后的自身</returns>
        public TypeParser SetAssemblies(params Assembly[] assemblies)
        {
            assemblyExtensions = assemblies ?? Enumerable.Empty<Assembly>();

            return this;
        }

        /// <summary>
        /// 解析字符串为类型
        /// </summary>
        /// <returns>读取的类型</returns>
        public Type Resolve(string typeString)
        {
            spResult = SymbolParser.Build(typeString);

            return ReadType();
        }
        #endregion

        #region Private Methods
        internal Type ReadType(string typeName = null, bool ignoreException = false)
        {
            Type type = null;
            StringBuilder sbValue =
                new StringBuilder(string.IsNullOrEmpty(typeName) ? spResult.Next() : typeName);
            do
            {
                // read generic parameters
                if (spResult.PeekNext() == "<")
                {
                    spResult.Skip();
                    List<Type> listGenericType = new List<Type>();
                    while (true)
                    {
                        listGenericType.Add(ReadType());
                        if (spResult.PeekNext() == ",")
                            spResult.Skip();
                        else
                            break;
                    }
                    NextIsEqual(">");

                    sbValue.AppendFormat("`{0}[{1}]", listGenericType.Count,
                        string.Join(",", listGenericType
                            .Select(p => "[" + p.AssemblyQualifiedName + "]").ToArray()));
                }

                type = GetType(sbValue.ToString());
                if (type == null)
                {
                    bool result = NextIsEqual(".", false);
                    if (!result)
                    {
                        if (ignoreException)
                            break;
                        throw new ParseUnfindTypeException(sbValue.ToString(), spResult.Index);
                    }
                    sbValue.Append(".");
                    sbValue.Append(spResult.Next());
                }
            } while (type == null);

            return type;
        }

        internal Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            // Nullable
            bool isNullable = false;
            if (typeName.EndsWith("?"))
            {
                isNullable = true;
                typeName = typeName.Substring(0, typeName.Length - 1);
            }

            Type type;
            switch (typeName)
            {
                case "bool":
                    type = typeof(bool);
                    break;
                case "byte":
                    type = typeof(byte);
                    break;
                case "sbyte":
                    type = typeof(sbyte);
                    break;
                case "char":
                    type = typeof(char);
                    break;
                case "decimal":
                    type = typeof(decimal);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "float":
                    type = typeof(float);
                    break;
                case "int":
                    type = typeof(int);
                    break;
                case "uint":
                    type = typeof(uint);
                    break;
                case "long":
                    type = typeof(long);
                    break;
                case "ulong":
                    type = typeof(ulong);
                    break;
                case "object":
                    type = typeof(object);
                    break;
                case "short":
                    type = typeof(short);
                    break;
                case "ushort":
                    type = typeof(ushort);
                    break;
                case "string":
                    type = typeof(string);
                    break;
                default:
                    {
                        // Suppose typeName is full name of class
                        type = GetTypeCore(typeName);

                        // Did not find the namespace to use all of the match again and again
                        if (type == null)
                        {
                            foreach (string theNamespace in namespaces)
                            {
                                type = GetTypeCore(string.Concat(theNamespace, ".", typeName));

                                // To find a qualified first class
                                if (type != null)
                                    break;
                            }
                        }
                    }
                    break;
            }

            if (isNullable && type != null)
                type = typeof(Nullable<>).MakeGenericType(type);

            return type;
        }

        private Type GetTypeCore(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;

            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in listAssembly)
            {
                type = assembly.GetType(typeName, false, false);
                if (type != null)
                    return type;
            }

            if (assemblyExtensions != null && assemblyExtensions.Any())
            {
                foreach (Assembly assembly in assemblyExtensions)
                {
                    type = assembly.GetType(typeName, false, false);
                    if (type != null)
                        return type;
                }
            }

            if (Assemblies != null && Assemblies.Any())
            {
                foreach (Assembly assembly in Assemblies)
                {
                    type = assembly.GetType(typeName, false, false);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }

        private bool NextIsEqual(string symbol, bool throwExceptionIfError = true)
        {
            if (spResult.Next() != symbol)
            {
                if (throwExceptionIfError)
                    throw new ApplicationException(string.Format("{0} isn't the next token", symbol));
                else
                    return false;
            }
            return true;
        }
        #endregion
    }
}
