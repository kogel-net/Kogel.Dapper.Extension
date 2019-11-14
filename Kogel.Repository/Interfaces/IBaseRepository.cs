using Kogel.Dapper.Extension.Core.SetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Kogel.Dapper.Extension.Core.SetC;

namespace Kogel.Repository.Interfaces
{
	public interface IBaseRepository<T>
	{
		QuerySet<T> QuerySet();
		QuerySet<T> QuerySet(IDbTransaction transaction);
		CommandSet<T> CommandSet();
		CommandSet<T> CommandSet(IDbTransaction transaction);
	}
}
