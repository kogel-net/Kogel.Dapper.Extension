using Dapper;
using System;

namespace Kogel.Dapper.Extension
{
	public class AopProvider
	{
		/// <summary>
		/// 执行事件类型
		/// </summary>
		/// <param name="command">执行对象</param>
		public delegate void AopMethod(CommandDefinition command);

		/// <summary>
		/// 执行前启动
		/// </summary>
		public event AopMethod OnExecuting;

		/// <summary>
		/// 执行后启动
		/// </summary>
		public event EventHandler<AopMethod> OnExecuted;

		public void Cry(string msg)
		{
			OnExecuting(new CommandDefinition());
		}
	}
}