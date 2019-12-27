using Kogel.Dapper.Extension;

namespace Dapper
{
    public static partial class SqlMapper
    {
        /// <summary>
        /// Dummy type for excluding from multi-map
        /// </summary>
        public class DontMap:IBaseEntity<int> { /* hiding constructor */ }
    }
}
