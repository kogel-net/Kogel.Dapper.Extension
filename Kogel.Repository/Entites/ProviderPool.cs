using Kogel.Dapper.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Repository.Entites
{
   public class ProviderPool
    {
        /// <summary>
        /// 
        /// </summary>
        public Func<SqlProvider, SqlProvider> FuncProvider { get; set; }

        /// <summary>
        /// 自定义名称
        /// </summary>
        public string DbName { get; set; }
    }
}
