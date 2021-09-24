using Kogel.Dapper.Extension;

namespace Kogel.Dapper.Extension
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
