using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Extension.From
{
    public class IJoin<T>
    {
        protected QuerySet<T> querySet { get; }

        public IJoin(QuerySet<T> querySet)
        {
            this.querySet = querySet;
        }


        public IJoin<T, T1> Join<T1, T2>(Expression<Func<T, T1, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            querySet.Join(expression, joinMode, isDisField);
            return new IJoin<T, T1>(querySet);
        }
    }

    public class IJoin<T, T1> : IJoin<T>
    {
        public IJoin(QuerySet<T> querySet) : base(querySet)
        {

        }


        public IJoin<T, T1> Join<T2, T3>(Expression<Func<T, T1, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            querySet.Join(expression, joinMode, isDisField);
          //  return new IJoin<T, T1>
        }


    }

    public class IJoin<T, T1, T2,T3,T4> : IJoin<T, T1>
    {
        public IJoin(QuerySet<T> querySet) : base(querySet)
        {

        }

        public IJoin<T, T1> Join<T3, T4>(Expression<Func<T, T1, bool>> expression, JoinMode joinMode = JoinMode.LEFT, bool isDisField = true)
        {
            querySet.Join(expression, joinMode, isDisField);
            return new IJoin<T, T1>(querySet);
        }
    }
}
