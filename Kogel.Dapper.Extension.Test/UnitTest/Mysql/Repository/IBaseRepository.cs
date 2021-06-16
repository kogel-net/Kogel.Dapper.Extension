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
            builder.BuildConnection(x => new MySqlConnection(@"server=192.168.0.120;port=3306;user id=root;password=123456;database=gc_fps_receivable;Persist Security Info=True;"))
                  .BuildConnection(x => new MySqlConnection(@"server=192.168.87.141;port=63307;user id=fps_dbuser_dev;password=kxx44cuvlmjluqncju;
                                 persistsecurityinfo=True;database=gc_fps_receivable_dev_2021_0;SslMode=none;Persist Security Info=True;"), "fps_2021")
                  .BuildConnection(x => new MySqlConnection(@"server=192.168.87.203;port=63307;user id=fps_user;password=1Nl7C9CQRWhsEy1E;
                                 persistsecurityinfo=True;database=gc_fps_receivable_test;SslMode=none;Persist Security Info=True;"), "fps_test")
                  .BuildConnection(x => new MySqlConnection(@"server=192.168.87.203;port=63307;user id=fps_user;password=1Nl7C9CQRWhsEy1E;
                                 persistsecurityinfo=True;database=gc_fps_receivable_test_2021_0;SslMode=none;Persist Security Info=True;"), "fps_test_2021")
                  .BuildAutoSyncStructure(false)
                  .BuildProvider(new MySqlProvider());
        }
    }
}
