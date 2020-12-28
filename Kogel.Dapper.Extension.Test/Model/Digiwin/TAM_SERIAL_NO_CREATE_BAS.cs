using Digiwin.MES.Server.Infrastructure.Core.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model.Digiwin
{
    public class TAM_SERIAL_NO_CREATE_BAS : EntityBase
    {
        /// <summary>
        /// 设备工序GUID
        /// </summary>
        public string SERIAL_GUID { get; set; }
        /// <summary>
        /// 工装GUID
        /// </summary>
        public string CLASS_GUID { get; set; }
        /// <summary>
        /// 组号
        /// </summary>
        public string GROUP_NO { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public decimal SERIAL_NUMBER { get; set; }
    }
}
