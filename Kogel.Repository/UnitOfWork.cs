using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kogel.Repository.Interfaces;

namespace Kogel.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// 工作单元事务
        /// </summary>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public UnitOfWork(IDbConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="transactionMethod"></param>
        /// <param name="IsolationLevel"></param>
        /// <returns></returns>
        public IUnitOfWork BeginTransaction(Action transactionMethod, IsolationLevel IsolationLevel = IsolationLevel.Serializable)
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
            Transaction = Connection.BeginTransaction(IsolationLevel);
            try
            {
                SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
                transactionMethod.Invoke();
            }
            catch (Exception ex)
            {
                if (Transaction != null)
                    Transaction.Rollback();
                throw ex;
            }
            finally
            {
                SqlMapper.Aop.OnExecuting -= Aop_OnExecuting;
            }
            return this;
        }

        /// <summary>
        /// 工作单元内所有访问数据库操作执行前
        /// </summary>
        /// <param name="command"></param>
        private void Aop_OnExecuting(ref CommandDefinition command)
        {
            //相同数据库链接才会进入单元事务
            if (command.Connection.ConnectionString.Contains(this.Connection.ConnectionString))
            {
                //是否进入过工作单元(防止循环嵌套UnitOfWork)
                if (!command.IsUnifOfWork)
                {
                    command.IsUnifOfWork = true;
                    command.Connection = this.Connection;
                    command.Transaction = this.Transaction;
                }
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
                Transaction.Commit();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            if (Transaction != null)
                Transaction.Rollback();
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            if (Transaction != null)
                Transaction.Dispose();

            if (Connection != null)
                Connection.Dispose();
        }
    }
}
