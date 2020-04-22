using Digiwin.MES.Server.Infrastructure.Core.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digiwin.MES.Server.Application.Domain.EQP.Entities
{
    public class EQP_STATE_BAS: EntityBase
    {
        public int STATUE_NO { get; set; }
        public string STATUE_DESC { get; set; }
        public int ALLOW_ALTER { get; set; }

    }
}
