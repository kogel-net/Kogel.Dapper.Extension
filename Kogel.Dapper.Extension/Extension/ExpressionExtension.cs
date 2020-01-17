using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Kogel.Dapper.Extension.Exception;
using Kogel.Dapper.Extension.Expressions;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension.Extension
{
	public static class ExpressionExtension
	{
		#region 表达式类型字典
		/// <summary>
		/// 表达式类型字典
		/// </summary>
		public static readonly Dictionary<ExpressionType, string> NodeTypeDic = new Dictionary<ExpressionType, string>
		{
			{ExpressionType.AndAlso," AND "},
			{ExpressionType.OrElse," OR "},
			{ExpressionType.Equal," = "},
			{ExpressionType.NotEqual," != "},
			{ExpressionType.LessThan," < "},
			{ExpressionType.LessThanOrEqual," <= "},
			{ExpressionType.GreaterThan," > "},
			{ExpressionType.GreaterThanOrEqual," >= "},
			{ExpressionType.Add," + " },
			{ExpressionType.Subtract," - " },
			{ExpressionType.Multiply," * " },
			{ExpressionType.Divide," / "},
			{ExpressionType.Modulo," % " }
		};
		#endregion

		#region 获取表达式类型转换结果
		/// <summary>
		/// 获取表达式类型转换结果
		/// </summary>
		/// <param name="node">二元表达式</param>
		/// <returns></returns>
		public static string GetExpressionType(this BinaryExpression node)
		{
			var nodeTypeDic = NodeTypeDic[node.NodeType];

			string nodeType = null;
			if (node.Right.NodeType == ExpressionType.Constant && ((ConstantExpression)node.Right).Value == null)
			{
				switch (node.NodeType)
				{
					case ExpressionType.Equal:
						nodeType = " IS ";
						break;
					case ExpressionType.NotEqual:
						nodeType = " IS NOT ";
						break;
				}
			}

			return !string.IsNullOrEmpty(nodeType) ? nodeType : nodeTypeDic;
		}
		#endregion

		#region 获取最底层成员表达式
		/// <summary>
		/// 获取最底层成员表达式
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static MemberExpression GetRootMember(this MemberExpression e)
		{
			if (e.Expression == null || e.Expression.NodeType == ExpressionType.Constant)
				return e;

			return e.Expression.NodeType == ExpressionType.MemberAccess
				? ((MemberExpression)e.Expression).GetRootMember()
				: null;
		}
		#endregion

		#region 转换成一元表达式并取值
		/// <summary>
		/// 转换成一元表达式并取值（同时会计算方法）
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static object ToConvertAndGetValue(this Expression expression)
		{
			if (expression.Type != typeof(object))
				expression = Expression.Convert(expression, typeof(object));

			var lambdaExpression = Expression.Lambda<Func<object>>(expression);
			return lambdaExpression.Compile().Invoke();
		}
		#endregion

		public static object MemberToValue(this MemberExpression memberExpression)
		{
			var topMember = GetRootMember(memberExpression);
			if (topMember == null)
				throw new InvalidOperationException("需计算的条件表达式只支持由 MemberExpression 和 ConstantExpression 组成的表达式");

			return memberExpression.MemberToValue(topMember);
		}

		public static object MemberToValue(this MemberExpression memberExpression, MemberExpression topMember)
		{
			if (topMember.Expression == null)
			{
				//var aquire = Cache.GetOrAdd(memberExpression.ToString(), key => GetStaticProperty(memberExpression));
				var aquire = GetStaticProperty(memberExpression);
				return aquire(null, null);
			}
			else
			{
				//var aquire = Cache.GetOrAdd(memberExpression.ToString(), key => GetInstanceProperty(memberExpression, topMember));

				var aquire = GetInstanceProperty(memberExpression, topMember);
				return aquire((topMember.Expression as ConstantExpression).Value, null);
			}
		}

		private static Func<object, object[], object> GetInstanceProperty(Expression e, MemberExpression topMember)
		{
			var parameter = Expression.Parameter(typeof(object), "local");
			var parameters = Expression.Parameter(typeof(object[]), "args");
			var castExpression = Expression.Convert(parameter, topMember.Member.DeclaringType);
			var localExpression = topMember.Update(castExpression);
			var replaceExpression = ExpressionModifier.Replace(e, topMember, localExpression);
			replaceExpression = Expression.Convert(replaceExpression, typeof(object));
			var compileExpression = Expression.Lambda<Func<object, object[], object>>(replaceExpression, parameter, parameters);
			return compileExpression.Compile();
		}

		private static Func<object, object[], object> GetStaticProperty(Expression e)
		{
			var parameter = Expression.Parameter(typeof(object), "local");
			var parameters = Expression.Parameter(typeof(object[]), "args");
			var convertExpression = Expression.Convert(e, typeof(object));
			var compileExpression = Expression.Lambda<Func<object, object[], object>>(convertExpression, parameter, parameters);
			return compileExpression.Compile();
		}

		public static string GetColumnAttributeName(this MemberInfo memberInfo)
		{
			return memberInfo.Name;
		}
		/// <summary>
		/// 获取表达式的字段名
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string GetCorrectPropertyName<T>(this Expression<Func<T, Object>> expression)
		{
			if (expression.Body is MemberExpression)
			{
				return ((MemberExpression)expression.Body).Member.Name;
			}
			else
			{
				var op = ((UnaryExpression)expression.Body).Operand;
				return ((MemberExpression)op).Member.Name;
			}
		}
		public static string GetCorrectPropertyName(this Expression expression)
		{
			if (expression is MemberExpression)
			{
				return ((MemberExpression)expression).Member.Name;
			}
			else
			{
				var op = ((UnaryExpression)expression).Operand;
				return ((MemberExpression)op).Member.Name;
			}
		}
		/// <summary>
		/// 子查询转sql
		/// </summary>
		/// <param name="expression">表达式</param>
		/// <param name="Param">返回的参数</param>
		/// <returns></returns>
		public static string MethodCallExpressionToSql(this MethodCallExpression expression, ref DynamicParameters Param)
		{
			//解析子查询
			var subquery = new SubqueryExpression(expression);
			Param = subquery.Param;
			return subquery.SqlCmd;
		}

		public static LambdaExpression GetLambdaExpression(this Expression expression)
		{
			return (LambdaExpression)(((UnaryExpression)(expression)).Operand);
		}

		/// <summary>
		/// 成员表达式转lambda表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static LambdaExpression GetLambdaExpressionByMember(this Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			return Expression.Lambda(memberExpression, memberExpression.Expression as ParameterExpression);
		}
		/// <summary>
		/// 是否继承了IBaseEntity
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsAnyBaseEntity(Type type, out Type entityType)
		{
			if (type.BaseType == null)
			{
				entityType = type;
				return false;
			}
			else if (type.FullName.Contains("System.Collections.Generic"))//泛型
			{
				return IsAnyBaseEntity(type.GenericTypeArguments[0], out entityType);
			}
			else if (type.BaseType.FullName.Contains("Kogel.Dapper.Extension.IBaseEntity"))
			{
				entityType = type.BaseType.GenericTypeArguments[0];
				return true;
			}
			else
			{
				return IsAnyBaseEntity(type.BaseType, out entityType);
			}
		}

		/// <summary>
		/// 克隆list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static List<TEntity> Clone<TEntity>(IEnumerable<TEntity> list) where TEntity : ICloneable
		{
			return list.Select(item => (TEntity)item.Clone()).ToList();
		}

		/// <summary>
		/// 根据名称获取实例值
		/// </summary>
		/// <param name="entityObject"></param>
		/// <param name="name"></param>
		/// <param name="entityObj"></param>
		public static object GetPropertyValue(EntityObject entityObject, string name, object entityObj)
		{
			PropertyInfo property = entityObject.Properties.FirstOrDefault(x => x.Name == name);
			return property.GetValue(entityObj);
		}

		/// <summary>
		/// 写入值对象
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="masterData"></param>
		/// <param name="joinTableList"></param>
		/// <param name="index"></param>
		public static void SetProperValue<TMaster, TEntity>(TMaster master, TEntity entityValue, PropertyInfo propertyInfo)
		{
			if (propertyInfo.PropertyType == typeof(TEntity))
			{
				propertyInfo.SetValue(master, entityValue);
			}
			else
			{
				List<TEntity> entities = (List<TEntity>)propertyInfo.GetValue(master);
				if (entities == null)
				{
					entities = new List<TEntity>() { entityValue };
				}
				else
				{
					entities.Add(entityValue);
				}
				propertyInfo.SetValue(master, entities);
			}
		}


		/// <summary>
		///判断两个类型是否 
		/// </summary>
		/// <param name="type1"></param>
		/// <param name="type2"></param>
		/// <returns></returns>
		public static bool IsTypeEquals(this Type type1, Type type2)
		{
			if (type1 == type2)
			{
				return true;
			}
			else if (type1.BaseType == null || type2.BaseType == null)
			{
				return false;
			}
			else
			{
				return IsTypeEquals(type1.BaseType, type2);
			}
		}
	}
}
