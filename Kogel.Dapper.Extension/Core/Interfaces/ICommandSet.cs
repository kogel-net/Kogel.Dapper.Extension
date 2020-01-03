using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
	public interface ICommandSet<T>: ICommand<T>
	{
		ICommand<T> Where(Expression<Func<T, bool>> predicate);

		ICommand<T> Where(string sqlWhere, object param = null);

		ICommand<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate);

		ICommand<T> AsTableName(Type type, string tableName);
	}
}
