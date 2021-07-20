using System;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension.Test.Model
{
    [Display(Rename = "flow_order",AsName = "flow_order1")]
    public class FlowOrder : IBaseEntity<FlowOrder, long>
    {
        /// <summary>
        /// id
        /// </summary>
        [Identity(IsIncrease = true)]
        [Display(Rename = "id")]
        public override long Id { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        [Display(Rename = "order_number")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 参考号
        /// </summary>
        [Display(Rename = "reference_number")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [Display(Rename = "business_type")]
        public string BusinessType { get; set; }

        /// <summary>
        /// 客户代码
        /// </summary>
        [Display(Rename = "customer_code")]
        public string CustomerCode { get; set; }

        /// <summary>
        /// 账户编码
        /// </summary>
        [Display(Rename = "account_code")]
        public string AccountCode { get; set; }

        /// <summary>
        /// 单据时间
        /// </summary>
        [Display(Rename = "order_create_time")]
        public DateTime? OrderCreateTime { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Display(Rename = "source")]
        public int Source { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Rename = "create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Rename = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 妥投状态
        /// </summary>
        [Display(Rename = "delivered_status")]
        public short DeliveredStatus { get; set; } = 1;

        /// <summary>
        /// 妥投时间
        /// </summary>
        [Display(Rename = "delivered_time")]
        public DateTime? DeliveredTime { get; set; }

        /// <summary>
        /// 妥投接收时间
        /// </summary>
        [Display(Rename = "delivered_receive_time")]
        public DateTime? DeliveredReceiveTime { get; set; }
    }

    [Display(Rename = "flow_order")]
    public class FlowOrder1 : FlowOrder
    {

    }

    [Display(Rename = "flow_order")]
    public class FlowOrder2 : FlowOrder
    {

    }
}
