using System;
using System.Data;
using System.Linq.Expressions;
using Kogel.Dapper.Extension.Core.Interfaces;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    /// <summary>
    /// 配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Option<T> : Query<T>, IOption<T>
    {
        protected Option(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {

        }

        protected Option(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {

        }

        public int? TopNum { get; set; }

        /// <summary>
		/// 返回对应行数数据（此方法只对mssql有效）
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
        public virtual Option<T> Top(int num)
        {
            TopNum = num;
            return this;
        }
    }
}
