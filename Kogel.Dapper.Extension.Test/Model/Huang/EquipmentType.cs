using Digiwin.MES.Server.Infrastructure.Core.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digiwin.MES.Server.Application.Domain.EQP.Entities
{
    public class EQP_TYPE_BAS : EntityBase
    {
        public string EQP_TYPE_NO { get; set; }
        public string EQP_TYPE_NAME { get; set; }
        public int IS_STOVE { get; set; }
    }
}
