using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kogel.Dapper.Extension.Core.Interfaces
{
    public interface ICommandSet<T>
    {
        ICommand<T> Where(Expression<Func<T, bool>> predicate);

        ICommand<T> AsTableName(Type type, string tableName);
    }
}
