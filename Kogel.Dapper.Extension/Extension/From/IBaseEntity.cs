using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Model;
using System.Reflection;
using Dapper;
using Kogel.Dapper.Extension.Exception;

namespace Kogel.Dapper.Extension
{
    public class IBaseEntity<T> where T : class
    {
        private QuerySet<T> querySet;
        private CommandSet<T> commandSet;
        private PropertyInfo identityProperty;
        private EntityObject entityObject;
        /// <summary>
        /// 初始化单元任务
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlProvider"></param>
        /// <param name="dbTransaction"></param>
        public void Init(IDbConnection connection, SqlProvider sqlProvider, IDbTransaction dbTransaction = null)
        {
            //创建查询和执行对象的实例
            querySet = new QuerySet<T>(connection, sqlProvider, dbTransaction);
            commandSet = new CommandSet<T>(connection, sqlProvider, dbTransaction);
            //检测是否设置了主键
            entityObject = EntityCache.QueryEntity(this.GetType());
            foreach (var propertiy in entityObject.Properties)
            {
                var identity = propertiy.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Equals(typeof(Identity)));
                if (identity != null)
                {
                    this.identityProperty = propertiy;
                    break;
                }
            }
            if (identityProperty == null)
            {
                throw new DapperExtensionException("主键不能为空，请使用[Identity]特性设置主键！！！");
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public void Insert()
        {
            int identityValue = commandSet.InsertIdentity(this as T);
            identityProperty.SetValue(this, identityValue);
            this.Clear();
        }
        /// <summary>
        /// 修改
        /// </summary>
        public void Update()
        {
            //主键名称
            var identityName = entityObject.FieldPairs[identityProperty.Name];
            //主键值
            object identityValue = identityProperty.GetValue(this);
            if (string.IsNullOrEmpty(identityName) || identityValue == null)
            {
                throw new DapperExtensionException("主键不能为空，请使用[Identity]特性设置主键！！！");
            }
            //条件字段
            var paramName = commandSet.SqlProvider.ProviderOption.ParameterPrefix + identityName;
            commandSet.WhereBuilder.Append($" AND {identityName}={paramName}");
            commandSet.Params.Add(paramName, identityValue);
            commandSet.Update(this as T);
            this.Clear();
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            //主键名称
            var identityName = entityObject.FieldPairs[identityProperty.Name];
            //主键值
            object identityValue = identityProperty.GetValue(this);
            if (string.IsNullOrEmpty(identityName) || identityValue == null)
            {
                throw new DapperExtensionException("主键不能为空，请使用[Identity]特性设置主键！！！");
            }
            //条件字段
            var paramName = commandSet.SqlProvider.ProviderOption.ParameterPrefix + identityName;
            commandSet.WhereBuilder.Append($" AND {identityName}={paramName}");
            commandSet.Params.Add(paramName, identityValue);
            commandSet.Delete();
            this.Clear();
        }
        /// <summary>
        /// 执行完成一个操作后清除条件
        /// </summary>
        private void Clear()
        {
            commandSet.WhereExpressionList.Clear();
            commandSet.Params = new DynamicParameters();
            commandSet.WhereBuilder.Clear();
            commandSet.SqlProvider.SqlString = string.Empty;
            commandSet.SqlProvider.Params = new DynamicParameters();
        }
    }
}
