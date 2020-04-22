using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digiwin.MES.Server.Infrastructure.Core.DataBase.Model
{
    /// <summary>
    /// 泛型实体基类
    /// </summary>
    /// <typeparam name="TPrimaryKey">主键</typeparam>
    public abstract class EntityBase<TPrimaryKey> : IBaseEntity<EntityBase, string>
    {
        [Identity(IsIncrease = false)]
        /// <summary>
        /// 主键
        /// </summary>
        public virtual TPrimaryKey GUID { get; set; }

        public virtual string FACTORY { get; set; }

        public virtual string CREATOR { get; set; }

        public virtual DateTime CREATE_TIME { get; set; }

        public virtual string MODIFIER { get; set; }

        public virtual DateTime MODIFY_TIME { get; set; }

        public virtual bool FLAG { get; set; }

        public virtual string DELETE_FLAG { get; set; }
    }

    #region Implements

    /// <summary>
    /// Guid 类型主键实体基类
    /// </summary>
    public abstract class EntityBase : EntityBase<string>
    { }
        
    #endregion
}
