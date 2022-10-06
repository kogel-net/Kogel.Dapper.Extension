using Kogel.Repository;
using System;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.MsSql;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mssql
{
    public class Command
    {
        public void Test()
        {
    
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Display(Rename = "test")]
    public class Test
    {
        /// <summary>
        /// 
        /// </summary>
        [Identity]
        [Display(Rename = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Rename = "name")]
        public string Name { get; set; }
    }
}
