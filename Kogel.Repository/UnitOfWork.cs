using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Kogel.Dapper.Extension;
using Kogel.Repository.Interfaces;

namespace Kogel.Repository
{
    /// <summary>
    /// 
    /// </summary>
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

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 上下文中所有得工作单元
        /// </summary>
        private static AsyncLocal<List<UnitOfWork>> _unitOfWorkContext = new AsyncLocal<List<UnitOfWork>>();
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public UnitOfWork(IDbConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public UnitOfWork(IDbConnection connection, IDbTransaction transaction)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="transactionMethod"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        [UnitOfWorkAttrbute]
        public IUnitOfWork BeginTransaction(Action transactionMethod, System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted)
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            if (Transaction == null)
                Transaction = Connection.BeginTransaction(isolationLevel);
            try
            {
                SqlMapper.Aop.OnExecuting += Aop_OnExecuting;

#if NETCOREAPP || NETSTANDARD
                lock (_unitOfWorkContext)
                {
                    if (_unitOfWorkContext.Value == null)
                        _unitOfWorkContext.Value = new List<UnitOfWork>();
                    _unitOfWorkContext.Value.Add(this);
                }
#endif
                transactionMethod.Invoke();
            }
            //catch (Exception ex)
            //{
            //    //this.Rollback();此处回滚无效
            //    throw ex;
            //}
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
            //是否进入过工作单元(防止循环嵌套UnitOfWork)
            if (!command.IsUnitOfWork)
            {
                var dbConnection = this.Connection as DbConnection;
                //是否排除在工作单元外
                if (command.IsExcludeUnitOfWork)
                {
                    var connectionFunc = Global.ConnectionPool
                        .FirstOrDefault(x =>
                            (x.ConnectionString.Contains(this.Connection.ConnectionString))
                            || (x.DataSource == dbConnection.DataSource && x.Database == dbConnection.Database)
                            );
                    if (connectionFunc == null)
                        throw new DapperExtensionException($"连接未注入{this.Connection.ConnectionString}");
                    command.Connection = connectionFunc.FuncConnection.Invoke(null);
                    command.Transaction = null;
                    return;
                }

                if (MatchConnection(this.Connection, command.Connection))
                {
                    command.IsUnitOfWork = true;

                    command.Connection = this.Connection;
                    command.Transaction = this.Transaction;
                }
                else
                {
#if NETCOREAPP || NETSTANDARD
                    lock (_unitOfWorkContext)
                    {
                        var commandConnection = command.Connection as DbConnection;
                        //如果不同连接就从上下文中取出
                        var contextUnitOfWork = _unitOfWorkContext.Value.FirstOrDefault(x => MatchConnection(x.Connection, commandConnection));
                        if (contextUnitOfWork != null)
                        {
                            command.Connection = contextUnitOfWork.Connection;
                            command.Transaction = contextUnitOfWork.Transaction;
                        }
                        else
                        {
                            //否则就产生一个新的放到上下文中
                            var connectionFunc = Global.ConnectionPool
                                .FirstOrDefault(x =>
                              (x.ConnectionString.Contains(this.Connection.ConnectionString))
                              || (x.DataSource == commandConnection.DataSource && x.Database == commandConnection.Database)
                              );
                            if (connectionFunc == null)
                                throw new DapperExtensionException($"连接未注入{this.Connection.ConnectionString}");
                            command.Connection = connectionFunc.FuncConnection.Invoke(null);
                            command.Connection.Open();
                            command.Transaction = command.Connection.BeginTransaction();
                            _unitOfWorkContext.Value.Add(new UnitOfWork(command.Connection, command.Transaction));
                        }
                    }
#endif
                }
            }
        }

        /// <summary>
        /// 匹配是否同一连接
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandConnection"></param>
        /// <returns></returns>
        private bool MatchConnection(IDbConnection connection, IDbConnection commandConnection)
        {
            var commandDBConnection = commandConnection as DbConnection;
            var dbConnection = connection as DbConnection;
            //相同数据库链接才会进入单元事务
            if (commandConnection.ConnectionString.Contains(this.Connection.ConnectionString)
            || (commandDBConnection.DataSource == dbConnection.DataSource && commandConnection.Database == dbConnection.Database))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
                if (!IsAnyUnitOfWork())
                {
#if NET45 || NET451
                    Transaction.Commit();
#else
                    //上下文中所有事务提交
                    _unitOfWorkContext.Value.ForEach(x => x.Transaction.Commit());
#endif
                }
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            if (Transaction != null)
                if (!IsAnyUnitOfWork())
                {
#if NET45 || NET451
                    Transaction.Rollback();
#else
                    //上下文中所有事务提交
                    _unitOfWorkContext.Value.ForEach(x => x.Transaction.Rollback());
#endif
                }
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            if (Transaction != null)
            {
#if NET45 || NET451
                Transaction.Dispose();
#else
                //上下文中所有事务提交
                _unitOfWorkContext.Value.ForEach(x => x.Transaction.Dispose());
#endif
            }

            GC.SuppressFinalize(this);
        }

        ~UnitOfWork() => this.Dispose();

        /// <summary>
        /// 是否存在最外层嵌套单元
        /// </summary>
        /// <returns></returns>
        private static bool IsAnyUnitOfWork()
        {
            //嵌套的Unitofwork数量
            var count = 0;
            //当前堆栈信息
            StackTrace st = new StackTrace();
            StackFrame[] sfs = st.GetFrames();
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                var method = sfs[i].GetMethod();//方法
                if (method.CustomAttributes.Any(x => x.AttributeType == typeof(UnitOfWorkAttrbute)))
                    count++;
            }
            return count > 0;
        }

        /// <summary>
        /// 仓储方法标记 （内部使用）
        /// </summary>
        private sealed class UnitOfWorkAttrbute : Attribute
        {
        }
    }
}
