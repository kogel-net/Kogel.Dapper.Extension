using Kogel.Dapper.Extension;

namespace Dapper
{
    public static partial class SqlMapper
    {
		/// <summary>
		/// Dummy type for excluding from multi-map
		/// </summary>
		public class DontMap : IBaseEntity<DontMap, int>
		{ /* hiding constructor */
			public override int Id { get; set; }
		}
	}
}
