using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Domain.Enums
{
    public enum MQType
    {
        None = 0,
        WhenPurchasing,
        /// <summary>
        /// 采购退回
        /// </summary>
        WhenPurchasingReturn,
        /// <summary>
        /// 销售退回，發貨退回
        /// </summary>
        WhenReturn,
        WhenAddToCart,
        WhenDeleteCart,
        WhenModifyCart,
        WhenPay,
        WhenDeliveryArranged,
        /// <summary>
        /// 支付后取消
        /// </summary>
        WhenOrderCancel,
        /// <summary>
        /// 支付前超时或取消，回滚Hold货数量
        /// </summary>
        WhenPayTimeOut,
    }

    public enum MQState
    {
        /// <summary>
        /// 未处理
        /// </summary>
        UnDeal = 1,
        /// <summary>
        /// 已处理
        /// </summary>
        Deal = 2,
        /// <summary>
        /// 异常
        /// </summary>
        Exception = 3
    }
}
