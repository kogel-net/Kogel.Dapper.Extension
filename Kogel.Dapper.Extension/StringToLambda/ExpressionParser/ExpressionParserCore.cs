using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Lenic.DI.Core
{
    public class ExpressionParserCore
    {
        #region Fields
        /// <summary>
        /// 数字类型优先级
        /// </summary>
        private static readonly Dictionary<Type, int> numericTypeLevel = null;

        /// <summary>
        /// 字符分析结果
        /// </summary>
        private SymbolParseResult spResult = null;
        /// <summary>
        /// 类型分析器
        /// </summary>
        private TypeParser typeParser = null;
        /// <summary>
        /// 委托参数类型
        /// </summary>
        private Type[] parameterTypes = null;
        /// <summary>
        /// 委托返回类型
        /// </summary>
        private Type returnType = null;
        /// <summary>
        /// 表达式树结果参数列表
        /// </summary>
        private List<ParameterExpression> expParams = new List<ParameterExpression>();
        #endregion

        private static readonly string iEnumerableName = typeof(IEnumerable<>).FullName;

        #region Construction
        /// <summary>
        /// Initializes the <see cref="ExpressionParserCore"/> class.
        /// </summary>
        static ExpressionParserCore()
        {
            // 数字类型优先级
            numericTypeLevel = new Dictionary<Type, int>();
            numericTypeLevel.Add(typeof(byte), 1);
            numericTypeLevel.Add(typeof(byte?), 1);
            numericTypeLevel.Add(typeof(short), 2);
            numericTypeLevel.Add(typeof(short?), 2);
            numericTypeLevel.Add(typeof(ushort), 3);
            numericTypeLevel.Add(typeof(ushort?), 3);
            numericTypeLevel.Add(typeof(int), 4);
            numericTypeLevel.Add(typeof(int?), 4);
            numericTypeLevel.Add(typeof(uint), 5);
            numericTypeLevel.Add(typeof(uint?), 5);
            numericTypeLevel.Add(typeof(long), 6);
            numericTypeLevel.Add(typeof(long?), 6);
            numericTypeLevel.Add(typeof(ulong), 7);
            numericTypeLevel.Add(typeof(ulong?), 7);
            numericTypeLevel.Add(typeof(float), 8);
            numericTypeLevel.Add(typeof(float?), 8);
            numericTypeLevel.Add(typeof(double), 9);
            numericTypeLevel.Add(typeof(double?), 9);
            numericTypeLevel.Add(typeof(decimal), 10);
            numericTypeLevel.Add(typeof(decimal?), 10);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParserCore"/> class.
        /// </summary>
        /// <param name="spResult">The sp result.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="namespaces">The namespaces.</param>
        public ExpressionParserCore(SymbolParseResult spResult, Type delegateType, IEnumerable<string> namespaces = null, IEnumerable<Assembly> assemblies = null)
        {
            this.spResult = spResult;

            this.typeParser = new TypeParser(ref this.spResult);
            typeParser.SetNamespaces(namespaces);
            typeParser.SetAssemblies(assemblies);

            var method = delegateType.GetMethod("Invoke");
            parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            returnType = method.ReturnType;
        }
        #endregion

        #region Lambda Perser
        public LambdaExpression ToLambdaExpression()
        {
            ProcessLambdaPrefix();
            var exp = ReadExpression();

            return LambdaExpression.Lambda(exp.Expression, expParams);
        }

        public Expression<T> ToLambdaExpression<T>()
        {
            ProcessLambdaPrefix();
            var exp = ReadExpression();

            return LambdaExpression.Lambda<T>(exp.Expression, expParams);
        }
        #endregion

        #region Read Expressions
        /// <summary>
        /// 处理表达式前缀.
        /// </summary>
        private void ProcessLambdaPrefix()
        {
            // 检查是否有 Lambda 前置符(如: m => )
            if (spResult.Any(p => p.ID == TokenId.LambdaPrefix))
            {
                Token token = spResult.Next();
                if (token.ID == TokenId.OpenParen)
                {
                    var bracketContent = spResult.SkipUntil(p => p.ID == TokenId.CloseParen);
                    bracketContent.RemoveAt(bracketContent.Count - 1);

                    if (bracketContent.Any(p => p.ID == TokenId.OpenParen))
                        throw new ParserSyntaxErrorException();

                    // 如果读取到 => 符号表示有
                    if (!spResult.NextIs(TokenId.LambdaPrefix))
                    {
                        spResult.ReturnToIndex(-1);
                        return;
                    }

                    // 解析参数
                    ResolveParameters(bracketContent).Foreach((p, i) =>
                    {
                        if (p.ExistType)
                            expParams.Add(Expression.Parameter(typeParser.GetType(p.Type), p.Variable));
                        else
                            expParams.Add(Expression.Parameter(parameterTypes[i], p.Variable));
                    });
                }
                else if (token.ID == TokenId.Identifier &&
                         (char.IsLetter(token.Text[0]) || token.Text[0] == '_') &&
                         !(token.Text == "true" ||
                           token.Text == "false" ||
                           token.Text == "null" ||
                           token.Text == "sizeof" ||
                           token.Text == "new" ||
                           token.Text == "typeof"))
                {
                    if (!spResult.NextIs(TokenId.LambdaPrefix))
                    {
                        spResult.ReturnToIndex(-1);
                        return;
                    }

                    expParams.Add(Expression.Parameter(parameterTypes[0], token.Text));
                }

                // 参数表达式个数和传入委托参数个数不匹配判断
                if (expParams.Count != parameterTypes.Length)
                    throw new ApplicationException("The count of parameters is not equal.");
            }
        }

        /// <summary>
        /// Reads the expression.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="wrapStart">The wrap start.</param>
        /// <returns>The read result.</returns>
        private ReadResult ReadExpression(int level = 0)
        {
            var exp = ReadFirstExpression();

            int nextLevel = 0;
            var next = spResult.PeekNext();
            while (!exp.IsClosedWrap &&
                   (nextLevel = PriorityManager.GetOperatorLevel(next)) > level)
            {
                exp = ReadNextExpression(nextLevel, exp);
                next = spResult.PeekNext();
            }

            return exp;
        }

        /// <summary>
        /// Reads the first expression.
        /// </summary>
        /// <returns>The read result.</returns>
        private ReadResult ReadFirstExpression()
        {
            ReadResult result = ReadResult.Empty;

            var token = spResult.Next();
            switch (token.ID)
            {
                case TokenId.Identifier:
                    result = ParseIdentifier(token);
                    break;
                case TokenId.StringLiteral:
                    if (token.Text.StartsWith("\""))
                        result.Expression = Expression.Constant(token.Text.Substring(1, token.Text.Length - 2), typeof(string));
                    else if (token.Text.StartsWith("'"))
                        result.Expression = Expression.Constant(token.Text[1], typeof(char));
                    break;
                case TokenId.IntegerLiteral:
                    result.Expression = Expression.Constant(int.Parse(token.Text), typeof(int));
                    break;
                case TokenId.LongIntegerLiteral:
                    result.Expression = Expression.Constant(long.Parse(DeleteDigitTypeReference(token.Text)), typeof(long));
                    break;
                case TokenId.RealLiteral:
                    result.Expression = Expression.Constant(double.Parse(DeleteDigitTypeReference(token.Text)), typeof(double));
                    break;
                case TokenId.SingleRealLiteral:
                    result.Expression = Expression.Constant(float.Parse(DeleteDigitTypeReference(token.Text)), typeof(float));
                    break;
                case TokenId.DecimalRealLiteral:
                    result.Expression = Expression.Constant(decimal.Parse(DeleteDigitTypeReference(token.Text)), typeof(decimal));
                    break;
                case TokenId.Exclamation:
                    result.Expression = Expression.Not(ReadExpression().Expression);
                    break;
                case TokenId.Plus:
                case TokenId.Comma:
                    result = ReadExpression();
                    break;
                case TokenId.Minus:
                    result.Expression = Expression.Negate(ReadExpression().Expression);
                    break;
                case TokenId.OpenParen:
                    result = ParseConvertType();
                    break;
                case TokenId.CloseParen:
                case TokenId.CloseBracket:
                case TokenId.CloseBrace:
                    result.IsClosedWrap = true;
                    break;
                default:
                    throw new ParserSyntaxErrorException();
            }

            return result;
        }

        /// <summary>
        /// Reads the next expression.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="wrapStart">The wrap start.</param>
        /// <returns>The read result.</returns>
        private ReadResult ReadNextExpression(int level, ReadResult previousResult)
        {
            var result = previousResult;
            Token token = spResult.Next();

            switch (token.ID)
            {
                case TokenId.End:
                case TokenId.CloseBrace:
                case TokenId.CloseBracket:
                case TokenId.CloseParen:
                    result.IsClosedWrap = true;
                    break;
                case TokenId.Dot:
                    token = spResult.Next().Throw(TokenId.Identifier);

                    // 获取尖括号 <> 中的类型数据
                    Type[] types = Type.EmptyTypes;
                    if (spResult.PeekNextIs(TokenId.LessThan))
                        types = GetTypes();

                    if (spResult.PeekNextIs(TokenId.LessThan) || spResult.PeekNextIs(TokenId.OpenParen))
                    {
                        var @params = GetCollectionInits().ToArray();
                        var method = FindBestMethod(result.Expression.Type, token.Text, @params, false, types);
                        result.Expression = Expression.Call(result.Expression, method, @params);
                    }
                    else
                    {
                        var member = result.Expression.Type.GetMember(token.Text)[0];
                        if (member.MemberType == MemberTypes.Property)
                            result.Expression = Expression.Property(result.Expression, (PropertyInfo)member);
                        else
                            result.Expression = Expression.Field(result.Expression, (FieldInfo)member);
                    }
                    break;
                case TokenId.Plus:
                    var right = ReadExpression(level);

                    if (result.Expression.Type == typeof(string) || right.Expression.Type == typeof(string))
                    {
                        result.Expression = Expression.Call(typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) }),
                            Expression.Convert(result.Expression, typeof(object)), Expression.Convert(right.Expression, typeof(object)));
                        result.IsClosedWrap = result.IsClosedWrap || right.IsClosedWrap;
                    }
                    else
                        NumericExpressionOperator(ref result, ref right, Expression.Add);
                    break;
                case TokenId.Minus:
                    right = ReadExpression(level);
                    NumericExpressionOperator(ref result, ref right, Expression.Subtract);
                    break;
                case TokenId.Asterisk:
                    right = ReadExpression(level);
                    NumericExpressionOperator(ref result, ref right, Expression.Multiply);
                    break;
                case TokenId.Slash:
                    right = ReadExpression(level);
                    NumericExpressionOperator(ref result, ref right, Expression.Divide);
                    break;
                case TokenId.Percent:
                    right = ReadExpression(level);
                    NumericExpressionOperator(ref result, ref right, Expression.Modulo);
                    break;
                case TokenId.OpenBracket:
                    if (result.Expression.Type.IsArray)
                        result.Expression = Expression.ArrayIndex(result.Expression, ReadExpression(level).Expression);
                    else
                    {
                        string indexerName = "Item";

                        var indexerNameAtt = result.Expression
                                                   .Type
                                                   .GetCustomAttributes(typeof(DefaultMemberAttribute), true)
                                                   .Cast<DefaultMemberAttribute>()
                                                   .SingleOrDefault();
                        if (indexerNameAtt != null)
                            indexerName = indexerNameAtt.MemberName;

                        var methodInfo = result.Expression
                                               .Type
                                               .GetProperty(indexerName)
                                               .GetGetMethod();
                        var @params = GetCollectionInits(true).ToArray();

                        result.Expression = Expression.Call(result.Expression, methodInfo, @params);
                    }
                    break;
                case TokenId.DoubleEqual:
                    right = ReadExpression(level);
                    //cs于2017-7-7修改》
                    //修改原由：当操作符右侧为数值时，即使类型为long也会根据它实际
                    //的长度判别出类型，造成与原类型不匹配的情况
                    //修改结果：当判断出操作符左侧为System.Int64,将右侧的System.Int32表达式改为System.Int64
                    if (result.Expression.Type!=right.Expression.Type&&
                        (result.Expression.Type.ToString()== "Int64"|| result.Expression.Type.ToString() == "System.Int64"))
                    {
                        right.Expression = Expression.Convert(right.Expression, result.Expression.Type);
                    }
                    result.Expression = Expression.Equal(result.Expression, right.Expression);
                    break;
                case TokenId.ExclamationEqual:
                    right = ReadExpression(level);
                    result.Expression = Expression.NotEqual(result.Expression, right.Expression);
                    break;
                case TokenId.GreaterThan:
                    right = ReadExpression(level);
                    result.Expression = Expression.GreaterThan(result.Expression, right.Expression);
                    break;
                case TokenId.GreaterThanEqual:
                    right = ReadExpression(level);
                    result.Expression = Expression.GreaterThanOrEqual(result.Expression, right.Expression);
                    break;
                case TokenId.LessThan:
                    right = ReadExpression(level);
                    result.Expression = Expression.LessThan(result.Expression, right.Expression);
                    break;
                case TokenId.LessThanEqual:
                    right = ReadExpression(level);
                    result.Expression = Expression.LessThanOrEqual(result.Expression, right.Expression);
                    break;
                case TokenId.Identifier:
                    result.Expression = ParseIdentifierNext(result, token, level);
                    break;
                case TokenId.DoubleAmphersand:
                    right = ReadExpression(level);
                    result.Expression = Expression.AndAlso(result.Expression, right.Expression);
                    break;
                case TokenId.DoubleBar:
                    right = ReadExpression(level);
                    result.Expression = Expression.OrElse(result.Expression, right.Expression);
                    break;
                case TokenId.Question:
                    right = ReadExpression(level);
                    spResult.NextIs(TokenId.Colon);
                    var third = ReadExpression(level);
                    result.Expression = Expression.Condition(result.Expression, right.Expression, third.Expression);
                    break;
                case TokenId.DoubleQuestion:
                    right = ReadExpression(level);
                    var test = Expression.Equal(result.Expression, Expression.Constant(null, result.Expression.Type));
                    result.Expression = Expression.Condition(test, right.Expression, result.Expression);
                    break;
                default:
                    throw new ParserSyntaxErrorException();
            }

            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 解析参数并返回配对（类型/变量）的字符单元列表.
        /// </summary>
        /// <param name="obj">待解析的字符单元列表.</param>
        /// <returns>解析完成的字符单元列表.</returns>
        private TypeVariable[] ResolveParameters(IEnumerable<Token> obj)
        {
            Token selector = new Token { ID = TokenId.Comma, Text = "," };
            var result = new List<TypeVariable>();
            if (ReferenceEquals(obj, null) || !obj.Any())
                return result.ToArray();

            if (obj.Last() != selector)
            {
                var list = obj.ToList();
                list.Add(selector);
                obj = list;
            }
            var data = obj.ToArray();

            int firstIndex = 0, secondIndex = 0;
            while ((secondIndex = Array.IndexOf(data, selector, firstIndex)) != -1)
            {
                if (secondIndex == firstIndex + 1)
                    result.Add(new TypeVariable(null, data[firstIndex]));
                else if (secondIndex == firstIndex + 2)
                    result.Add(new TypeVariable(data[firstIndex], data[firstIndex + 1]));
                else
                    throw new ParserSyntaxErrorException();

                firstIndex = secondIndex + 1;
            }

            return result.ToArray();
        }

        private ReadResult ParseIdentifier(Token token)
        {
            ReadResult result = ReadResult.Empty;

            switch (token.Text)
            {
                case "true":
                    result.Expression = Expression.Constant(true);
                    break;
                case "false":
                    result.Expression = Expression.Constant(false);
                    break;
                case "null":
                    result.Expression = Expression.Constant(null);
                    break;
                case "sizeof":
                    if (spResult.NextIs(TokenId.OpenParen))
                    {
                        result.Expression = Expression.Constant(Marshal.SizeOf(typeParser.ReadType()));
                        spResult.NextIs(TokenId.CloseParen);
                    }
                    else
                        throw new ParserSyntaxErrorException();
                    break;
                case "typeof":
                    if (spResult.NextIs(TokenId.OpenParen))
                    {
                        result.Expression = Expression.Constant(typeParser.ReadType(), typeof(Type));
                        spResult.NextIs(TokenId.CloseParen);

                    }
                    else
                        throw new ParserSyntaxErrorException();
                    break;
                case "new":
                    {
                        var type = typeParser.ReadType();

                        // 判断初始化的类型
                        token = spResult.Next();

                        // 构造函数成员初始化/集合项初始化
                        if (token.ID == TokenId.OpenParen || token.ID == TokenId.OpenBrace)
                        {
                            // 构建构造函数 new 的部分
                            if (token.ID == TokenId.OpenParen)
                            {
                                // 获取参数
                                var listParam = GetCollectionInits(true).ToArray();
                                var paramTypes = listParam.Select(m => m.Type).ToArray();

                                // 获取构造函数
                                var dd = type.GetConstructors()
                                    .Select(p => new
                                    {
                                        Parameters = p.GetParameters().Select(r => r.ParameterType).ToArray(),
                                        Constructor = p,
                                    })
                                    .ToArray();

                                var constructor = type.GetConstructors()
                                    .Select(p => new
                                    {
                                        Parameters = p.GetParameters().Select(r => r.ParameterType).ToArray(),
                                        Constructor = p,
                                    })
                                    .Where(p => p.Parameters.Length == paramTypes.Length)
                                    .First(p => p.Parameters.Select(r => r.GetNoneNullableType())
                                                            .SequenceEqual(paramTypes.Select(r => r.GetNoneNullableType()),
                                                                           new EqualComparer<Type>((x, y) =>
                                                                           {
                                                                               if (x.IsAssignableFrom(y) || y.IsAssignableFrom(x))
                                                                                   return true;
                                                                               else
                                                                                   return x == y;
                                                                           })))
                                    .Constructor;

                                // 获取匹配的构造函数参数
                                var constructorParamTypes =
                                    constructor.GetParameters()
                                               .Select(p => p.ParameterType)
                                               .Zip(listParam, (x, y) => new { Left = x, Right = y })
                                               .Select((p, i) =>
                                               {
                                                   if (p.Left.IsNullable() && p.Left != p.Right.Type)
                                                       return Expression.Convert(p.Right, p.Left.GetNullableType());
                                                   else
                                                       return p.Right;
                                               })
                                               .ToArray();

                                // 构造函数调用
                                result.Expression = Expression.New(constructor, constructorParamTypes);
                            }
                            else
                                result.Expression = Expression.New(type.GetConstructor(Type.EmptyTypes));

                            // 构建构造函数属性成员初始化或者集合初始化
                            if (spResult.PeekNextIs(TokenId.OpenBrace) || token.ID == TokenId.OpenBrace)
                            {
                                if (token.ID == TokenId.OpenParen)
                                    spResult.Next();

                                // 测试是否属性成员初始化                            
                                bool isMemberInit = spResult.PeekNextIs(TokenId.Equal, 2);

                                if (isMemberInit)
                                    result.Expression =
                                        Expression.MemberInit((NewExpression)result.Expression, GetObjectMembers(type).ToArray());
                                else
                                    result.Expression =
                                        Expression.ListInit((NewExpression)result.Expression, GetCollectionInits().ToArray());
                            }
                        }
                        else if (token.ID == TokenId.OpenBracket)
                        {
                            Expression[] @params = null;
                            if (spResult.PeekNextIs(TokenId.CloseBracket))
                                spResult.Next();
                            else
                                @params = GetCollectionInits().ToArray();

                            if (spResult.PeekNextIs(TokenId.OpenBrace))
                                result.Expression = Expression.NewArrayInit(type, GetCollectionInits().ToArray());
                            else
                                result.Expression = Expression.NewArrayBounds(type, @params);
                        }
                        else
                            throw new ParserSyntaxErrorException();
                        break;
                    }
                default:
                    // 参数
                    if (expParams.Any(p => p.Name == token.Text))
                        result.Expression = expParams.First(p => p.Name == token.Text);
                    // 类型
                    else
                    {
                        var type = typeParser.ReadType(token.Text);
                        spResult.NextIs(TokenId.Dot, true);
                        var name = spResult.Next();

                        // 获取尖括号 <> 中的类型数据
                        Type[] types = Type.EmptyTypes;
                        if (spResult.PeekNextIs(TokenId.LessThan))
                            types = GetTypes();

                        if (spResult.PeekNextIs(TokenId.OpenParen))
                        {
                            var @params = GetCollectionInits().ToArray();
                            var method = FindBestMethod(type, name, @params, true, types);
                            result.Expression = Expression.Call(method, @params);
                        }
                        else
                        {
                            var member = type.GetMember(name)[0];
                            if (member.MemberType == MemberTypes.Property)
                                result.Expression = Expression.Property(null, (PropertyInfo)member);
                            else
                                result.Expression = Expression.Field(null, (FieldInfo)member);
                        }
                    }
                    break;
            }

            return result;
        }

        private Expression ParseIdentifierNext(ReadResult result, Token token, int level)
        {
            Expression exp = null;

            switch (token.Text)
            {
                case "is":
                    exp = Expression.TypeIs(result.Expression, typeParser.ReadType());
                    break;
                case "as":
                    exp = Expression.TypeAs(result.Expression, typeParser.ReadType());
                    break;
                default:
                    throw new ParserSyntaxErrorException();
            }

            return exp;
        }

        private ReadResult ParseConvertType()
        {
            var originPos = spResult.Index;
            var type = typeParser.ReadType(ignoreException: true);
            if (type != null)
            {
                spResult.NextIs(TokenId.CloseParen, true);
                var inner = ReadExpression();
                return new ReadResult
                {
                    Expression = Expression.Convert(inner.Expression, type),
                    IsClosedWrap = inner.IsClosedWrap,
                };
            }
            else
            {
                spResult.ReturnToIndex(originPos);
                var result = ReadExpression();
                if (!spResult.PeekNextIs(TokenId.End))
                    result.IsClosedWrap = false;
                return result;
            }
        }

        private string DeleteDigitTypeReference(string number)
        {
            if (char.IsLetter(number.Last()))
                return number.Substring(0, number.Length - 1);
            return number;
        }

        private IEnumerable<MemberBinding> GetObjectMembers(Type type)
        {
            Token token = Token.Empty;
            while (token.ID != TokenId.CloseBrace && (token = spResult.Next()).ID != TokenId.CloseBrace)
            {
                var member = type.GetProperty(token.Text);

                // 读取等号
                spResult.NextIs(TokenId.Equal, true);

                var result = ReadExpression();
                if (result.Expression != null)
                    yield return BindProperty(member, result.Expression);

                // 读取逗号
                token = spResult.Next();
            }
        }

        private MemberAssignment BindProperty(PropertyInfo @prop, Expression exp)
        {
            var targetType = @prop.PropertyType;
            if (PriorityManager.IsValueType(targetType))
            {
                var targetLevel = PriorityManager.GetNumericLevel(targetType);
                var sourceLevel = PriorityManager.GetNumericLevel(exp.Type);
                if (targetLevel > sourceLevel ||
                    (targetLevel == sourceLevel && targetType.IsNullable()))
                    exp = Expression.Convert(exp, targetType);
            }
            return Expression.Bind(@prop, exp);
        }

        private IEnumerable<Expression> GetCollectionInits(bool isReadPrefix = false)
        {
            if (!isReadPrefix)
            {
                var token = spResult.PeekNext();
                if (token.ID == TokenId.OpenBrace ||
                    token.ID == TokenId.OpenBracket ||
                    token.ID == TokenId.OpenParen)
                    spResult.Next();
            }

            ReadResult result = ReadResult.Empty;
            do
            {
                int pos = spResult.Index;
                result = ReadExpression();
                if (result.Expression != null)
                    yield return result.Expression;
            } while (!result.IsClosedWrap);
        }

        private MethodInfo FindBestMethod(Type type, string name, IEnumerable<Expression> @params, bool isStatic, Type[] genericParameterTypes)
        {
            BindingFlags flags;
            if (isStatic)
                flags = BindingFlags.Public | BindingFlags.Static;
            else
                flags = BindingFlags.Public | BindingFlags.Instance;

            var methods = type.GetMethods(flags).Where(p => p.Name == name).ToArray();
            if (methods.Length == 0)
                return null;

            if (genericParameterTypes != null && genericParameterTypes.Any())
            {
                methods = methods.Where(p => p.IsGenericMethod && p.GetGenericArguments().Length == genericParameterTypes.Length)
                    .Select(p => p.MakeGenericMethod(genericParameterTypes))
                    .Concat(methods.Where(p => !(p.IsGenericMethod && p.GetGenericArguments().Length == genericParameterTypes.Length)))
                    .ToArray();
            }

            var paramTypes = @params == null ? Type.EmptyTypes : @params.Select(p => p.Type).ToArray();
            var item = methods.FirstOrDefault(p =>
                p.GetParameters().Select(r => r.ParameterType).SequenceEqual(paramTypes));

            if (item != null)
                return item;

            var data = methods.Where(p =>
                p.IsGenericMethod && p.GetParameters().Length == paramTypes.Length)
                              .Select((p, i) => new
                              {
                                  Prameters = p.GetParameters(),
                                  Method = p,
                                  Order = string.Join(string.Empty, p.GetParameters().Select(r => r.ParameterType.FullName == null ? "1" : "0"))
                              })
                              .Where(p =>
                              {
                                  for (int i = 0; i < paramTypes.Length; i++)
                                  {
                                      var left = p.Prameters[i].ParameterType;
                                      if (left.FullName == null)
                                          continue;
                                      var right = paramTypes[i];
                                      if (left == right)
                                          continue;
                                      else
                                          return false;
                                  }
                                  return true;
                              })
                              .OrderBy(p => p.Order)
                              .ToArray();
            if (data.Length == 0)
                return null;

            // 只返回第一个符合条件的方法（已定类型的参数符合的优先）
            var first = data.First();
            var genericParamTypes = first.Order
                                         .Select((x, y) => x == '1' ? paramTypes[y] : null)
                                         .Where(p => p != null)
                                         .ToArray();

            var genericTypes = first.Prameters
                                    .Where(p => p.ParameterType.FullName == null)
                                    .ToList();

            var actualTypes = first.Method
                .GetGenericArguments()
                .Select(r => genericParamTypes[genericTypes.FindIndex(p => p.ParameterType.Name == r.Name)])
                .ToArray();

            return first.Method.MakeGenericMethod(actualTypes);
        }

        private void NumericExpressionOperator(ref ReadResult left, ref ReadResult right, Func<Expression, Expression, BinaryExpression> predicate)
        {
            if (left.Expression.Type != right.Expression.Type)
            {
                int leftLevel = PriorityManager.GetNumericLevel(left.Expression.Type);
                int rightLevel = PriorityManager.GetNumericLevel(right.Expression.Type);

                if (leftLevel > rightLevel)
                    right.Expression = Expression.Convert(right.Expression, left.Expression.Type);
                else
                    left.Expression = Expression.Convert(left.Expression, right.Expression.Type);
            }

            left.Expression = predicate(left.Expression, right.Expression);
            left.IsClosedWrap = left.IsClosedWrap || right.IsClosedWrap;
        }

        private Type[] GetTypes()
        {
            List<Type> types = new List<Type>();
            if (spResult.PeekNextIs(TokenId.LessThan))
                spResult.Next();

            do
            {
                types.Add(typeParser.ReadType());
                if (spResult.PeekNextIs(TokenId.Comma))
                    spResult.Next();
            } while (!spResult.PeekNextIs(TokenId.GreaterThan));
            spResult.NextIs(TokenId.GreaterThan, true);

            return types.ToArray();
        }
        #endregion

        #region MyRegion


        //public Expression ReadExpression(int operateLevel = 0, string wrapStart = null)
        //{
        //    Expression resultExp = null;
        //    Token token = spResult.Next();

        //    switch (token.ID)
        //    {
        //        case TokenId.Unknown:
        //        case TokenId.End:
        //            return null;
        //        case TokenId.Identifier:
        //            resultExp = IdentifierParser();
        //            break;
        //        case TokenId.StringLiteral:
        //            resultExp = Expression.Constant(token.Text, typeof(string));
        //            break;
        //        case TokenId.IntegerLiteral:
        //            resultExp = Expression.Constant(int.Parse(token.Text), typeof(int));
        //            break;
        //        case TokenId.RealLiteral:
        //            resultExp = Expression.Constant(double.Parse(token.Text), typeof(int));
        //            break;
        //        case TokenId.Exclamation:
        //            resultExp = ReadExpression(operateLevel, wrapStart);
        //            break;
        //        case TokenId.Percent:
        //            break;
        //        case TokenId.Amphersand:
        //            break;
        //        case TokenId.OpenParen:
        //            break;
        //        case TokenId.CloseParen:
        //            break;
        //        case TokenId.Asterisk:
        //            break;
        //        case TokenId.Plus:
        //            break;
        //        case TokenId.Comma:
        //            break;
        //        case TokenId.Minus:
        //            break;
        //        case TokenId.Dot:
        //            break;
        //        case TokenId.Slash:
        //            break;
        //        case TokenId.Colon:
        //            break;
        //        case TokenId.LessThan:
        //            break;
        //        case TokenId.Equal:
        //            break;
        //        case TokenId.GreaterThan:
        //            break;
        //        case TokenId.Question:
        //            break;
        //        case TokenId.OpenBracket:
        //            break;
        //        case TokenId.CloseBracket:
        //            break;
        //        case TokenId.Bar:
        //            break;
        //        case TokenId.ExclamationEqual:
        //            break;
        //        case TokenId.DoubleAmphersand:
        //            break;
        //        case TokenId.LessThanEqual:
        //            break;
        //        case TokenId.LessGreater:
        //            break;
        //        case TokenId.DoubleEqual:
        //            break;
        //        case TokenId.GreaterThanEqual:
        //            break;
        //        case TokenId.DoubleBar:
        //            break;
        //        case TokenId.LambdaPrefix:
        //            break;
        //        case TokenId.OpenBrace:
        //            break;
        //        case TokenId.CloseBrace:
        //            break;
        //        default:
        //            break;
        //    }

        //    return resultExp;
        //}

        //private Expression IdentifierParser()
        //{
        //    Expression currentExpression = null;
        //    var token = spResult.Current;
        //    if (token.ID != TokenId.Identifier)
        //        throw new ApplicationException("Not Identifier.");

        //    switch (token.Text)
        //    {
        //        #region case "null":
        //        case "null":
        //            currentExpression = Expression.Constant(null);
        //            break;
        //        #endregion

        //        #region case "true":
        //        case "true":
        //            currentExpression = Expression.Constant(true);
        //            break;
        //        #endregion

        //        #region case "false":
        //        case "false":
        //            currentExpression = Expression.Constant(false);
        //            break;
        //        #endregion

        //        #region case "sizeof":
        //        case "sizeof":
        //            {
        //                bool hasLeftParen = false;
        //                if (spResult.PeekNextTokenIdIs(TokenId.OpenParen))
        //                {
        //                    hasLeftParen = true;
        //                    spResult.Skip();
        //                }

        //                Type type = typeParser.ReadType();
        //                if (type == null)
        //                    throw new ApplicationException();
        //                if (hasLeftParen && spResult.NextTokenIdIs(TokenId.CloseParen, true))
        //                    currentExpression = Expression.Constant(Marshal.SizeOf(type));
        //            }
        //            break;
        //        #endregion

        //        #region case "typeof":
        //        case "typeof":
        //            {
        //                bool hasLeftParen = false;
        //                if (spResult.PeekNextTokenIdIs(TokenId.OpenParen))
        //                {
        //                    hasLeftParen = true;
        //                    spResult.Skip();
        //                }

        //                Type type = typeParser.ReadType();
        //                if (type == null)
        //                    throw new ApplicationException();
        //                if (hasLeftParen && spResult.NextTokenIdIs(TokenId.CloseParen, true))
        //                    currentExpression = Expression.Constant(type, typeof(Type));
        //            }
        //            break;
        //        #endregion

        //        #region case "new":
        //        case "new":
        //            {
        //                Type type = typeParser.ReadType();
        //                if (type == null)
        //                    throw new ApplicationException();

        //                // 是对象还是数组
        //                bool isSingleObject = false;
        //                // 是否需要读取括号中的参数
        //                bool needReadParameters = false;
        //                // 是否有对象或集合初始化
        //                bool hasOpenBrace = false;

        //                if (spResult.PeekNextTokenIdIs(TokenId.OpenParen))
        //                {
        //                    isSingleObject = true;
        //                    needReadParameters = true;
        //                }
        //                else if (spResult.PeekNextTokenIdIs(TokenId.OpenBracket))
        //                {
        //                    isSingleObject = false;
        //                }
        //                else if (spResult.PeekNextTokenIdIs(TokenId.OpenBrace))
        //                {
        //                    isSingleObject = true;
        //                    needReadParameters = false;
        //                    hasOpenBrace = true;
        //                }

        //                if (isSingleObject && !needReadParameters)
        //                    currentExpression = Expression.New(type);

        //                if (isSingleObject && needReadParameters && !hasOpenBrace)
        //                {
        //                    var parameters = ReadParameters(TokenId.OpenParen, TokenId.CloseParen, false);
        //                    currentExpression = Expression.New(type.GetConstructor(
        //                        parameters.Select(p => p.Type).ToArray()), parameters.ToArray());
        //                }
        //            }
        //            break;
        //        #endregion
        //        default:
        //            break;
        //    }

        //    return currentExpression;
        //}

        //private IEnumerable<Expression> ReadParameters(TokenId openWrap, TokenId closeWrap, bool hasReadPrefix = true)
        //{
        //    // 读前置括号
        //    if (!hasReadPrefix)
        //        spResult.NextTokenIdIs(openWrap, true);

        //    // 读参数
        //    List<Expression> result = new List<Expression>();
        //    while (!spResult.PeekNextTokenIdIs(closeWrap))
        //    {
        //        Expression expression = ReadExpression();
        //        if (expression == null)
        //            break;
        //        result.Add(expression);

        //        if (spResult.PeekNextTokenIdIs(TokenId.Comma))
        //            spResult.Next();
        //    }
        //    spResult.Skip();

        //    return result;
        //}
        //private Expression ReadExpression(Expression leftExp = null)
        //{
        //    Expression resultExp = null;
        //    Token token = spResult.Next();

        //    switch (token.ID)
        //    {
        //        case TokenId.Unknown:
        //            break;
        //        case TokenId.End:
        //            break;
        //        case TokenId.Identifier:
        //            {
        //                switch (token.Text)
        //                {
        //                    #region null true false
        //                    case "null":
        //                        resultExp = Expression.Constant(null);
        //                        break;
        //                    case "true":
        //                        resultExp = Expression.Constant(true);
        //                        break;
        //                    case "false":
        //                        resultExp = Expression.Constant(false);
        //                        break;
        //                    #endregion

        //                    #region sizeof
        //                    case "sizeof":
        //                        {

        //                            var content = GetBracketArray(false);
        //                            if (content == null)
        //                                throw new ParserResultException("sizeof can't nest parentheses");
        //                            if (content.Count != 1)
        //                                throw new ParserResultException("sizeof can't contain many types");
        //                            Type type = typeParser.GetType(content.Next().Text);
        //                            resultExp = Expression.Constant(Marshal.SizeOf(type));
        //                        }
        //                        break;
        //                    #endregion

        //                    #region typeof
        //                    case "typeof":
        //                        {
        //                            var content = GetBracketArray(false);
        //                            if (content == null)
        //                                throw new ParserResultException("sizeof can't nest parentheses");
        //                            if (content.Count != 1)
        //                                throw new ParserResultException("sizeof can't contain many types");
        //                            Type type = typeParser.GetType(content.Next().Text);
        //                            resultExp = Expression.Constant(type, typeof(Type));
        //                        }
        //                        break;
        //                    #endregion

        //                    #region case "new":
        //                    case "new":
        //                        {
        //                            // get new type
        //                            Type type = typeParser.ReadType();

        //                            // next symbol is ( or {
        //                            var next = spResult.Next();
        //                            if (next.ID == TokenId.OpenParen ||
        //                                next.ID == TokenId.OpenBrace)
        //                            {
        //                                if (next.ID == TokenId.OpenParen)
        //                                {
        //                                    // get constructor parameters
        //                                    var listParam = ReadParams(TokenId.OpenParen, true);

        //                                    // get constructor
        //                                    var ctor = type.GetConstructor(listParam.ToArray(m => m.Type));
        //                                    resultExp = Expression.New(ctor, listParam);
        //                                }
        //                                else
        //                                    resultExp = Expression.New(type.GetConstructor(Type.EmptyTypes));

        //                                // member init or collection init
        //                                if (next.ID == TokenId.OpenBrace)
        //                                {
        //                                    // test：member int or collection init?
        //                                    var position = spResult.Index;

        //                                    if (!spResult.NextTokenIdIs(TokenId.CloseBrace))
        //                                    {
        //                                        bool isMemberInit = spResult.NextTokenIdIs(TokenId.Equal);
        //                                        spResult.RevertFromIndex(position);

        //                                        // member init
        //                                        if (isMemberInit)
        //                                        {
        //                                            List<MemberBinding> listMemberBinding = new List<MemberBinding>();
        //                                            Token memberName;
        //                                            while ((memberName = spResult.Next()).ID != TokenId.CloseBrace)
        //                                            {
        //                                                spResult.NextTokenIdIs(TokenId.Equal, true);

        //                                                MemberInfo memberInfo = type.GetMember(memberName)[0];
        //                                                MemberBinding memberBinding = Expression.Bind(memberInfo, ReadExpression());// ReadExpression(0, wrapStart, out isClosedWrap));
        //                                                listMemberBinding.Add(memberBinding);

        //                                                // ,
        //                                                var comma = spResult.Next();
        //                                                if (comma.ID == TokenId.CloseBrace)
        //                                                    break;
        //                                                if (comma.ID != TokenId.Comma)
        //                                                    throw new ParserResultException("symbol isn't comma");
        //                                            }
        //                                            resultExp = Expression.MemberInit((NewExpression)resultExp, listMemberBinding);
        //                                        }
        //                                        // collection init
        //                                        else
        //                                        {
        //                                            List<Expression> listExpression = new List<Expression>();
        //                                            while (true)
        //                                            {
        //                                                listExpression.Add(ReadExpression());//0, wrapStart, out isClosedWrap));

        //                                                // ,
        //                                                var comma = spResult.Next();
        //                                                if (comma.ID == TokenId.CloseBrace)
        //                                                    break;
        //                                                if (comma.ID != TokenId.Comma)
        //                                                    throw new ParserResultException("symbol isn't comma");
        //                                            }
        //                                            resultExp = Expression.ListInit((NewExpression)resultExp, listExpression);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            else if (next.ID == TokenId.OpenBracket)
        //                            {
        //                                var nextStr = spResult.Next();

        //                                // 读[]里的长度
        //                                List<Expression> listLen = new List<Expression>();
        //                                if (nextStr.ID == TokenId.CloseBracket)
        //                                    spResult.Next();
        //                                else
        //                                    listLen = ReadParams(TokenId.OpenBracket, true).ToList();

        //                                // 读{}里的数组初始化
        //                                var start = spResult.Next();
        //                                if (start.ID == TokenId.OpenBrace)
        //                                {
        //                                    var listParams = ReadParams(TokenId.OpenBrace, true);
        //                                    resultExp = Expression.NewArrayInit(type, listParams);
        //                                }
        //                                else
        //                                    resultExp = Expression.NewArrayBounds(type, listLen);
        //                            }
        //                            else
        //                                throw new ParserResultException("");
        //                        }
        //                        break;
        //                    #endregion
        //                    default:
        //                        break;
        //                }
        //            }
        //            break;
        //        case TokenId.StringLiteral:
        //            break;
        //        case TokenId.IntegerLiteral:
        //        case TokenId.RealLiteral:
        //            resultExp = ReadNumberExpression(token);
        //            break;
        //        case TokenId.Exclamation:
        //            resultExp = Expression.Not(ReadExpression());
        //            break;
        //        case TokenId.Percent:
        //            break;
        //        case TokenId.Amphersand:
        //            break;
        //        case TokenId.OpenParen:
        //            {
        //                var data = GetBracketArray(true);
        //                Func<Token, bool> filter = p => p.ID != TokenId.Identifier;
        //                if (data.Any(filter))
        //                    throw new ParserResultException("类型转换异常在：" + data.First(filter).Index);
        //                if (!data.Any())
        //                    throw new ParserResultException("类型转换异常在：" + spResult.Current.Index);
        //                var type = typeParser.GetType(data[0].Text);

        //                resultExp = Expression.Convert(ReadExpression(), type);
        //            }
        //            break;
        //        case TokenId.CloseParen:
        //            break;
        //        case TokenId.Asterisk:
        //            break;
        //        case TokenId.Plus:
        //            resultExp = ReadExpression();
        //            break;
        //        case TokenId.Comma:
        //            break;
        //        case TokenId.Minus:
        //            {
        //                var right = ReadExpression();
        //                if (leftExp.Type == typeof(string) && right.Type == typeof(string))
        //                {
        //                    // 调用 string.Concat 方法
        //                    resultExp = Expression.Call(
        //                        typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) }),
        //                        Expression.Convert(leftExp, typeof(object)), Expression.Convert(right, typeof(object)));
        //                }
        //                //else
        //                //{
        //                //    AdjustNumberType(ref currentExpression, ref right);
        //                //    currentExpression = Expression.Add(currentExpression, right);
        //                //}
        //            }
        //            resultExp = Expression.Negate(ReadExpression());
        //            break;
        //        case TokenId.Dot:
        //            break;
        //        case TokenId.Slash:
        //            break;
        //        case TokenId.Colon:
        //            break;
        //        case TokenId.LessThan:
        //            break;
        //        case TokenId.Equal:
        //            break;
        //        case TokenId.GreaterThan:
        //            break;
        //        case TokenId.Question:
        //            break;
        //        case TokenId.OpenBracket:
        //            break;
        //        case TokenId.CloseBracket:
        //            break;
        //        case TokenId.Bar:
        //            break;
        //        case TokenId.ExclamationEqual:
        //            break;
        //        case TokenId.DoubleAmphersand:
        //            break;
        //        case TokenId.LessThanEqual:
        //            break;
        //        case TokenId.LessGreater:
        //            break;
        //        case TokenId.DoubleEqual:
        //            break;
        //        case TokenId.GreaterThanEqual:
        //            break;
        //        case TokenId.DoubleBar:
        //            break;
        //        default:
        //            break;
        //    }

        //    return resultExp;
        //}

        //private Expression ReadNumberExpression(Token token)
        //{
        //    object constValue = null;

        //    switch (token.ID)
        //    {
        //        case TokenId.IntegerLiteral:
        //            var next = spResult.PeekNext();
        //            if (next.ID == TokenId.Identifier)
        //            {
        //                spResult.Skip();
        //                switch (next.Text)
        //                {
        //                    case "l":
        //                    case "L":
        //                        constValue = long.Parse(token.Text);
        //                        break;
        //                    case "m":
        //                    case "M":
        //                        constValue = decimal.Parse(token.Text);
        //                        break;
        //                    case "f":
        //                    case "F":
        //                        constValue = float.Parse(token.Text);
        //                        break;
        //                    case "d":
        //                    case "D":
        //                        constValue = double.Parse(token.Text);
        //                        break;
        //                    default:
        //                        throw new ParserResultException("整型数字分析异常在：" + next.Index);
        //                }
        //            }
        //            else
        //                constValue = int.Parse(token.Text);
        //            break;
        //        case TokenId.RealLiteral:
        //            next = spResult.PeekNext();
        //            if (next.ID == TokenId.Identifier)
        //            {
        //                spResult.Skip();
        //                switch (next.Text)
        //                {
        //                    case "m":
        //                    case "M":
        //                        constValue = decimal.Parse(token.Text);
        //                        break;
        //                    case "f":
        //                    case "F":
        //                        constValue = float.Parse(token.Text);
        //                        break;
        //                    case "d":
        //                    case "D":
        //                        constValue = double.Parse(token.Text);
        //                        break;
        //                    default:
        //                        throw new ParserResultException("浮点数字分析异常在：" + next.Index);
        //                }
        //            }
        //            else
        //                constValue = double.Parse(token.Text);
        //            break;
        //        default:
        //            throw new ParserResultException("数字分析异常（不是数字）在：" + token.Index);
        //    }

        //    return Expression.Constant(constValue);
        //}

        //private void AdjustNumber(ref Expression left, ref Expression right)
        //{
        //    if (left.Type == right.Type)
        //        return;

        //    int leftLevel = numericTypeLevel[GetNoNullableType(left.Type)];
        //    int rightLevel = numericTypeLevel[GetNoNullableType(right.Type)];

        //    if (leftLevel > rightLevel)
        //        right = Expression.Convert(right, left.Type);
        //    else
        //        left = Expression.Convert(left, right.Type);
        //}

        //private Type GetNoNullableType(Type type)
        //{
        //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        //        return type.GetGenericArguments()[0];
        //    else
        //        return type;
        //}

        //private IEnumerable<Expression> ReadParams(TokenId openSymbol, bool hasReadPrefix = false)
        //{
        //    yield return default(Expression);
        //}
        #endregion
    }
}
