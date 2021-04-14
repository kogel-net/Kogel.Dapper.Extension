using Kogel.Repository;
using MySql.Data.MySqlClient;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mysql.Repository
{
    public class IBaseRepository<T> : BaseRepository<T>
    {
        /// <summary>
        /// 配置连接
        /// </summary>
        /// <param name="builder"></param>
        public override void OnConfiguring(RepositoryOptionsBuilder builder)
        {
            builder.BuildConnection(new MySqlConnection(@"server=localhost;port=3306;user id=root;password=A5101264a;database=gc_fps_receivable;"))
                  .BuildConnection(new MySqlConnection(@"server=192.168.87.141;port=63307;user id=fps_dbuser_dev;password=kxx44cuvlmjluqncju;
                                 persistsecurityinfo=True;database=gc_fps_receivable_dev_2021_0;SslMode=none"), "fps_2021")
                  .BuildAutoSyncStructure(false)
                  .BuildProvider(new MySqlProvider());
        }
    }
}
