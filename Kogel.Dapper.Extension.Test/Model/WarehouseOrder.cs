using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension.Test.Model
{
    /// <summary>
    /// 仓储订单表
    /// </summary>
    public class WarehouseOrder : IBaseEntity<WarehouseOrder, long>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Identity(IsIncrease = false)]
        [Display(Rename = "OrderId")]
        public override long Id { set; get; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNumber { set; get; }

        /// <summary>
        /// 参考号
        /// </summary>
        public string ReferenceNumber { set; get; }

        /// <summary>
        /// 客户代码
        /// </summary>
        public string CustomerCode { set; get; }

        /// <summary>
        /// 客户渐简称
        /// </summary>
        public string CustomerAbbreviation { set; get; }

        /// <summary>
        /// 账户编号
        /// </summary>
        public string AccountCode { set; get; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { set; get; }

        /// <summary>
        /// 物流产品代码
        /// </summary>
        public string LogisticsProducts { set; get; }

        /// <summary>
        /// 物流产品名称
        /// </summary>
        public string LogisticsProductName { set; get; }

        /// <summary>
        /// 计费币种
        /// </summary>
        public string CurrencyCode { set; get; }

        /// <summary>
        /// 计费总金额
        /// </summary>
        public decimal BillingAmount { set; get; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { set; get; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Total { set; get; }
        /// <summary>
        /// 区域仓库代码
        /// </summary>
        public string WarehouseCode { set; get; }
        /// <summary>
        /// 区域仓库名称
        /// </summary>
        public string WarehouseName { set; get; }

        /// <summary>
        /// 物理仓代码
        /// </summary>
        public string PhysicalWarehouseCode { set; get; }

        /// <summary>
        /// 物理仓名
        /// </summary>
        public string PhysicalWarehouseName { set; get; }

        /// <summary>
        /// 计费重量(KG)
        /// </summary>
        public decimal? Weight { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { set; get; }
        /// <summary>
        /// 邮编区域
        /// </summary>
        public string ZipCodeArea { set; get; }
        /// <summary>
        /// 目的国家
        /// </summary>
        public string DestinationCountry { set; get; }
        /// <summary>
        /// 跟踪号
        /// </summary>
        public string TrackNumber { set; get; }
        /// <summary>
        /// 材积(CM)
        /// </summary>
        public string Volume { set; get; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderType { set; get; }
        /// <summary>
        /// 费用状态 0未出账，1 已出账，2，正在出账
        /// </summary>
        public int ChargeType { set; get; }
        /// <summary>
        /// 账单生成时间
        /// </summary>
        public DateTime? BillAddTime { set; get; }
        /// <summary>
        /// 账单号
        /// </summary>
        public string BillNumber { set; get; }

        /// <summary>
        /// 费用发生时间=>订单签出时间
        /// </summary>
        public DateTime? BillingTime { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public int Source { set; get; }
        /// <summary>
        /// 来源编码
        /// </summary>
        public int SourceId { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime { set; get; }
        /// <summary>
        /// 售后单号
        /// </summary>
        public string ReturnNumber { get; set; }

        /// <summary>
        /// 售后类型
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderAddTime { get; set; }

        /// <summary>
        /// 切片Id,格式年月（201907)
        /// </summary>
        public int ShardingId { get; set; }

        /// <summary>
        /// 重新计费，0 否，1 是
        /// </summary>
        public int ReCharge { get; set; }

        /// <summary>
        /// 可以出账；默认为1-可出账，0-不可出账
        /// </summary>
        public bool? CanBill { get; set; }

        /// <summary>
        /// 最新单据费用发生变化时间,历史数据为空
        /// </summary>
        public DateTime? FlowUpdateTime { get; set; }

        /// <summary>
        /// 更新时间-》单据推送时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 海柜号
        /// </summary>
        public string SeaCabinetNo { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string ReceiptNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 海柜型号
        /// </summary>
        public string SeaCabinetType { get; set; }

        /// <summary>
        /// SKU种类数
        /// </summary>
        public int? SkuKinds { get; set; }

        /// <summary>
        /// 省州
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 箱数
        /// </summary>
        public string BoxNumber { get; set; }
    }
}
