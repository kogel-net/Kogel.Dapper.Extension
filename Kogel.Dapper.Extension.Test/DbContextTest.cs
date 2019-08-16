using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Test.Model;
using Kogel.Dapper.Extension.MsSql;

namespace Kogel.Dapper.Extension.Test
{
    public class DbContextTest : DbContext
    {
        public DbSet<Comment> Comments { get; set; }
        /// <summary>
        /// 配置连接方式
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder = new DbContextOptionsBuilder()
                .UseConnectionString("Data Source=42.157.195.21,4344;Initial Catalog=Qx_Sport_Common;User ID=qxdev;Password=qxdev123456;")
                .UseSqlProvider(new MsSqlProvider());
            base.OnConfiguring(builder);
        }
    }

    public class test
    {
        public void send()
        {
            using (var ctx = new DbContextTest())
            {
                var commentList = ctx.Comments.Select().Where(x => x.UserId.In(new int[] { 1, 2, 3 })).ToList();
            }
        }
    }
}
