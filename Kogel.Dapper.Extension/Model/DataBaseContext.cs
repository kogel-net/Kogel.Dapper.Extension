using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;

namespace Kogel.Dapper.Extension
{
    public class DataBaseContext<T> : AbstractDataBaseContext
    {
        public QuerySet<T> QuerySet => (QuerySet<T>)Set;

        public CommandSet<T> CommandSet => (CommandSet<T>)Set;
    }

    public abstract class AbstractDataBaseContext
    {
        public AbstractSet Set { get; set; }

        public EOperateType OperateType { get; set; }
    }
}
