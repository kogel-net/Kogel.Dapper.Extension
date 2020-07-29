using Dapper;
using System;
using System.Data;
using System.Threading;

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

		private static ThreadLocal<AopProvider> _aop = new ThreadLocal<AopProvider>();

		/// <summary>
		/// 获取当前线程唯一Aop
		/// </summary>
		/// <returns></returns>
		public static AopProvider Get()
		{
			if (_aop.Value == null)
			{
				//_aop = new ThreadLocal<AopProvider>();
				_aop.Value = new AopProvider();
			}
			return _aop.Value;
		}
	}
}