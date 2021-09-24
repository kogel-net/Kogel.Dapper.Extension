using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.Test.Model;
using System;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension;
using System.Data;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mssql
{
    public class Query
    {

        public void Test()
        {
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHanlder1());
            using (var conn = new SqlConnection("server=localhost;database=Lige;user=sa;password=!RisingupTech/././.;Connection Timeout=60;"))
            {
                //var aaa = Guid.NewGuid().ToString();
                var guid = Guid.Parse("25661F78-7113-4BD1-91D4-0B42FC570789");
                var test = conn.QuerySet<TestModel>()
                    .Where(x => x.Id == guid)
                    .Get(x => new
                    {
                        x.Id
                    });
            }



        }

    }

    public class GuidTypeHanlder1 : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Size = 32;
            parameter.DbType = DbType.Object;
            //parameter.Value = value.ToString();
            parameter.Value = value;
        }

        public override Guid Parse(object value)
        {
            //return Guid.Parse((string)value);
            return Convert(value);
        }

        internal static Guid Convert(object value)
        {
            if (value.GetType().Name.Contains("Guid"))
            {
                return (Guid)value;
            }
            else
            {
                return Guid.Parse(value.ToString());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Display(Rename = "test")]
    public class TestModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}
