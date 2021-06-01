using Kogel.Dapper.Extension.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Core.SetQ
{
    public class QuerySet<T, TReturn> : QuerySet<T>, IQuerySet<T, TReturn>
    {
        public QuerySet(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {
        }

        public QuerySet(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {
        }

        public IQuery<T, TReturn> Select(Expression<Func<T, TReturn>> select)
        {
            SqlProvider.Context.Set.SelectExpression = select;
            return this as IQuery<T, TReturn>;
        }
    }
}
