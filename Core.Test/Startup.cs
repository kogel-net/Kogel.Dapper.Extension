using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kogel.Repository;
using MySql.Data.MySqlClient;
using Kogel.Dapper.Extension.MySql;
using Kogel.Dapper.Extension;
using Core.Test.Model;

namespace Core.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //简便版仓储定义
            services.AddKogelRepository((kogel) =>
            {
                kogel.BuildConnection(x => new MySqlConnection(@"server=192.168.0.120;port=3306;user id=root;password=123456;database=gc_fps_receivable;Persist Security Info=True;"));
                kogel.BuildProvider(x => new MySqlProvider());
            });

            ////多种不同类型数据库版仓储定义
            //services.AddTransient(typeof(IMySqlRepository<>), typeof(MySqlRepository<>));


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }


    public class MySqlRepository<T> : BaseRepository<T>, IMySqlRepository<T>
    {
        public override void OnConfiguring(RepositoryOptionsBuilder builder)
        {
            builder.BuildConnection(x => new MySqlConnection(@"server=192.168.0.120;port=3306;user id=root;password=123456;database=gc_fps_receivable;Persist Security Info=True;"));
            builder.BuildProvider(new MySqlProvider());
        }
    }


    public interface IMySqlRepository<T> : IRepository<T>
    {

    }
}
