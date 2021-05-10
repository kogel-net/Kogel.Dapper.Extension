using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface ICommandSet<T> : ICommand<T>
    {
        ICommandSet<T> Where(Expression<Func<T, bool>> predicate);

        ICommandSet<T> Where(string sqlWhere, object param = null);

        ICommandSet<T> WhereIf(bool where, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate);

        ICommandSet<T> ResetTableName(Type type, string tableName);
    }
}
