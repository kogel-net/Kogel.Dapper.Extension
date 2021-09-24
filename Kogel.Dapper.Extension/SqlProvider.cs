using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Extension;
using Kogel.Dapper.Extension.Entites;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Helper;
using Kogel.Dapper.Extension;
using System.Data;
using Kogel.Dapper.Extension.Expressions;

namespace Kogel.Dapper.Extension
{
    public abstract class SqlProvider
    {
        public AbstractDataBaseContext Context { get; set; }

        protected SqlProvider()
        {
            Params = new DynamicParameters();
            JoinList = new List<JoinAssTable>();
            AsTableNameDic = new Dictionary<Type, string>();
        }

        /// <summary>
        /// 工具方
        /// </summary>
        public abstract IProviderOption ProviderOption { get; set; }

        /// <summary>
        /// 解析者
        /// </summary>
        public abstract IResolveExpression ResolveExpression { get; set; }

        /// <summary>
        /// 生成的sql
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 连接对象集合
        /// </summary>
        public List<JoinAssTable> JoinList { get; set; }

        /// <summary>
        /// 参数对象
        /// </summary>
        public DynamicParameters Params { get; set; }

        /// <summary>
        /// 重命名目录
        /// </summary>
        public Dictionary<Type, string> AsTableNameDic { get; set; }

        /// <summary>
        /// 是否排除在工作单元外
        /// </summary>
        public bool IsExcludeUnitOfWork { get; set; }

        public abstract SqlProvider FormatGet<T>();

        public abstract SqlProvider FormatToList<T>();

        public abstract SqlProvider FormatToPageList<T>(int pageIndex, int pageSize);

        public abstract SqlProvider FormatCount();

        public abstract SqlProvider FormatDelete();

        public abstract SqlProvider FormatInsert<T>(T entity, string[] excludeFields);

        public abstract SqlProvider FormatInsert<T>(IEnumerable<T> entitys, string[] excludeFields);

        public abstract SqlProvider FormatInsertIdentity<T>(T entity, string[] excludeFields);

        public abstract SqlProvider FormatUpdate<T>(Expression<Func<T, T>> updateExpression);

        public abstract SqlProvider FormatUpdate<T>(T entity, string[] excludeFields);

        public abstract SqlProvider FormatUpdate<T>(IEnumerable<T> entity, string[] excludeFields);

        public abstract SqlProvider FormatSum(LambdaExpression sumExpression);

        public abstract SqlProvider FormatMin(LambdaExpression MinExpression);

        public abstract SqlProvider FormatMax(LambdaExpression MaxExpression);

        public abstract SqlProvider FormatUpdateSelect<T>(Expression<Func<T, T>> updator);

        public abstract SqlProvider Create();

        public virtual SqlProvider Clear()
        {
            Params.Clear();
            ProviderOption.MappingList.Clear();
            ProviderOption.NavigationList.Clear();
            return this;
        }

        /// <summary>
        /// 条件是否需要加上asname（一般修改和删除时不需要）
        /// </summary>
        public bool IsAppendAsName { get; set; } = true;

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="isNeedFrom"></param>
        /// <param name="isAsName"></param>
        /// <param name="tableType">连接查询时会用到</param>
        /// <returns></returns>

        public virtual string FormatTableName(bool isNeedFrom = true, bool isAsName = true, Type tableType = null)
        {
            //实体解析类型
            var entity = EntityCache.QueryEntity(tableType == null ? Context.Set.TableType : tableType);
            string schema = string.IsNullOrEmpty(entity.Schema) ? "" : ProviderOption.CombineFieldName(entity.Schema) + ".";
            string fromName = entity.Name;
            //函数AsTableName优先级大于一切
            string asTableName;
            if (AsTableNameDic.TryGetValue(entity.Type, out asTableName))
            {
                fromName = asTableName;
            }
            //是否存在实体特性中的AsName标记
            if (isAsName)
            {
                if (entity.AsName.Equals(fromName))
                {
                    fromName = ProviderOption.CombineFieldName(fromName);
                }
                else
                {
                    //加上as标记
                    fromName = $"{ProviderOption.CombineFieldName(fromName)} {entity.AsName}";
                }
            }
            else
            {
                fromName = ProviderOption.CombineFieldName(fromName);
            }
            string sqlString = $" {schema}{fromName} ";
            //是否需要FROM
            if (isNeedFrom)
                sqlString = $" FROM {sqlString}";
            return sqlString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected DataBaseContext<T> DataBaseContext<T>() => (DataBaseContext<T>)Context;

        /// <summary>
        /// 根据主键获取条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual string GetIdentityWhere<T>(T entite, DynamicParameters param = null)
        {
            var entityObject = EntityCache.QueryEntity(typeof(T));
            if (string.IsNullOrEmpty(entityObject.Identitys))
                throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");

            //设置参数
            if (param != null)
            {
                //获取主键数据
                var id = entityObject.EntityFieldList
                    .FirstOrDefault(x => x.FieldName == entityObject.Identitys)
                    .PropertyInfo
                    .GetValue(entite);
                param.Add(entityObject.Identitys, id);
            }
            return $" AND {entityObject.Identitys}={ProviderOption.ParameterPrefix}{entityObject.Identitys} ";
        }

        /// <summary>
        /// 根据主键获取条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual string GetIdentityWhere<T>(IEnumerable<T> entites, DynamicParameters param = null)
        {
            var entityObject = EntityCache.QueryEntity(typeof(T));
            if (string.IsNullOrEmpty(entityObject.Identitys))
                throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");

            //设置参数
            if (param != null)
            {
                List<object> idList = new List<object>();
                foreach (var entite in entites)
                {
                    //获取主键数据
                    var id = entityObject.EntityFieldList
                        .FirstOrDefault(x => x.FieldName == entityObject.Identitys)
                        .PropertyInfo
                        .GetValue(entite);
                    idList.Add(id);
                }
                param.Add(entityObject.Identitys, idList);
            }
            return $" AND {entityObject.Identitys} IN {ProviderOption.ParameterPrefix}{entityObject.Identitys} ";
        }

        /// <summary>
        /// 根据主键获取条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual string GetIdentityWhere<T>(object id, DynamicParameters param = null)
        {
            var entityObject = EntityCache.QueryEntity(typeof(T));
            if (string.IsNullOrEmpty(entityObject.Identitys))
                throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");

            //设置参数
            if (param != null)
            {
                param.Add(entityObject.Identitys, id);
            }
            return $" AND {entityObject.Identitys}={ProviderOption.ParameterPrefix}{entityObject.Identitys} ";
        }

        /// <summary>
        /// 自定义条件生成表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dynamicTree"></param>
        /// <returns></returns>
        public virtual IEnumerable<LambdaExpression> FormatDynamicTreeWhereExpression<T>(Dictionary<string, DynamicTree> dynamicTree)
        {
            foreach (var key in dynamicTree.Keys)
            {
                DynamicTree tree = dynamicTree[key];
                if (tree != null && !string.IsNullOrEmpty(tree.Value))
                {
                    Type tableType = typeof(T);
                    if (!string.IsNullOrEmpty(tree.Table))
                    {
                        tableType = EntityCache.QueryEntity(tree.Table).Type;
                    }
                    //如果不存在对应表就使用默认表
                    ParameterExpression param = Expression.Parameter(tableType, "param");
                    object value = tree.Value;
                    if (value == null)
                    {
                        continue;
                    }
                    else if (tree.ValueType == DbType.DateTime)
                    {
                        value = Convert.ToDateTime(value);
                    }
                    else if (tree.ValueType == DbType.String)
                    {
                        value = Convert.ToString(value);
                        if ("" == value.ToString())
                        {
                            continue;
                        }
                    }
                    else if (tree.ValueType == DbType.Int32)
                    {
                        int number = Convert.ToInt32(value);
                        value = number;
                        if (0 == number)
                        {
                            continue;
                        }
                    }
                    else if (tree.ValueType == DbType.Boolean)
                    {
                        if (value.ToString() == "")
                        {
                            continue;
                        }
                        value = Convert.ToBoolean(value);
                    }
                    Expression whereExpress = null;
                    switch (tree.Operators)
                    {
                        case ExpressionType.Equal://等于
                            whereExpress = Expression.Equal(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                        case ExpressionType.GreaterThanOrEqual://大于等于
                            whereExpress = Expression.GreaterThanOrEqual(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                        case ExpressionType.LessThanOrEqual://小于等于
                            whereExpress = Expression.LessThanOrEqual(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                        case ExpressionType.Call://模糊查询
                            var method = typeof(string).GetMethodss().FirstOrDefault(x => x.Name.Equals("Contains"));
                            whereExpress = Expression.Call(Expression.Property(param, tree.Field), method, new Expression[] { Expression.Constant(value) });
                            break;
                        default:
                            whereExpress = Expression.Equal(Expression.Property(param, tree.Field), Expression.Constant(value));
                            break;
                    }
                    var lambdaExp = Expression.Lambda(TrimExpression.Trim(whereExpress), param);
                    //WhereExpressionList.Add();
                    yield return lambdaExp;
                }
            }
        }
    }
}
