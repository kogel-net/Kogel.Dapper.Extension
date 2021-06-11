using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension.Test.Model
{
    public class HeadOrder : IBaseEntity<HeadOrder, int>
    {
        /// <summary>
        /// 
        /// </summary>
        [Identity]
        [Display(Rename = "HeadOrderId")]
        public override int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransportOrderNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReferenceNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BusinessType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransportMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LogisticsProductsCode { get; set; }
        /// <summary>
        /// FPS-快递
        /// </summary>
        public string LogisticsProductsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DestinationCountry { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UnitPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CBM { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Carton { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ETD { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ETA { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderAddTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExportTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TargetWarehouseCode { get; set; }
        /// <summary>
        /// 美东仓库
        /// </summary>
        public string TargetWarehouseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransferWarehouseCode { get; set; }
        /// <summary>
        /// 深圳仓库
        /// </summary>
        public string TransferWarehouseName { get; set; }
        /// <summary>
        /// 空运-KY
        /// </summary>
        public string TransportTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CabinetType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerAbbreviation { get; set; }
    }
}
