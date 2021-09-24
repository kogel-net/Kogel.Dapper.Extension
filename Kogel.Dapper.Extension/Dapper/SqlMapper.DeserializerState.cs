using System;
using System.Data;

namespace Kogel.Dapper.Extension
{
    public static partial class SqlMapper
    {
        internal struct DeserializerState
        {
            public readonly int Hash;
            public readonly Func<IDataReader, object> Func;

            public DeserializerState(int hash, Func<IDataReader, object> func)
            {
                Hash = hash;
                Func = func;
            }
        }
    }
}
