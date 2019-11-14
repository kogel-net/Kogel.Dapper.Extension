using Dapper;
using System;
using System.Data;

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
	}
}