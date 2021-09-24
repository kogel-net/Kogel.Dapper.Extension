using Kogel.Dapper.Extension;
using System.Threading;

#if NET45 || NET451
using System.Runtime.Remoting.Messaging;
#endif


namespace Kogel.Dapper.Extension
{
    public class AopProvider
    {
        /// <summary>
        /// 事件模型定义
        /// </summary>
        /// <param name="command"></param>
        public delegate void EventHander(ref CommandDefinition command);

        /// <summary>
        /// 执行前
        /// </summary>
        public event EventHander OnExecuting;

        /// <summary>
        /// 执行后
        /// </summary>
        public event EventHander OnExecuted;

        /// <summary>
        /// 触发执行前
        /// </summary>
        /// <param name="definition"></param>
        internal void InvokeExecuting(ref CommandDefinition definition)
        {
            this.OnExecuting?.Invoke(ref definition);
        }
        /// <summary>
        /// 触发执行后
        /// </summary>
        /// <param name="definition"></param>
        internal void InvokeExecuted(ref CommandDefinition definition)
        {
            this.OnExecuted?.Invoke(ref definition);
        }

#if NET45 || NET451
        //private static ThreadLocal<AopProvider> _aop = new ThreadLocal<AopProvider>();
#else
                private static AsyncLocal<AopProvider> _aop = new AsyncLocal<AopProvider>();
#endif

        /// <summary>
        /// 获取当前线程唯一Aop
        /// </summary>
        /// <returns></returns>
        public static AopProvider Get()
        {
#if NET45 || NET451
            string contextKey = typeof(AopProvider).FullName;
            var _aop = CallContext.LogicalGetData(contextKey);
            if (_aop == null)
            {
                _aop = new AopProvider();
                CallContext.LogicalSetData(contextKey, _aop);
            }
            return _aop as AopProvider;
#else  
            lock (_aop) 
            {
                if (_aop.Value == null)
                {
                    //_aop = new ThreadLocal<AopProvider>();
                    _aop.Value = new AopProvider();
                }
                return _aop.Value;
            }
#endif
        }
    }
}